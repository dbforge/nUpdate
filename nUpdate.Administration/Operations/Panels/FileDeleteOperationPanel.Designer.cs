using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.Operations.Panels
{
    partial class FileDeleteOperationPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileDeleteOperationPanel));
            this.addButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.filesToDeleteListBox = new System.Windows.Forms.ListBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.pathTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.environmentVariablesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(379, 43);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 15;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 19);
            this.label1.TabIndex = 13;
            this.label1.Text = "Filename:";
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(4, 16);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(97, 13);
            this.pathLabel.TabIndex = 11;
            this.pathLabel.Text = "Path of the file(s):";
            // 
            // filesToDeleteListBox
            // 
            this.filesToDeleteListBox.FormattingEnabled = true;
            this.filesToDeleteListBox.Location = new System.Drawing.Point(106, 71);
            this.filesToDeleteListBox.Name = "filesToDeleteListBox";
            this.filesToDeleteListBox.Size = new System.Drawing.Size(267, 121);
            this.filesToDeleteListBox.TabIndex = 10;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(379, 71);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 18;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Cue = "obsolete.exe";
            this.fileNameTextBox.Location = new System.Drawing.Point(106, 43);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(267, 22);
            this.fileNameTextBox.TabIndex = 14;
            this.fileNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileNameTextBox_KeyDown);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Cue = "%appdata%\\myapp\\";
            this.pathTextBox.Location = new System.Drawing.Point(106, 14);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(267, 22);
            this.pathTextBox.TabIndex = 12;
            this.pathTextBox.TextChanged += new System.EventHandler(this.pathTextBox_TextChanged);
            // 
            // environmentVariablesButton
            // 
            this.environmentVariablesButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("environmentVariablesButton.BackgroundImage")));
            this.environmentVariablesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.environmentVariablesButton.Location = new System.Drawing.Point(379, 14);
            this.environmentVariablesButton.Name = "environmentVariablesButton";
            this.environmentVariablesButton.Size = new System.Drawing.Size(31, 23);
            this.environmentVariablesButton.TabIndex = 31;
            this.environmentVariablesButton.UseVisualStyleBackColor = true;
            this.environmentVariablesButton.Click += new System.EventHandler(this.environmentVariablesButton_Click);
            // 
            // FileDeleteOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.environmentVariablesButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.fileNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.filesToDeleteListBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FileDeleteOperationPanel";
            this.Size = new System.Drawing.Size(488, 203);
            this.Load += new System.EventHandler(this.FileDeleteOperationPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private CueTextBox fileNameTextBox;
        private System.Windows.Forms.Label label1;
        private CueTextBox pathTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ListBox filesToDeleteListBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button environmentVariablesButton;
    }
}
