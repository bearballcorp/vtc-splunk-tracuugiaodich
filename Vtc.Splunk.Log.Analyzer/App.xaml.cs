using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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

        // Ensure logs are flushed when the application exits
        Exit += (sender, e) => Serilog.Log.CloseAndFlush();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configure Serilog
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
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

