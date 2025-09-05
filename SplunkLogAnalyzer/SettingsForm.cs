using SplunkLogAnalyzer.Properties;
using System;
using System.Windows.Forms;

namespace SplunkLogAnalyzer
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            this.Load += SettingsForm_Load;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            btnTestConnection.Click += BtnTestConnection_Click;
        }

        private void SettingsForm_Load(object? sender, EventArgs e)
        {
            txtServerUrl.Text = Settings.Default.SplunkServerUrl;
            txtUsername.Text = Settings.Default.SplunkUsername;
            txtPassword.Text = Settings.Default.SplunkPassword; // Note: For security, this should be handled more carefully in a real app.
            numTimeout.Value = Settings.Default.ApiTimeout;
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            Settings.Default.SplunkServerUrl = txtServerUrl.Text;
            Settings.Default.SplunkUsername = txtUsername.Text;
            Settings.Default.SplunkPassword = txtPassword.Text;
            Settings.Default.ApiTimeout = (int)numTimeout.Value;
            Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void BtnTestConnection_Click(object? sender, EventArgs e)
        {
            // Simple test logic - we can create a dedicated method in SplunkService later
            try
            {
                using (var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true })
                using (var client = new HttpClient(handler))
                {
                    var baseUrl = txtServerUrl.Text.Replace("http://", "https://");
                    var uri = new Uri(new Uri(baseUrl), "/services/auth/login");
                    var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{txtUsername.Text}:{txtPassword.Text}"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                    // Use POST with form data as per Splunk docs
                    var requestData = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "username", txtUsername.Text },
                        { "password", txtPassword.Text }
                    };
                    var content = new System.Net.Http.FormUrlEncodedContent(requestData);

                    var response = await client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Connection failed. Status code: {response.StatusCode}\n\nDetails: {errorContent}", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                // Show the full exception details for better debugging
                MessageBox.Show($"An error occurred:\n\n{ex}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}