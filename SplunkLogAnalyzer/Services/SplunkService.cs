using Newtonsoft.Json;
using SplunkLogAnalyzer.Models;
using SplunkLogAnalyzer.Properties;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SplunkLogAnalyzer.Services
{
    public class SplunkService
    {
        private static readonly System.Text.RegularExpressions.Regex _jsonRegex = new System.Text.RegularExpressions.Regex(@"\{.*?\}", System.Text.RegularExpressions.RegexOptions.Singleline);
        private static readonly System.Text.RegularExpressions.Regex _normalizeRegex = new System.Text.RegularExpressions.Regex(@"(\d+)");

        public SplunkService()
        {
            // HttpClient will be created per request to avoid the "already started" error
        }

        private void ConfigureHttpClient(HttpClient httpClient)
        {
            var settings = Settings.Default;
            if (string.IsNullOrWhiteSpace(settings.SplunkServerUrl))
            {
                throw new InvalidOperationException("Splunk server URL is not configured.");
            }

            // Thay đổi thành HTTPS
            var baseUrl = settings.SplunkServerUrl.Replace("http://", "https://");
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.SplunkUsername}:{settings.SplunkPassword}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            httpClient.Timeout = TimeSpan.FromSeconds(settings.ApiTimeout);
        }

        public async Task<SearchResult> SearchLogsAsync(string code, DateTime startTime, DateTime endTime)
        {
            // Create a new HttpClient instance for each request to avoid the "already started" error
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var httpClient = new HttpClient(handler);
            ConfigureHttpClient(httpClient);

            var searchQuery = $"search {code} source=\"bankgatev2-public\"";
            var requestData = new Dictionary<string, string>
            {
                { "search", searchQuery },
                { "earliest_time", startTime.ToString("o") },
                { "latest_time", endTime.ToString("o") },
                { "output_mode", "json" }
            };

            var content = new FormUrlEncodedContent(requestData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync("/services/search/jobs/export", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Splunk API request failed with status code {response.StatusCode}: {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            
            // The export endpoint returns a stream of JSON objects. We need to find the one we need.
            var jsonObjects = responseString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var jsonObject in jsonObjects)
            {
                dynamic? item = JsonConvert.DeserializeObject<dynamic>(jsonObject);
                if (item != null && item.result != null && item.result._raw != null)
                {
                    string? rawLog = item.result._raw?.ToString();
                    if (!string.IsNullOrEmpty(rawLog) && rawLog.Contains("transactionEntityAttribute"))
                    {
                        // Parse the entire raw log as JSON and extract transactions from the array
                        try
                        {
                            // Find the JSON part (skip timestamp and other prefix)
                            var jsonStart = rawLog.IndexOf("{");
                            if (jsonStart != -1)
                            {
                                var jsonPart = rawLog.Substring(jsonStart);
                                
                                // Find the end of JSON (before any additional text like "signature:")
                                var jsonEnd = jsonPart.LastIndexOf("}");
                                if (jsonEnd != -1)
                                {
                                    jsonPart = jsonPart.Substring(0, jsonEnd + 1);
                                }
                                
                                // Unescape JSON if needed
                                jsonPart = System.Text.RegularExpressions.Regex.Unescape(jsonPart);
                                
                                var logData = Newtonsoft.Json.Linq.JObject.Parse(jsonPart);

                            var transactions = ExtractTransactionsFromJson(jsonPart);
                            foreach (var transactionEntityAttribute in transactions)
                            {
                                var partnerCode = transactionEntityAttribute["partnerCustomerCode"]?.ToString();
                                if (!string.IsNullOrEmpty(partnerCode) && MatchesCode(partnerCode, code))
                                {
                                    return new SearchResult
                                    {
                                        SearchCode = code,
                                        IssuerBankName = transactionEntityAttribute["issuerBankName"]?.ToString() ?? "",
                                        RemitterName = transactionEntityAttribute["remitterName"]?.ToString() ?? "",
                                        RemitterAccountNumber = transactionEntityAttribute["remitterAccountNumber"]?.ToString() ?? "",
                                        Status = SearchStatus.Success,
                                        Timestamp = DateTime.Now
                                    };
                                }
                            }
                            }
                        }
                        catch (Exception)
                        {
                            // Fallback to old method if JSON parsing fails
                            var transactions = ExtractAllTransactionJsons(rawLog);
                            foreach (var transactionJson in transactions)
                            {
                                dynamic? transactionData = JsonConvert.DeserializeObject<dynamic>(transactionJson);
                                var partnerCode = transactionData?.partnerCustomerCode?.ToString();
                                if (!string.IsNullOrEmpty(partnerCode) && MatchesCode(partnerCode, code))
                                {
                                    return new SearchResult
                                    {
                                        SearchCode = code,
                                        IssuerBankName = transactionData?.issuerBankName?.ToString() ?? "",
                                        RemitterName = transactionData?.remitterName?.ToString() ?? "",
                                        RemitterAccountNumber = transactionData?.remitterAccountNumber?.ToString() ?? "",
                                        Status = SearchStatus.Success,
                                        Timestamp = DateTime.Now
                                    };
                                }
                            }
                        }
                    }
                }
            }

            return new SearchResult { SearchCode = code, Status = SearchStatus.NoData, Timestamp = DateTime.Now };
        }

        private List<string> ExtractAllTransactionJsons(string rawLog)
        {
            var transactions = new List<string>();
            const string startMarker = "\"transactionEntityAttribute\":";
            var startIndex = 0;

            while ((startIndex = rawLog.IndexOf(startMarker, startIndex)) != -1)
            {
                startIndex += startMarker.Length;
                var jsonSubstring = rawLog.Substring(startIndex).Trim();

                // Find the matching closing brace for the JSON object
                int braceCount = 0;
                int endIndex = -1;
                for (int i = 0; i < jsonSubstring.Length; i++)
                {
                    if (jsonSubstring[i] == '{') braceCount++;
                    else if (jsonSubstring[i] == '}') braceCount--;

                    if (braceCount == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }

                if (endIndex != -1)
                {
                    var transactionJson = jsonSubstring.Substring(0, endIndex + 1);
                    transactions.Add(transactionJson);
                }

                // Move past this transaction to find the next one
                startIndex += jsonSubstring.Length;
            }

            return transactions;
        }

        private bool MatchesCode(string partnerCode, string searchCode)
        {
            // Direct contains
            if (partnerCode.Contains(searchCode)) return true;

            // Normalize by removing leading zeros in numeric parts
            var normalizedPartner = NormalizeCode(partnerCode);
            var normalizedSearch = NormalizeCode(searchCode);

            return normalizedPartner.Contains(normalizedSearch) || normalizedSearch.Contains(normalizedPartner);
        }

        private List<Newtonsoft.Json.Linq.JToken> ExtractTransactionsFromJson(string jsonPart)
        {
            var logData = Newtonsoft.Json.Linq.JObject.Parse(jsonPart);
            var transactions = new List<Newtonsoft.Json.Linq.JToken>();

            // Navigate to transactions array
            var transactionArray = logData["requestParameters"]?["request"]?["requestParams"]?["transactions"] as Newtonsoft.Json.Linq.JArray;

            if (transactionArray != null)
            {
                foreach (var transaction in transactionArray)
                {
                    var transactionEntityAttribute = transaction["transactionEntityAttribute"];
                    if (transactionEntityAttribute != null)
                    {
                        transactions.Add(transactionEntityAttribute);
                    }
                }
            }

            return transactions;
        }

        private string NormalizeCode(string code)
        {
            // Remove leading zeros from numeric parts
            // Example: VTCMS0079194023 -> VTCMS79194023
            var parts = _normalizeRegex.Split(code);
            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out _))
                {
                    parts[i] = parts[i].TrimStart('0');
                    if (string.IsNullOrEmpty(parts[i])) parts[i] = "0";
                }
            }
            return string.Join("", parts);
        }
    }
}