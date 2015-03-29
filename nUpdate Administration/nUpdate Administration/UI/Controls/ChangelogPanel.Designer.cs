namespace nUpdate.Administration.UI.Controls
{
    partial class ChangelogPanel
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
            this.changelogTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // changelogTextBox
            // 
            this.changelogTextBox.AcceptsReturn = true;
            this.changelogTextBox.Location = new System.Drawing.Point(0, 1);
            this.changelogTextBox.Multiline = true;
            this.changelogTextBox.Name = "changelogTextBox";
            this.changelogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.changelogTextBox.Size = new System.Drawing.Size(453, 190);
            this.changelogTextBox.TabIndex = 1;
            this.changelogTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.changelogTextBox_KeyDown);
            // 
            // ChangelogPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.changelogTextBox);
            this.Name = "ChangelogPanel";
            this.Size = new System.Drawing.Size(453, 194);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox changelogTextBox;
    }
}
