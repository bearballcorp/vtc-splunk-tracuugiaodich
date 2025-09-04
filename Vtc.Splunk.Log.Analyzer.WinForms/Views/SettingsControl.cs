using System;
using System.Windows.Forms;
using Vtc.Splunk.Log.Analyzer.WinForms.Core;

namespace Vtc.Splunk.Log.Analyzer.WinForms.Views
{
    public partial class SettingsControl : UserControl
    {
        private AppSettings _settings;

        public SettingsControl()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            LoadSettings();
            btnSave.Click += BtnSave_Click;
        }

        private void LoadSettings()
        {
            txtSplunkUrl.Text = _settings.SplunkUrl;
            txtUsername.Text = _settings.SplunkUsername;
            txtPassword.Text = _settings.SplunkPassword;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _settings.SplunkUrl = txtSplunkUrl.Text;
            _settings.SplunkUsername = txtUsername.Text;
            _settings.SplunkPassword = txtPassword.Text;
            _settings.Save();

            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}