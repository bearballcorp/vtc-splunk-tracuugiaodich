using System.IO;
using Newtonsoft.Json;

namespace Vtc.Splunk.Log.Analyzer.WinForms.Core
{
    public class AppSettings
    {
        public string SplunkUrl { get; set; }
        public string SplunkUsername { get; set; }
        public string SplunkPassword { get; set; }

        private static readonly string SettingsFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsFilePath))
            {
                return new AppSettings();
            }

            string json = File.ReadAllText(SettingsFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }
    }
}