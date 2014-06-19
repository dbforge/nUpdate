namespace nUpdate.Administration.Core.Update.Operations
{
    partial class FileDeleteOperationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileDeleteOperationDialog));
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.fileTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.filesListBox = new System.Windows.Forms.ListBox();
            this.environmentLinkLabel = new System.Windows.Forms.LinkLabel();
            this.controlPanel1 = new nUpdate.UI.Controls.ControlPanel();
            this.addOperationButton = new System.Windows.Forms.Button();
            this.directoryTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(300, 85);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(76, 22);
            this.removeButton.TabIndex = 14;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(214, 85);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(80, 22);
            this.addButton.TabIndex = 13;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // fileTextBox
            // 
            this.fileTextBox.Cue = "file.txt";
            this.fileTextBox.Location = new System.Drawing.Point(15, 85);
            this.fileTextBox.Name = "fileTextBox";
            this.fileTextBox.Size = new System.Drawing.Size(193, 22);
            this.fileTextBox.TabIndex = 12;
            // 
            // filesListBox
            // 
            this.filesListBox.FormattingEnabled = true;
            this.filesListBox.Location = new System.Drawing.Point(15, 113);
            this.filesListBox.Name = "filesListBox";
            this.filesListBox.Size = new System.Drawing.Size(361, 95);
            this.filesListBox.TabIndex = 11;
            // 
            // environmentLinkLabel
            // 
            this.environmentLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.environmentLinkLabel.AutoSize = true;
            this.environmentLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.environmentLinkLabel.Location = new System.Drawing.Point(256, 49);
            this.environmentLinkLabel.Name = "environmentLinkLabel";
            this.environmentLinkLabel.Size = new System.Drawing.Size(120, 13);
            this.environmentLinkLabel.TabIndex = 10;
            this.environmentLinkLabel.TabStop = true;
            this.environmentLinkLabel.Text = "Environment variables";
            this.environmentLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.environmentLinkLabel_LinkClicked);
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.addOperationButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 227);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(391, 40);
            this.controlPanel1.TabIndex = 2;
            // 
            // addOperationButton
            // 
            this.addOperationButton.Location = new System.Drawing.Point(259, 8);
            this.addOperationButton.Name = "addOperationButton";
            this.addOperationButton.Size = new System.Drawing.Size(121, 23);
            this.addOperationButton.TabIndex = 3;
            this.addOperationButton.Text = "Add operation";
            this.addOperationButton.UseVisualStyleBackColor = true;
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Cue = "%appdata%/folder";
            this.directoryTextBox.Location = new System.Drawing.Point(133, 18);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(243, 22);
            this.directoryTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path of the directory:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(178, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 15;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // FileDeleteOperationDialog
            // 
            this.AcceptButton = this.addOperationButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(391, 267);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.fileTextBox);
            this.Controls.Add(this.filesListBox);
            this.Controls.Add(this.environmentLinkLabel);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileDeleteOperationDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add a file-operation - nUpdate Administration 1.1.0.0";
            this.Load += new System.EventHandler(this.FileDeleteOperationDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private WatermarkTextBox directoryTextBox;
        private nUpdate.UI.Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button addOperationButton;
        private System.Windows.Forms.LinkLabel environmentLinkLabel;
        private System.Windows.Forms.ListBox filesListBox;
        private WatermarkTextBox fileTextBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button cancelButton;
    }
}