using System;

namespace Vtc.Splunk.Log.Analyzer.Models;

public class TransactionInfo
{
    public string? SearchCode { get; set; }

    public string? IssuerBankName { get; set; }

    public string? RemitterName { get; set; }

    public string? RemitterAccountNumber { get; set; }

    public string? SearchStatus { get; set; } // Enum or string for status (Success/Failed/No Data)

    public DateTime Timestamp { get; set; }
}