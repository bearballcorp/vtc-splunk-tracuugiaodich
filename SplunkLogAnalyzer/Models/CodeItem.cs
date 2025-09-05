using System.ComponentModel;

namespace SplunkLogAnalyzer.Models
{
    public class CodeItem
    {
        public string Code { get; set; }
        public string Status { get; set; } = "Pending";
    }
}