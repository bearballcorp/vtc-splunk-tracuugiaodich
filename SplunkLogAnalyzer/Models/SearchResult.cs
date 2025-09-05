namespace SplunkLogAnalyzer.Models
{
    public class SearchResult
    {
        public string SearchCode { get; set; }
        public string IssuerBankName { get; set; }
        public string RemitterName { get; set; }
        public string RemitterAccountNumber { get; set; }
        public SearchStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum SearchStatus
    {
        Pending,
        Success,
        Failed,
        NoData
    }
}