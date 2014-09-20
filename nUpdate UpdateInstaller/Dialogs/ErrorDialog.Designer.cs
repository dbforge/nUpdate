namespace nUpdate.UpdateInstaller
{
    partial class ErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.errorMessageTextBox = new System.Windows.Forms.RichTextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.failedLabel = new System.Windows.Forms.Label();
            this.pict_icon = new System.Windows.Forms.PictureBox();
            this.controlPanel1 = new nUpdate.UpdateInstaller.Controls.ControlPanel();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pict_icon)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorMessageTextBox
            // 
            this.errorMessageTextBox.Location = new System.Drawing.Point(13, 72);
            this.errorMessageTextBox.Name = "errorMessageTextBox";
            this.errorMessageTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.errorMessageTextBox.Size = new System.Drawing.Size(376, 139);
            this.errorMessageTextBox.TabIndex = 12;
            this.errorMessageTextBox.Text = resources.GetString("errorMessageTextBox.Text");
            this.errorMessageTextBox.Enter += new System.EventHandler(this.errorMessageTextBox_Enter);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(67, 35);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(233, 13);
            this.infoLabel.TabIndex = 10;
            this.infoLabel.Text = "The arguments for the startup weren\'t valid.";
            // 
            // failedLabel
            // 
            this.failedLabel.AutoSize = true;
            this.failedLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.failedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.failedLabel.Location = new System.Drawing.Point(66, 11);
            this.failedLabel.Name = "failedLabel";
            this.failedLabel.Size = new System.Drawing.Size(282, 21);
            this.failedLabel.TabIndex = 9;
            this.failedLabel.Text = "Installing the update package has failed";
            // 
            // pict_icon
            // 
            this.pict_icon.Location = new System.Drawing.Point(20, 16);
            this.pict_icon.Name = "pict_icon";
            this.pict_icon.Size = new System.Drawing.Size(41, 41);
            this.pict_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pict_icon.TabIndex = 8;
            this.pict_icon.TabStop = false;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.closeButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 236);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(412, 36);
            this.controlPanel1.TabIndex = 13;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(315, 6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(88, 25);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(412, 273);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.errorMessageTextBox);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.failedLabel);
            this.Controls.Add(this.pict_icon);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "nUpdate UpdateInstaller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ErrorDialog_FormClosing);
            this.Load += new System.EventHandler(this.ErrorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pict_icon)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox errorMessageTextBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label failedLabel;
        private System.Windows.Forms.PictureBox pict_icon;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button closeButton;
    }
}