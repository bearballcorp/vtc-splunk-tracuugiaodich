using SplunkLogAnalyzer.Models;
using SplunkLogAnalyzer.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SplunkLogAnalyzer
{
    public partial class MainForm : Form
    {
        private readonly SplunkService _splunkService;
        private readonly BindingList<CodeItem> _codeList = new BindingList<CodeItem>();
        private readonly BindingList<SearchResult> _resultsList = new BindingList<SearchResult>();
        private readonly BackgroundWorker _searchWorker = new BackgroundWorker();
        private Button btnSearchSelected;

        public MainForm()
        {
            InitializeComponent();
            _splunkService = new SplunkService();

            // Bind data sources
            dgvCodeList.DataSource = _codeList;
            dgvResults.DataSource = _resultsList;

            // Configure BackgroundWorker
            _searchWorker.WorkerReportsProgress = true;
            _searchWorker.WorkerSupportsCancellation = true;
            _searchWorker.DoWork += SearchWorker_DoWork;
            _searchWorker.ProgressChanged += SearchWorker_ProgressChanged;
            _searchWorker.RunWorkerCompleted += SearchWorker_RunWorkerCompleted;

            // Register event handlers
            settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;
            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
            btnImport.Click += BtnImport_Click;
            btnSearch.Click += BtnSearch_Click;
            exportButton.Click += ExportButton_Click;

            // Configure DataGridView
            dgvCodeList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCodeList.MultiSelect = true;
            dgvCodeList.ClearSelection();

            // Add Search Selected button
            btnSearchSelected = new Button();
            btnSearchSelected.Location = new Point(387, 20);
            btnSearchSelected.Name = "btnSearchSelected";
            btnSearchSelected.Size = new Size(100, 27);
            btnSearchSelected.TabIndex = 3;
            btnSearchSelected.Text = "Search Selected";
            btnSearchSelected.UseVisualStyleBackColor = true;
            groupBox1.Controls.Add(btnSearchSelected);
            btnSearchSelected.Click += BtnSearchSelected_Click;
        }

        private void SearchWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Search was cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (e.Error != null)
            {
                MessageBox.Show($"An error occurred during the search: {e.Error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Search completed.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Re-enable UI controls
            btnSearch.Enabled = true;
            btnSearchSelected.Enabled = true;
            btnAdd.Enabled = true;
            btnRemove.Enabled = true;
            btnImport.Enabled = true;
            progressBar.Value = 0;
        }

        private void SearchWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            if (e.UserState is SearchResult result)
            {
                var existingResult = _resultsList.FirstOrDefault(r => r.SearchCode == result.SearchCode);
                if (existingResult != null)
                {
                    // Update existing result
                    existingResult.IssuerBankName = result.IssuerBankName;
                    existingResult.RemitterName = result.RemitterName;
                    existingResult.RemitterAccountNumber = result.RemitterAccountNumber;
                    existingResult.Status = result.Status;
                    existingResult.Timestamp = result.Timestamp;
                    _resultsList.ResetItem(_resultsList.IndexOf(existingResult));
                }
                else
                {
                    // Add new result
                    if (InvokeRequired)
                    {
                        Invoke((MethodInvoker)(() => _resultsList.Add(result)));
                    }
                    else
                    {
                        _resultsList.Add(result);
                    }
                }
                var codeItem = _codeList.FirstOrDefault(c => c.Code == result.SearchCode);
                if (codeItem != null)
                {
                    codeItem.Status = result.Status.ToString();
                    _codeList.ResetItem(_codeList.IndexOf(codeItem));
                }
            }
        }

        private void SearchWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            if (worker == null) return;

            var codesToSearch = (List<CodeItem>)e.Argument;
            int totalCodes = codesToSearch.Count;
            int processedCount = 0;

            foreach (var codeItem in codesToSearch)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                try
                {
                    // Update status in UI thread before searching
                    this.Invoke((MethodInvoker)delegate
                    {
                        codeItem.Status = "Searching...";
                        _codeList.ResetItem(_codeList.IndexOf(codeItem));
                    });

                    // Perform the search
                    var result = _splunkService.SearchLogsAsync(codeItem.Code, dtpStartTime.Value, dtpEndTime.Value).Result;
                    worker.ReportProgress((int)((++processedCount / (double)totalCodes) * 100), result);
                }
                catch (Exception ex)
                {
                    var errorResult = new SearchResult
                    {
                        SearchCode = codeItem.Code,
                        Status = SearchStatus.Failed,
                        RemitterName = ex.InnerException?.Message ?? ex.Message
                    };
                    worker.ReportProgress((int)((++processedCount / (double)totalCodes) * 100), errorResult);
                }
            }
        }

        private void BtnSearchSelected_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.SplunkServerUrl))
            {
                MessageBox.Show("Splunk server URL is not configured. Please configure it in the Settings menu.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get selected codes
            var selectedCodes = dgvCodeList.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem as CodeItem)
                .Where(item => item != null)
                .ToList();

            if (selectedCodes.Count == 0)
            {
                MessageBox.Show("Please select at least one code to search.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_searchWorker.IsBusy)
            {
                _searchWorker.CancelAsync();
            }
            else
            {
                // Reset status for selected codes
                foreach (var item in selectedCodes)
                {
                    item.Status = "Pending";
                }
                _codeList.ResetBindings();

                btnSearch.Enabled = false;
                btnSearchSelected.Enabled = false;
                btnAdd.Enabled = false;
                btnRemove.Enabled = false;
                btnImport.Enabled = false;
                _searchWorker.RunWorkerAsync(selectedCodes);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.SplunkServerUrl))
            {
                MessageBox.Show("Splunk server URL is not configured. Please configure it in the Settings menu.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_codeList.Count == 0)
            {
                MessageBox.Show("Please add at least one code to search.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_searchWorker.IsBusy)
            {
                _searchWorker.CancelAsync();
            }
            else
            {
                // Reset status and results
                _resultsList.Clear();
                foreach (var item in _codeList)
                {
                    item.Status = "Pending";
                }
                _codeList.ResetBindings();

                btnSearch.Enabled = false;
                btnSearchSelected.Enabled = false;
                btnAdd.Enabled = false;
                btnRemove.Enabled = false;
                btnImport.Enabled = false;
                _searchWorker.RunWorkerAsync(_codeList.ToList());
            }
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                ofd.Title = "Import Codes";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = File.ReadAllLines(ofd.FileName);
                        int importedCount = 0;
                        foreach (var line in lines)
                        {
                            var trimmedCode = line.Trim();
                            if (!string.IsNullOrEmpty(trimmedCode))
                            {
                                _codeList.Add(new CodeItem { Code = trimmedCode });
                                importedCount++;
                            }
                        }
                        MessageBox.Show($"{importedCount} codes imported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while importing the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            if (dgvCodeList.SelectedRows.Count > 0)
            {
                var itemsToRemove = dgvCodeList.SelectedRows
                                               .Cast<DataGridViewRow>()
                                               .Select(row => row.DataBoundItem as CodeItem)
                                               .Where(item => item != null)
                                               .ToList();

                foreach (var item in itemsToRemove)
                {
                    _codeList.Remove(item);
                }
            }
            else
            {
                MessageBox.Show("Please select one or more codes to remove.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var code = txtManualCode.Text.Trim();
            if (!string.IsNullOrEmpty(code))
            {
                _codeList.Add(new CodeItem { Code = code });
                txtManualCode.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a code.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

        private void ExportButton_Click(object? sender, EventArgs e)
        {
            if (_resultsList.Count == 0)
            {
                MessageBox.Show("There are no results to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.Title = "Export Results";
                sfd.FileName = $"SplunkResults_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new System.Text.StringBuilder();
                        // Add header
                        sb.AppendLine("SearchCode,IssuerBankName,RemitterName,RemitterAccountNumber,Status,Timestamp");

                        // Add rows
                        foreach (var result in _resultsList)
                        {
                            sb.AppendLine($"\"{result.SearchCode}\",\"{result.IssuerBankName}\",\"{result.RemitterName}\",\"{result.RemitterAccountNumber}\",\"{result.Status}\",\"{result.Timestamp:o}\"");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString());
                        MessageBox.Show("Results exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while exporting the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
