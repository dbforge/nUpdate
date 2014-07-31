namespace nUpdate.Administration.Core.Update.Operations.Dialogs
{
    partial class RegistryEntryCreateOperationPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistryEntryCreateOperationPanel));
            this.label2 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pathTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.filesToDeleteListBox = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(124, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(337, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "If you want to delete an entry, select it and press \"Delete\"/\"Backspace\".";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(381, 35);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 23;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Cue = "Keyname";
            this.fileNameTextBox.Location = new System.Drawing.Point(108, 35);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(267, 22);
            this.fileNameTextBox.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 19);
            this.label1.TabIndex = 21;
            this.label1.Text = "Filename:";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Cue = "%appdata%/myapp";
            this.pathTextBox.Location = new System.Drawing.Point(251, 5);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(150, 22);
            this.pathTextBox.TabIndex = 20;
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(5, 8);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(48, 19);
            this.pathLabel.TabIndex = 19;
            this.pathLabel.Text = "Path:";
            // 
            // filesToDeleteListBox
            // 
            this.filesToDeleteListBox.FormattingEnabled = true;
            this.filesToDeleteListBox.Location = new System.Drawing.Point(108, 63);
            this.filesToDeleteListBox.Name = "filesToDeleteListBox";
            this.filesToDeleteListBox.Size = new System.Drawing.Size(267, 121);
            this.filesToDeleteListBox.TabIndex = 18;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(107, 190);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "HKEY_CLASSES_ROOT",
            "HKEY_CURRENT_USER",
            "HKEY_LOCAL_MACHINE"});
            this.comboBox1.Location = new System.Drawing.Point(108, 5);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "\\";
            // 
            // RegistryEntryCreateOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.fileNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.filesToDeleteListBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RegistryEntryCreateOperationPanel";
            this.Size = new System.Drawing.Size(488, 225);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button addButton;
        private WatermarkTextBox fileNameTextBox;
        private System.Windows.Forms.Label label1;
        private WatermarkTextBox pathTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ListBox filesToDeleteListBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
    }
}
