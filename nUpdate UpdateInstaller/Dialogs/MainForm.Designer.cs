namespace nUpdate
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
            this.unpackingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // extractProgressBar
            // 
            this.extractProgressBar.Location = new System.Drawing.Point(14, 37);
            this.extractProgressBar.Name = "extractProgressBar";
            this.extractProgressBar.Size = new System.Drawing.Size(260, 14);
            this.extractProgressBar.TabIndex = 0;
            // 
            // unpackingLabel
            // 
            this.unpackingLabel.AutoSize = true;
            this.unpackingLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unpackingLabel.Location = new System.Drawing.Point(11, 11);
            this.unpackingLabel.Name = "unpackingLabel";
            this.unpackingLabel.Size = new System.Drawing.Size(63, 13);
            this.unpackingLabel.TabIndex = 1;
            this.unpackingLabel.Text = "Unpacking";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(294, 64);
            this.Controls.Add(this.unpackingLabel);
            this.Controls.Add(this.extractProgressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar extractProgressBar;
        private System.Windows.Forms.Label unpackingLabel;
    }
}

