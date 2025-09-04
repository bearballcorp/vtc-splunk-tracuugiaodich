using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Vtc.Splunk.Log.Analyzer.Core;
using Vtc.Splunk.Log.Analyzer.Models;
using Vtc.Splunk.Log.Analyzer.WinForms.Core;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Vtc.Splunk.Log.Analyzer.WinForms.Views
{
    public partial class DashboardControl : UserControl
    {
        private readonly SplunkService _splunkService;
        private readonly FileImportService _fileImportService;
        private readonly DataExtractionService _dataExtractionService;
        private readonly FileExportService _fileExportService;
        private List<string> _searchCodes;
        private List<TransactionInfo> _results;

        public DashboardControl()
        {
            InitializeComponent();

            var loggerFactory = new NullLoggerFactory();
            _splunkService = new SplunkService(new NullLogger<SplunkService>());
            _fileImportService = new FileImportService(new NullLogger<FileImportService>());
            _dataExtractionService = new DataExtractionService();
            _fileExportService = new FileExportService(new NullLogger<FileExportService>());
            _searchCodes = new List<string>();
            _results = new List<TransactionInfo>();

            btnImport.Click += BtnImport_Click;
            btnSearch.Click += BtnSearch_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportCsv.Click += BtnExportCsv_Click;
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "All Supported Files|*.csv;*.xlsx;*.xls;*.txt|CSV Files|*.csv|Excel Files|*.xlsx;*.xls|Text Files|*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _searchCodes = _fileImportService.ImportCodesFromFile(ofd.FileName);
                        lblStatus.Text = $"{_searchCodes.Count} codes imported.";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            if (!_searchCodes.Any())
            {
                MessageBox.Show("Please import search codes first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var settings = AppSettings.Load();
            if (string.IsNullOrEmpty(settings.SplunkUrl) || string.IsNullOrEmpty(settings.SplunkUsername))
            {
                MessageBox.Show("Please configure Splunk settings first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                lblStatus.Text = "Searching...";
                btnSearch.Enabled = false;
                _splunkService.Configure(settings.SplunkUrl, settings.SplunkUsername, settings.SplunkPassword);

                _results.Clear();
                var searchResults = await _splunkService.SearchLogs(_searchCodes, dtpStartTime.Value, dtpEndTime.Value);

                foreach (var result in searchResults)
                {
                    var transactionInfo = _dataExtractionService.ExtractTransactionInfo(result as Newtonsoft.Json.Linq.JObject);
                    if (transactionInfo != null)
                    {
                        _results.Add(transactionInfo);
                    }
                }

                dgvResults.DataSource = null;
                dgvResults.DataSource = _results;
                lblStatus.Text = $"Search complete. Found {_results.Count} results.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error during search.";
                MessageBox.Show($"An error occurred during search: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSearch.Enabled = true;
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (!_results.Any())
            {
                MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _fileExportService.ExportToExcel(_results, sfd.FileName);
                        MessageBox.Show("Export successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            if (!_results.Any())
            {
                MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File|*.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _fileExportService.ExportToCsv(_results, sfd.FileName);
                        MessageBox.Show("Export successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}