using System.Windows.Controls;
using Wpf.Ui.Controls;
using Vtc.Splunk.Log.Analyzer.ViewModels;

namespace Vtc.Splunk.Log.Analyzer.Views.Pages;

/// <summary>
/// Interaction logic for DashboardPage.xaml
/// </summary>
public partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        DataContext = ViewModel;
    }
}