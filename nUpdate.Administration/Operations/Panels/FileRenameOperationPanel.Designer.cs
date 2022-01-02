using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.Operations.Panels
{
    partial class FileRenameOperationPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileRenameOperationPanel));
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.newNameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.pathTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.environmentVariablesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 19);
            this.label1.TabIndex = 22;
            this.label1.Text = "New name of the file:";
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(4, 16);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(126, 19);
            this.pathLabel.TabIndex = 19;
            this.pathLabel.Text = "Path of the file:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(447, 30);
            this.label2.TabIndex = 25;
            this.label2.Text = "Please make sure that the new name you choose for the file is not already used by" +
    " any other \r\nfile inside the directory set., otherwise the client will get an er" +
    "ror because the renaming failed.\r\n";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 72);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // newNameTextBox
            // 
            this.newNameTextBox.Cue = "newname.exe";
            this.newNameTextBox.Location = new System.Drawing.Point(136, 41);
            this.newNameTextBox.Name = "newNameTextBox";
            this.newNameTextBox.Size = new System.Drawing.Size(202, 22);
            this.newNameTextBox.TabIndex = 23;
            // 
            // pathTextBox
            // 
            this.pathTextBox.Cue = "%appdata%\\myapp\\oldname.exe";
            this.pathTextBox.Location = new System.Drawing.Point(136, 13);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(202, 22);
            this.pathTextBox.TabIndex = 20;
            this.pathTextBox.TextChanged += new System.EventHandler(this.pathTextBox_TextChanged);
            // 
            // environmentVariablesButton
            // 
            this.environmentVariablesButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("environmentVariablesButton.BackgroundImage")));
            this.environmentVariablesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.environmentVariablesButton.Location = new System.Drawing.Point(344, 11);
            this.environmentVariablesButton.Name = "environmentVariablesButton";
            this.environmentVariablesButton.Size = new System.Drawing.Size(31, 23);
            this.environmentVariablesButton.TabIndex = 31;
            this.environmentVariablesButton.UseVisualStyleBackColor = true;
            this.environmentVariablesButton.Click += new System.EventHandler(this.environmentVariablesButton_Click);
            // 
            // FileRenameOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.environmentVariablesButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.newNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FileRenameOperationPanel";
            this.Size = new System.Drawing.Size(488, 225);
            this.Load += new System.EventHandler(this.FileRenameOperationPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CueTextBox newNameTextBox;
        private System.Windows.Forms.Label label1;
        private CueTextBox pathTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button environmentVariablesButton;
    }
}
