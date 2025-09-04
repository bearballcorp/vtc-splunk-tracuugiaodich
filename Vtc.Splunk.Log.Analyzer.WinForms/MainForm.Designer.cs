namespace Vtc.Splunk.Log.Analyzer.WinForms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.dashboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.contentPanel = new System.Windows.Forms.Panel();
        this.menuStrip1.SuspendLayout();
        this.SuspendLayout();
        //
        // menuStrip1
        //
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.fileToolStripMenuItem});
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Size = new System.Drawing.Size(1084, 24);
        this.menuStrip1.TabIndex = 0;
        this.menuStrip1.Text = "menuStrip1";
        //
        // fileToolStripMenuItem
        //
        this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.dashboardToolStripMenuItem,
        this.settingsToolStripMenuItem,
        this.exitToolStripMenuItem});
        this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
        this.fileToolStripMenuItem.Text = "Menu";
        //
        // dashboardToolStripMenuItem
        //
        this.dashboardToolStripMenuItem.Name = "dashboardToolStripMenuItem";
        this.dashboardToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
        this.dashboardToolStripMenuItem.Text = "Dashboard";
        //
        // settingsToolStripMenuItem
        //
        this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
        this.settingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
        this.settingsToolStripMenuItem.Text = "Settings";
        //
        // exitToolStripMenuItem
        //
        this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
        this.exitToolStripMenuItem.Text = "Exit";
        //
        // contentPanel
        //
        this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.contentPanel.Location = new System.Drawing.Point(0, 24);
        this.contentPanel.Name = "contentPanel";
        this.contentPanel.Size = new System.Drawing.Size(1084, 627);
        this.contentPanel.TabIndex = 1;
        //
        // MainForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1084, 651);
        this.Controls.Add(this.contentPanel);
        this.Controls.Add(this.menuStrip1);
        this.MainMenuStrip = this.menuStrip1;
        this.Name = "MainForm";
        this.Text = "VTC Splunk Log Analyzer";
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem dashboardToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.Panel contentPanel;

    #endregion
}
