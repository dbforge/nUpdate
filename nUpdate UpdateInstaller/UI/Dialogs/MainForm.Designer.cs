namespace nUpdate.UpdateInstaller.UI.Dialogs
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.extractProgressBar = new System.Windows.Forms.ProgressBar();
            this.updateLabel = new System.Windows.Forms.Label();
            this.percentageLabel = new System.Windows.Forms.Label();
            this.controlPanel1 = new nUpdate.UpdateInstaller.UI.Controls.ControlPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // extractProgressBar
            // 
            this.extractProgressBar.Location = new System.Drawing.Point(12, 34);
            this.extractProgressBar.Name = "extractProgressBar";
            this.extractProgressBar.Size = new System.Drawing.Size(302, 16);
            this.extractProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.extractProgressBar.TabIndex = 0;
            // 
            // updateLabel
            // 
            this.updateLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateLabel.Location = new System.Drawing.Point(9, 13);
            this.updateLabel.Name = "updateLabel";
            this.updateLabel.Size = new System.Drawing.Size(270, 13);
            this.updateLabel.TabIndex = 1;
            this.updateLabel.Text = "Extracting files...";
            // 
            // percentageLabel
            // 
            this.percentageLabel.AutoSize = true;
            this.percentageLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentageLabel.Location = new System.Drawing.Point(285, 13);
            this.percentageLabel.Name = "percentageLabel";
            this.percentageLabel.Size = new System.Drawing.Size(22, 13);
            this.percentageLabel.TabIndex = 2;
            this.percentageLabel.Text = "0%";
            this.percentageLabel.Visible = false;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 64);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(326, 40);
            this.controlPanel1.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(241, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(326, 104);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.percentageLabel);
            this.Controls.Add(this.updateLabel);
            this.Controls.Add(this.extractProgressBar);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar extractProgressBar;
        private System.Windows.Forms.Label updateLabel;
        private System.Windows.Forms.Label percentageLabel;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
    }
}

