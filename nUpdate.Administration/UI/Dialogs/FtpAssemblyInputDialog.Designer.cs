using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class FtpAssemblyInputDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FtpAssemblyInputDialog));
            this.assemblyFilePathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bottomPanel1 = new BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.bottomPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // assemblyFilePathTextBox
            // 
            this.assemblyFilePathTextBox.ButtonText = "...";
            this.assemblyFilePathTextBox.Cue = "The path of the binary including the assembly...";
            this.assemblyFilePathTextBox.Location = new System.Drawing.Point(15, 36);
            this.assemblyFilePathTextBox.Name = "assemblyFilePathTextBox";
            this.assemblyFilePathTextBox.Size = new System.Drawing.Size(284, 22);
            this.assemblyFilePathTextBox.TabIndex = 0;
            this.assemblyFilePathTextBox.ButtonClicked += new System.EventHandler<System.EventArgs>(this.assemblyFilePathTextBox_ButtonClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Assembly file path:";
            // 
            // bottomPanel1
            // 
            this.bottomPanel1.Controls.Add(this.cancelButton);
            this.bottomPanel1.Controls.Add(this.continueButton);
            this.bottomPanel1.Location = new System.Drawing.Point(0, 77);
            this.bottomPanel1.Name = "bottomPanel1";
            this.bottomPanel1.Size = new System.Drawing.Size(311, 40);
            this.bottomPanel1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(229, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(151, 8);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // FtpAssemblyInputDialog
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(311, 117);
            this.Controls.Add(this.bottomPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.assemblyFilePathTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FtpAssemblyInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Specify an assembly... - {0}";
            this.Load += new System.EventHandler(this.FtpAssemblyInputDialog_Load);
            this.bottomPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ButtonTextBox assemblyFilePathTextBox;
        private System.Windows.Forms.Label label1;
        private BottomPanel bottomPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
    }
}