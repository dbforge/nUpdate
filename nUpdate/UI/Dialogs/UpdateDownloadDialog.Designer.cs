namespace nUpdate.Dialogs
{
    partial class UpdateDownloadDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDownloadDialog));
            this.headerLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.hookPictureBox = new System.Windows.Forms.PictureBox();
            this.percentLabel = new System.Windows.Forms.Label();
            this.controlPanel1 = new nUpdate.UI.Controls.ControlPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.furtherButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hookPictureBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.Location = new System.Drawing.Point(62, 12);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(251, 21);
            this.headerLabel.TabIndex = 12;
            this.headerLabel.Text = "Aktualisierungen werden geladen...";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(62, 36);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(213, 13);
            this.infoLabel.TabIndex = 11;
            this.infoLabel.Text = "Bitte haben Sie einen Moment Geduld...";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(13, 104);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(16, 13);
            this.statusLabel.TabIndex = 9;
            this.statusLabel.Text = "...";
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(18, 17);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(41, 39);
            this.iconPictureBox.TabIndex = 10;
            this.iconPictureBox.TabStop = false;
            // 
            // downloadProgressBar
            // 
            this.downloadProgressBar.Location = new System.Drawing.Point(14, 72);
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.Size = new System.Drawing.Size(293, 23);
            this.downloadProgressBar.TabIndex = 13;
            // 
            // hookPictureBox
            // 
            this.hookPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("hookPictureBox.Image")));
            this.hookPictureBox.Location = new System.Drawing.Point(14, 102);
            this.hookPictureBox.Name = "hookPictureBox";
            this.hookPictureBox.Size = new System.Drawing.Size(16, 16);
            this.hookPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.hookPictureBox.TabIndex = 15;
            this.hookPictureBox.TabStop = false;
            // 
            // percentLabel
            // 
            this.percentLabel.AutoSize = true;
            this.percentLabel.Location = new System.Drawing.Point(276, 105);
            this.percentLabel.Name = "percentLabel";
            this.percentLabel.Size = new System.Drawing.Size(22, 13);
            this.percentLabel.TabIndex = 18;
            this.percentLabel.Text = "0%";
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.furtherButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 138);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(320, 37);
            this.controlPanel1.TabIndex = 19;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(157, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 21;
            this.cancelButton.Text = "Abbrechen";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // furtherButton
            // 
            this.furtherButton.Location = new System.Drawing.Point(237, 7);
            this.furtherButton.Name = "furtherButton";
            this.furtherButton.Size = new System.Drawing.Size(75, 23);
            this.furtherButton.TabIndex = 20;
            this.furtherButton.Text = "Weiter";
            this.furtherButton.UseVisualStyleBackColor = true;
            this.furtherButton.Click += new System.EventHandler(this.furtherButton_Click);
            // 
            // UpdateDownloadDialog
            // 
            this.AcceptButton = this.furtherButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(320, 175);
            this.ControlBox = false;
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.percentLabel);
            this.Controls.Add(this.hookPictureBox);
            this.Controls.Add(this.downloadProgressBar);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.iconPictureBox);
            this.Controls.Add(this.statusLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateDownloadDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTitle";
            this.Load += new System.EventHandler(this.UpdateDownloadDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hookPictureBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar downloadProgressBar;
        private System.Windows.Forms.PictureBox hookPictureBox;
        private System.Windows.Forms.Label percentLabel;
        private UI.Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button furtherButton;
    }
}