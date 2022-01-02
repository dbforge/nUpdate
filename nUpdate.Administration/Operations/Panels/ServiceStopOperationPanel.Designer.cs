namespace nUpdate.Administration.Operations.Panels
{
    partial class ServiceStopOperationPanel
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceStopOperationPanel));
            this.serviceNameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.serviceNameLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // serviceNameTextBox
            // 
            this.serviceNameTextBox.Cue = "MyService";
            this.serviceNameTextBox.Location = new System.Drawing.Point(147, 13);
            this.serviceNameTextBox.Name = "serviceNameTextBox";
            this.serviceNameTextBox.Size = new System.Drawing.Size(193, 22);
            this.serviceNameTextBox.TabIndex = 41;
            // 
            // serviceNameLabel
            // 
            this.serviceNameLabel.Location = new System.Drawing.Point(4, 16);
            this.serviceNameLabel.Name = "serviceNameLabel";
            this.serviceNameLabel.Size = new System.Drawing.Size(135, 19);
            this.serviceNameLabel.TabIndex = 40;
            this.serviceNameLabel.Text = "Name of the service:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(399, 30);
            this.label2.TabIndex = 39;
            this.label2.Text = "Make sure not to set the service\'s display name but the real one.\r\nIf the service" +
    " is not running, this operation will be ignored.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 38);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 38;
            this.pictureBox1.TabStop = false;
            // 
            // ServiceStopOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.serviceNameTextBox);
            this.Controls.Add(this.serviceNameLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ServiceStopOperationPanel";
            this.Size = new System.Drawing.Size(488, 225);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.Controls.CueTextBox serviceNameTextBox;
        private System.Windows.Forms.Label serviceNameLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
