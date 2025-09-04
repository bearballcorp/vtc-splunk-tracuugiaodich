using CommunityToolkit.Mvvm.ComponentModel;

namespace Vtc.Splunk.Log.Analyzer.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _pageTitle = "Settings";

    public SettingsViewModel()
    {
        
    }
}