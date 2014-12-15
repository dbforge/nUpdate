namespace nUpdate.UI.Dialogs
{
    partial class UpdateErrorDialog
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
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.failedLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.errorCodeLabel = new System.Windows.Forms.Label();
            this.errorMessageTextBox = new System.Windows.Forms.RichTextBox();
            this.controlPanel1 = new nUpdate.UI.Controls.ControlPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.showStackTraceCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(19, 17);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(41, 41);
            this.iconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.iconPictureBox.TabIndex = 0;
            this.iconPictureBox.TabStop = false;
            // 
            // failedLabel
            // 
            this.failedLabel.AutoSize = true;
            this.failedLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.failedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.failedLabel.Location = new System.Drawing.Point(65, 12);
            this.failedLabel.Name = "failedLabel";
            this.failedLabel.Size = new System.Drawing.Size(253, 21);
            this.failedLabel.TabIndex = 1;
            this.failedLabel.Text = "Updating the application has failed.";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(66, 36);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(284, 13);
            this.infoLabel.TabIndex = 2;
            this.infoLabel.Text = "Die Updates konnten nicht heruntergeladen werden. ";
            // 
            // errorCodeLabel
            // 
            this.errorCodeLabel.AutoSize = true;
            this.errorCodeLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorCodeLabel.Location = new System.Drawing.Point(9, 74);
            this.errorCodeLabel.Name = "errorCodeLabel";
            this.errorCodeLabel.Size = new System.Drawing.Size(60, 13);
            this.errorCodeLabel.TabIndex = 3;
            this.errorCodeLabel.Text = "Errorcode:";
            // 
            // errorMessageTextBox
            // 
            this.errorMessageTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.errorMessageTextBox.Location = new System.Drawing.Point(12, 100);
            this.errorMessageTextBox.Name = "errorMessageTextBox";
            this.errorMessageTextBox.ReadOnly = true;
            this.errorMessageTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.errorMessageTextBox.Size = new System.Drawing.Size(376, 139);
            this.errorMessageTextBox.TabIndex = 7;
            this.errorMessageTextBox.Text = "";
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.closeButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 255);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(402, 37);
            this.controlPanel1.TabIndex = 8;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(318, 7);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // showStackTraceCheckBox
            // 
            this.showStackTraceCheckBox.AutoSize = true;
            this.showStackTraceCheckBox.Location = new System.Drawing.Point(282, 73);
            this.showStackTraceCheckBox.Name = "showStackTraceCheckBox";
            this.showStackTraceCheckBox.Size = new System.Drawing.Size(111, 17);
            this.showStackTraceCheckBox.TabIndex = 9;
            this.showStackTraceCheckBox.Text = "Show StackTrace";
            this.showStackTraceCheckBox.UseVisualStyleBackColor = true;
            this.showStackTraceCheckBox.CheckedChanged += new System.EventHandler(this.showStackTraceCheckBox_CheckedChanged);
            // 
            // UpdateErrorDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(402, 292);
            this.Controls.Add(this.showStackTraceCheckBox);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.errorMessageTextBox);
            this.Controls.Add(this.errorCodeLabel);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.failedLabel);
            this.Controls.Add(this.iconPictureBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateErrorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTitle";
            this.Load += new System.EventHandler(this.UpdateErrorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.Label failedLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label errorCodeLabel;
        private System.Windows.Forms.RichTextBox errorMessageTextBox;
        private UI.Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckBox showStackTraceCheckBox;
    }
}