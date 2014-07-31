namespace nUpdate.Administration.Core.Update.Operations.Dialogs
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
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.addButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.filesToDeleteListBox = new System.Windows.Forms.ListBox();
            this.fileNameTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.pathTextBox = new nUpdate.Administration.WatermarkTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(122, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(337, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "If you want to delete an entry, select it and press \"Delete\"/\"Backspace\".";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(105, 197);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(379, 42);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 15;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 19);
            this.label1.TabIndex = 13;
            this.label1.Text = "Filename:";
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(3, 15);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(97, 13);
            this.pathLabel.TabIndex = 11;
            this.pathLabel.Text = "Path of the file(s):";
            // 
            // filesToDeleteListBox
            // 
            this.filesToDeleteListBox.FormattingEnabled = true;
            this.filesToDeleteListBox.Location = new System.Drawing.Point(106, 70);
            this.filesToDeleteListBox.Name = "filesToDeleteListBox";
            this.filesToDeleteListBox.Size = new System.Drawing.Size(267, 121);
            this.filesToDeleteListBox.TabIndex = 10;
            this.filesToDeleteListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filesToDeleteListBox_KeyDown);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Cue = "obsolete.exe";
            this.fileNameTextBox.Location = new System.Drawing.Point(106, 42);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(267, 22);
            this.fileNameTextBox.TabIndex = 14;
            this.fileNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileNameTextBox_KeyDown);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Cue = "%appdata%/myapp";
            this.pathTextBox.Location = new System.Drawing.Point(106, 12);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(267, 22);
            this.pathTextBox.TabIndex = 12;
            this.pathTextBox.Leave += new System.EventHandler(this.pathTextBox_Leave);
            // 
            // FileDeleteOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.fileNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.filesToDeleteListBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FileDeleteOperationPanel";
            this.Size = new System.Drawing.Size(488, 225);
            this.Load += new System.EventHandler(this.FileDeleteOperationPanel_Load);
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
    }
}
