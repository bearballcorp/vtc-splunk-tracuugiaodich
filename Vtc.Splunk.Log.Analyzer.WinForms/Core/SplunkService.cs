using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Add this
using Newtonsoft.Json.Linq;

namespace Vtc.Splunk.Log.Analyzer.Core;

public class SplunkService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SplunkService> _logger; // Add this
    private string? _splunkUrl;
    private string? _username;
    private string? _password;

    public SplunkService(ILogger<SplunkService> logger) // Inject ILogger
    {
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public void Configure(string splunkUrl, string username, string password)
    {
        _splunkUrl = splunkUrl;
        _username = username;
        _password = password;

        // Set up basic authentication header
        var authenticationString = $"{_username}:{_password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        _logger.LogInformation("SplunkService configured successfully.");
    }

    public async Task<JArray> SearchLogs(IEnumerable<string> codes, DateTime startTime, DateTime endTime, int timeoutSeconds = 30)
    {
        var codeQuery = string.Join(" OR ", codes);
        _logger.LogInformation($"Starting Splunk search for codes: '{codeQuery}' from {startTime} to {endTime}");

        if (string.IsNullOrEmpty(_splunkUrl) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
        {
            _logger.LogError("SplunkService is not configured. Cannot perform search.");
            throw new InvalidOperationException("SplunkService is not configured. Call Configure() first.");
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        // Splunk REST API endpoint for search jobs
        // Example: /services/search/jobs
        var searchEndpoint = $"{_splunkUrl!}/services/search/jobs";
        _logger.LogDebug($"Search endpoint: {searchEndpoint}");

        // Construct the search query
        var splunkQuery = $"search ({codeQuery}) earliest=\"{startTime:MM/dd/yyyy:HH:mm:ss}\" latest=\"{endTime:MM/dd/yyyy:HH:mm:ss}\"";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("output_mode", "json"),
            new KeyValuePair<string, string>("search", splunkQuery)
        });

        try
        {
            var response = await _httpClient.PostAsync(searchEndpoint, content);
            response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseString);

            // Extract SID (Search Job ID)
            var sid = jsonResponse["sid"]?.ToString();
            _logger.LogInformation($"Splunk search job created with SID: {sid}");

            if (string.IsNullOrEmpty(sid))
            {
                _logger.LogError("Failed to get SID from Splunk search job response.");
                throw new Exception("Failed to get SID from Splunk search job response.");
            }

            // Poll for search job completion
            var resultsEndpoint = $"{searchEndpoint}/{sid!}/results";
            JArray results = new JArray();

            while (true)
            {
                await Task.Delay(1000); // Wait for 1 second before polling again
                _logger.LogDebug($"Polling Splunk search job {sid} for results...");

                var resultsResponse = await _httpClient.GetAsync($"{resultsEndpoint}?output_mode=json");
                resultsResponse.EnsureSuccessStatusCode();

                var resultsString = await resultsResponse.Content.ReadAsStringAsync();
                var resultsJson = JObject.Parse(resultsString);

                bool isDone = resultsJson["entry"]?["content"]?["isDone"]?.ToObject<bool>() ?? false;

                if (isDone)
                {
                    results = resultsJson["entry"]?["content"]?["results"] as JArray ?? new JArray();
                    _logger.LogInformation($"Splunk search job {sid} completed. Found {results.Count} results.");
                    break;
                }
            }

            return results;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, $"HttpRequestException during Splunk search: {e.Message}");
            throw;
        }
        catch (TaskCanceledException e)
        {
            _logger.LogError(e, $"TaskCanceledException (Timeout) during Splunk search: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An unexpected exception occurred during Splunk search: {e.Message}");
            throw;
        }
    }
}