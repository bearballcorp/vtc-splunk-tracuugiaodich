using Newtonsoft.Json.Linq;
using Vtc.Splunk.Log.Analyzer.Models;

namespace Vtc.Splunk.Log.Analyzer.Core;

public class DataExtractionService
{
    public TransactionInfo? ExtractTransactionInfo(JObject logEntry)
    {
        if (logEntry == null)
        {
            return null;
        }

        // FR-002: The system SHALL identify the first log entry containing `transactionEntityAttribute` JSON object
        // Assuming the transactionEntityAttribute is directly within the logEntry or a nested property
        // We need to find the first occurrence of "transactionEntityAttribute"
        JObject? transactionEntityAttribute = null;

        // Simple approach: check if it's a direct property
        if (logEntry.TryGetValue("transactionEntityAttribute", out JToken? token) && token is JObject obj)
        {
            transactionEntityAttribute = obj;
        }
        else
        {
            // More complex approach: search for it in nested properties
            // This might be too broad, but for now, let's assume it's a direct property or a simple nested one.
            // A more robust solution would involve a recursive search or a specific path.
            foreach (var property in logEntry.Properties())
            {
                if (property.Value is JObject nestedObject)
                {
                    if (nestedObject.TryGetValue("transactionEntityAttribute", out JToken? nestedToken) && nestedToken is JObject nestedObj)
                    {
                        transactionEntityAttribute = nestedObj;
                        break;
                    }
                }
            }
        }

        if (transactionEntityAttribute == null)
        {
            return null;
        }

        // FR-003: The system SHALL extract the following fields from the JSON:
        // - issuerBankName
        // - remitterName
        // - remitterAccountNumber
        var issuerBankName = transactionEntityAttribute["issuerBankName"]?.ToString();
        var remitterName = transactionEntityAttribute["remitterName"]?.ToString();
        var remitterAccountNumber = transactionEntityAttribute["remitterAccountNumber"]?.ToString();

        if (string.IsNullOrEmpty(issuerBankName) && string.IsNullOrEmpty(remitterName) && string.IsNullOrEmpty(remitterAccountNumber))
        {
            return null; // No relevant data found
        }

        return new TransactionInfo
        {
            IssuerBankName = issuerBankName,
            RemitterName = remitterName,
            RemitterAccountNumber = remitterAccountNumber
        };
    }
}