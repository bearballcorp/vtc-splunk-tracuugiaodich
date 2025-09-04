using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic; // Add this
using System.Collections.ObjectModel;
using System.IO; // Add this
using System.Linq; // Add this
using System.Threading.Tasks;
using Microsoft.Win32; // For OpenFileDialog and SaveFileDialog
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
        FileExportService fileExportService)
    {
        _splunkService = splunkService;
        _dataExtractionService = dataExtractionService;
        _fileImportService = fileImportService;
        _fileExportService = fileExportService;

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
                SearchProgress = $"Imported {CodesToSearch.Count} codes from {Path.GetFileName(openFileDialog.FileName)}.";
            }
            catch (Exception ex)
            {
                SearchProgress = $"Error importing codes: {ex.Message}";
                // Log the exception
            }
        }
    }

    private async Task Search()
    {
        IsSearching = true;
        SearchResults.Clear();
        SearchProgress = "Starting search...";

        try
        {
            for (int i = 0; i < CodesToSearch.Count; i++)
            {
                var code = CodesToSearch[i];
                SearchProgress = $"Searching for code {i + 1}/{CodesToSearch.Count}: {code}";

                try
                {
                    var splunkResults = await _splunkService.SearchLogs(code, StartDate, EndDate);

                    if (splunkResults != null && splunkResults.Any())
                    {
                        foreach (JToken logEntryToken in splunkResults)
                        {
                            if (logEntryToken is JObject logEntry)
                            {
                                var transactionInfo = _dataExtractionService.ExtractTransactionInfo(logEntry);
                                if (transactionInfo != null)
                                {
                                    transactionInfo.SearchCode = code;
                                    transactionInfo.SearchStatus = "Success";
                                    SearchResults.Add(transactionInfo);
                                    break; // Assuming we only need the first transactionEntityAttribute
                                }
                            }
                        }
                        // The 'logEntry' variable is now scoped within the 'if' block above.
                        // This block is a duplicate and should be removed.
                    }
                    else
                    {
                        SearchResults.Add(new TransactionInfo { SearchCode = code, SearchStatus = "No Data" });
                    }
                }
                catch (Exception ex)
                {
                    SearchResults.Add(new TransactionInfo { SearchCode = code, SearchStatus = $"Failed: {ex.Message}" });
                    // Log the exception
                }
            }
            SearchProgress = "Search complete.";
        }
        catch (Exception ex)
        {
            SearchProgress = $"An error occurred during search: {ex.Message}";
            // Log the exception
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
                SearchProgress = $"Results exported to {Path.GetFileName(saveFileDialog.FileName)}.";
            }
            catch (Exception ex)
            {
                SearchProgress = $"Error exporting results: {ex.Message}";
                // Log the exception
            }
        }
    }
}