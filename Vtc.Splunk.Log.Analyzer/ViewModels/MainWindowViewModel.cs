using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;


namespace Vtc.Splunk.Log.Analyzer.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _applicationTitle = "Splunk Log Analyzer";

    [ObservableProperty]
    private ObservableCollection<NavigationViewItem> _navigationViewItems = new()
    {
        new NavigationViewItem()
        {
            Content = "Dashboard",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(Views.Pages.DashboardPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<NavigationViewItem> _navigationViewFooter = new()
    {
        new NavigationViewItem()
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(Views.Pages.SettingsPage)
        }
    };
}