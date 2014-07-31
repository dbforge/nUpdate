namespace nUpdate.Administration.UI.Dialogs
{
    partial class ProjectEditDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectEditDialog));
            this.headerLabel = new System.Windows.Forms.Label();
            this.newNameLabel = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.newTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.updateUrlTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.headerLabel.Location = new System.Drawing.Point(12, 9);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(86, 20);
            this.headerLabel.TabIndex = 7;
            this.headerLabel.Text = "Edit project";
            // 
            // newNameLabel
            // 
            this.newNameLabel.AutoSize = true;
            this.newNameLabel.Location = new System.Drawing.Point(16, 51);
            this.newNameLabel.Name = "newNameLabel";
            this.newNameLabel.Size = new System.Drawing.Size(64, 13);
            this.newNameLabel.TabIndex = 6;
            this.newNameLabel.Text = "New name:";
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            "VB.NET",
            "C#"});
            this.languageComboBox.Location = new System.Drawing.Point(95, 79);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(164, 21);
            this.languageComboBox.TabIndex = 4;
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.Location = new System.Drawing.Point(16, 82);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(61, 13);
            this.languageLabel.TabIndex = 3;
            this.languageLabel.Text = "Language:";
            // 
            // newTextBox
            // 
            this.newTextBox.Cue = "The new name of the project";
            this.newTextBox.Location = new System.Drawing.Point(95, 48);
            this.newTextBox.Name = "newTextBox";
            this.newTextBox.Size = new System.Drawing.Size(191, 22);
            this.newTextBox.TabIndex = 28;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.saveButton);
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 147);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(359, 38);
            this.controlPanel1.TabIndex = 29;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(277, 7);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(198, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Update-URL:";
            // 
            // updateUrlTextBox
            // 
            this.updateUrlTextBox.Cue = "http://www.yourserver.com/newdirectory";
            this.updateUrlTextBox.Location = new System.Drawing.Point(95, 110);
            this.updateUrlTextBox.Name = "updateUrlTextBox";
            this.updateUrlTextBox.Size = new System.Drawing.Size(231, 22);
            this.updateUrlTextBox.TabIndex = 31;
            this.updateUrlTextBox.Enter += new System.EventHandler(this.updateUrlTextBox_Enter);
            // 
            // ProjectEditDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(360, 185);
            this.Controls.Add(this.updateUrlTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.newTextBox);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.newNameLabel);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.languageLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectEditDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit project - {0} - nUpdate Administration 1.1.0.0";
            this.Load += new System.EventHandler(this.ProjectProjectEditDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label newNameLabel;
        private System.Windows.Forms.Label headerLabel;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private WatermarkTextBox updateUrlTextBox;
        private WatermarkTextBox newTextBox;
    }
}