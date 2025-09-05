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
        private readonly HttpClient _httpClient;

        public SplunkService()
        {
            // It's better to use IHttpClientFactory in a real-world app, but for simplicity, we'll create it here.
            // We also ignore SSL certificate validation for development purposes. This should be configured properly in production.
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            var settings = Settings.Default;
            if (string.IsNullOrWhiteSpace(settings.SplunkServerUrl))
            {
                throw new InvalidOperationException("Splunk server URL is not configured.");
            }

            _httpClient.BaseAddress = new Uri(settings.SplunkServerUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.SplunkUsername}:{settings.SplunkPassword}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            _httpClient.Timeout = TimeSpan.FromSeconds(settings.ApiTimeout);
        }

        public async Task<SearchResult> SearchLogsAsync(string code, DateTime startTime, DateTime endTime)
        {
            // Re-configure client in case settings have changed
            ConfigureHttpClient();

            var searchQuery = $"search {code} source=\"bankgatev2-public\"";
            var requestData = new Dictionary<string, string>
            {
                { "search", searchQuery },
                { "earliest_time", startTime.ToString("o") },
                { "latest_time", endTime.ToString("o") },
                { "output_mode", "json" }
            };

            var content = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync("/services/search/jobs/export", content);

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
                    string rawLog = item.result._raw;
                    if (rawLog.Contains("transactionEntityAttribute"))
                    {
                        // This is a simplified extraction. A more robust solution would parse the JSON properly.
                        try
                        {
                            var transactionJson = ExtractTransactionJson(rawLog);
                            dynamic? transactionData = JsonConvert.DeserializeObject<dynamic>(transactionJson);

                            if (transactionData != null)
                            {
                                return new SearchResult
                                {
                                    SearchCode = code,
                                    IssuerBankName = transactionData.issuerBankName,
                                    RemitterName = transactionData.remitterName,
                                    RemitterAccountNumber = transactionData.remitterAccountNumber,
                                    Status = SearchStatus.Success,
                                    Timestamp = DateTime.Now
                                };
                            }
                        }
                        catch
                        {
                            // Ignore parsing errors and continue to the next log entry
                        }
                    }
                }
            }

            return new SearchResult { SearchCode = code, Status = SearchStatus.NoData, Timestamp = DateTime.Now };
        }

        private string ExtractTransactionJson(string rawLog)
        {
            const string startMarker = "\"transactionEntityAttribute\":";
            var startIndex = rawLog.IndexOf(startMarker);
            if (startIndex == -1) return string.Empty;

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
                return jsonSubstring.Substring(0, endIndex + 1);
            }

            return string.Empty;
        }
    }
}