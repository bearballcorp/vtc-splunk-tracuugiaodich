using System;
using System.Windows.Forms;
using Vtc.Splunk.Log.Analyzer.WinForms.Views;

namespace Vtc.Splunk.Log.Analyzer.WinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
        dashboardToolStripMenuItem.Click += DashboardToolStripMenuItem_Click;
        settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;

        // Show Dashboard by default
        ShowUserControl(new DashboardControl());
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void DashboardToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowUserControl(new DashboardControl());
    }

    private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowUserControl(new SettingsControl());
    }

    private void ShowUserControl(UserControl control)
    {
        contentPanel.Controls.Clear();
        control.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(control);
    }
}
