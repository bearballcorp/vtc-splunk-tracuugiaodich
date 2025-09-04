using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

using Vtc.Splunk.Log.Analyzer.ViewModels;
using Vtc.Splunk.Log.Analyzer.Views.Pages;
using Wpf.Ui.Abstractions;

namespace Vtc.Splunk.Log.Analyzer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow()
    {
        ViewModel = App.ServiceProvider!.GetRequiredService<MainWindowViewModel>();
        DataContext = ViewModel;

        InitializeComponent();

        var pageProvider = App.ServiceProvider!.GetRequiredService<INavigationViewPageProvider>();
        NavigationView.SetPageProviderService(pageProvider);
        NavigationView.Navigate(typeof(DashboardPage));
    }
}