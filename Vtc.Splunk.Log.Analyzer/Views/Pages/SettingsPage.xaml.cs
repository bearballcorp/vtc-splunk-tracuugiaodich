using System.Windows.Controls;
using Wpf.Ui.Controls;
using Vtc.Splunk.Log.Analyzer.ViewModels;

namespace Vtc.Splunk.Log.Analyzer.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        DataContext = ViewModel;
    }
}