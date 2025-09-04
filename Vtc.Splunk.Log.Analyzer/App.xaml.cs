using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;
using Vtc.Splunk.Log.Analyzer.Core;
using Vtc.Splunk.Log.Analyzer.ViewModels;
using Vtc.Splunk.Log.Analyzer.Views.Pages;

namespace Vtc.Splunk.Log.Analyzer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.SetMinimumLevel(LogLevel.Debug); // Set minimum log level
        });

        // Add WPF-UI services
        services.AddNavigationViewPageProvider();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ITaskBarService, TaskBarService>();

        // Services
        services.AddSingleton<SplunkService>();
        services.AddSingleton<DataExtractionService>();
        services.AddSingleton<FileImportService>();
        services.AddSingleton<FileExportService>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<SettingsViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<DashboardPage>();
        services.AddSingleton<SettingsPage>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = ServiceProvider!.GetService<MainWindow>();
        mainWindow?.Show();
    }
}

