using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic; // Add this
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO; // Add this
using System.Linq; // Add this
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32; // For OpenFileDialog and SaveFileDialog
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq; // Add this
using Vtc.Splunk.Log.Analyzer.Core;
using Vtc.Splunk.Log.Analyzer.Models;

namespace Vtc.Splunk.Log.Analyzer.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly SplunkService _splunkService;
    private readonly DataExtractionService _dataExtractionService;
    private readonly FileImportService _fileImportService;
    private readonly FileExportService _fileExportService;
    private readonly ILogger<DashboardViewModel> _logger;

    [ObservableProperty]
    private string _pageTitle = "Dashboard";

    [ObservableProperty]
    private ObservableCollection<string> _codesToSearch = new();

    [ObservableProperty]
    private DateTime _startDate = DateTime.Now.AddDays(-1);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Now;

    [ObservableProperty]
    private ObservableCollection<TransactionInfo> _searchResults = new();

    [ObservableProperty]
    private bool _isSearching = false;

    [ObservableProperty]
    private string _searchProgress = string.Empty;

    public IRelayCommand ImportCodesCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }
    public IRelayCommand ExportResultsCommand { get; }

    public DashboardViewModel(
        SplunkService splunkService,
        DataExtractionService dataExtractionService,
        FileImportService fileImportService,
        FileExportService fileExportService,
        ILogger<DashboardViewModel> logger)
    {
        _splunkService = splunkService;
        _dataExtractionService = dataExtractionService;
        _fileImportService = fileImportService;
        _fileExportService = fileExportService;
        _logger = logger;

        ImportCodesCommand = new RelayCommand(ImportCodes);
        SearchCommand = new AsyncRelayCommand(Search);
        ExportResultsCommand = new RelayCommand(ExportResults);
    }

    private void ImportCodes()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
        openFileDialog.Multiselect = false;

        if (openFileDialog.ShowDialog() == true) // Use 'true' for OK
        {
            try
            {
                var importedCodes = _fileImportService.ImportCodesFromFile(openFileDialog.FileName);
                CodesToSearch.Clear();
                foreach (var code in importedCodes)
                {
                    CodesToSearch.Add(code);
                }
                var message = $"Imported {CodesToSearch.Count} codes from {Path.GetFileName(openFileDialog.FileName)}.";
                SearchProgress = message;
                _logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                SearchProgress = $"Error importing codes: {ex.Message}";
                _logger.LogError(ex, "An error occurred while importing codes from {FilePath}", openFileDialog.FileName);
                MessageBox.Show($"An error occurred while importing codes: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async Task Search()
    {
        IsSearching = true;
        SearchResults.Clear();
        SearchProgress = "Starting search...";
        var searchResultsBag = new ConcurrentBag<TransactionInfo>();
        var processedCount = 0;

        try
        {
            var splunkResults = await _splunkService.SearchLogs(CodesToSearch, StartDate, EndDate);

            if (splunkResults != null && splunkResults.Any())
            {
                foreach (JToken logEntryToken in splunkResults)
                {
                    if (logEntryToken is JObject logEntry)
                    {
                        var transactionInfo = _dataExtractionService.ExtractTransactionInfo(logEntry);
                        if (transactionInfo != null)
                        {
                            searchResultsBag.Add(transactionInfo);
                        }
                    }
                }
            }

            var foundCodes = new HashSet<string>(searchResultsBag.Select(r => r.SearchCode));
            foreach (var code in CodesToSearch)
            {
                if (!foundCodes.Contains(code))
                {
                    searchResultsBag.Add(new TransactionInfo { SearchCode = code, SearchStatus = "No Data" });
                }
            }

            SearchResults.Clear();
            foreach (var item in searchResultsBag.OrderBy(r => r.SearchCode))
            {
                SearchResults.Add(item);
            }

            SearchProgress = $"Search complete. Found {searchResultsBag.Count} results for {CodesToSearch.Count} codes.";
            _logger.LogInformation("Search operation completed successfully.");
        }
        catch (Exception ex)
        {
            SearchProgress = $"An error occurred during search: {ex.Message}";
            _logger.LogError(ex, "An unexpected error occurred during the search operation.");
            MessageBox.Show($"An error occurred during the search operation: {ex.Message}", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsSearching = false;
        }
    }

    private void ExportResults()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx";
        saveFileDialog.FileName = $"SplunkLogAnalysis_{DateTime.Now:yyyyMMdd_HHmmss}";

        if (saveFileDialog.ShowDialog() == true) // Use 'true' for OK
        {
            try
            {
                if (saveFileDialog.FileName.EndsWith(".csv"))
                {
                    _fileExportService.ExportToCsv(SearchResults.ToList(), saveFileDialog.FileName);
                }
                else if (saveFileDialog.FileName.EndsWith(".xlsx"))
                {
                    _fileExportService.ExportToExcel(SearchResults.ToList(), saveFileDialog.FileName);
                }
                var message = $"Results exported to {Path.GetFileName(saveFileDialog.FileName)}.";
                SearchProgress = message;
                _logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                SearchProgress = $"Error exporting results: {ex.Message}";
                _logger.LogError(ex, "An error occurred while exporting results to {FilePath}", saveFileDialog.FileName);
                MessageBox.Show($"An error occurred while exporting results: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}