namespace Vtc.Splunk.Log.Analyzer.WinForms.Views
{
    partial class SettingsControl
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
            this.lblSplunkUrl = new System.Windows.Forms.Label();
            this.txtSplunkUrl = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblSplunkUrl
            //
            this.lblSplunkUrl.AutoSize = true;
            this.lblSplunkUrl.Location = new System.Drawing.Point(30, 30);
            this.lblSplunkUrl.Name = "lblSplunkUrl";
            this.lblSplunkUrl.Size = new System.Drawing.Size(66, 13);
            this.lblSplunkUrl.TabIndex = 0;
            this.lblSplunkUrl.Text = "Splunk URL:";
            //
            // txtSplunkUrl
            //
            this.txtSplunkUrl.Location = new System.Drawing.Point(120, 27);
            this.txtSplunkUrl.Name = "txtSplunkUrl";
            this.txtSplunkUrl.Size = new System.Drawing.Size(400, 20);
            this.txtSplunkUrl.TabIndex = 1;
            //
            // lblUsername
            //
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(30, 70);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username:";
            //
            // txtUsername
            //
            this.txtUsername.Location = new System.Drawing.Point(120, 67);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 20);
            this.txtUsername.TabIndex = 3;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(30, 110);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            //
            // txtPassword
            //
            this.txtPassword.Location = new System.Drawing.Point(120, 107);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 5;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(120, 150);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save Settings";
            this.btnSave.UseVisualStyleBackColor = true;
            //
            // SettingsControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtSplunkUrl);
            this.Controls.Add(this.lblSplunkUrl);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblSplunkUrl;
        private System.Windows.Forms.TextBox txtSplunkUrl;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSave;
    }
}