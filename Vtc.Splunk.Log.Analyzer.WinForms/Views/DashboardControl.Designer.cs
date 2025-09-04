namespace Vtc.Splunk.Log.Analyzer.WinForms.Views
{
    partial class DashboardControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnImport = new System.Windows.Forms.Button();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            //
            // btnImport
            //
            this.btnImport.Location = new System.Drawing.Point(20, 20);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(120, 23);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Import Search Codes";
            this.btnImport.UseVisualStyleBackColor = true;
            //
            // lblStartTime
            //
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(160, 25);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(58, 13);
            this.lblStartTime.TabIndex = 1;
            this.lblStartTime.Text = "Start Time:";
            //
            // dtpStartTime
            //
            this.dtpStartTime.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(225, 22);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(150, 20);
            this.dtpStartTime.TabIndex = 2;
            //
            // lblEndTime
            //
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(390, 25);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(55, 13);
            this.lblEndTime.TabIndex = 3;
            this.lblEndTime.Text = "End Time:";
            //
            // dtpEndTime
            //
            this.dtpEndTime.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndTime.Location = new System.Drawing.Point(450, 22);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(150, 20);
            this.dtpEndTime.TabIndex = 4;
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(620, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 23);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            //
            // dgvResults
            //
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(20, 60);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.Size = new System.Drawing.Size(760, 480);
            this.dgvResults.TabIndex = 6;
            //
            // btnExportExcel
            //
            this.btnExportExcel.Location = new System.Drawing.Point(20, 560);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(120, 23);
            this.btnExportExcel.TabIndex = 7;
            this.btnExportExcel.Text = "Export to Excel";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            //
            // btnExportCsv
            //
            this.btnExportCsv.Location = new System.Drawing.Point(150, 560);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(120, 23);
            this.btnExportCsv.TabIndex = 8;
            this.btnExportCsv.Text = "Export to CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(290, 565);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "Ready";
            //
            // DashboardControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.btnExportExcel);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.dtpEndTime);
            this.Controls.Add(this.lblEndTime);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.btnImport);
            this.Name = "DashboardControl";
            this.Size = new System.Drawing.Size(800, 600);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Label lblStatus;
    }
}