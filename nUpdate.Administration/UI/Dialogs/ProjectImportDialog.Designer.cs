using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class ProjectImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectImportDialog));
            this.controlPanel1 = new BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.headerLabel = new System.Windows.Forms.Label();
            this.importProjectRadioButton = new System.Windows.Forms.RadioButton();
            this.shareProjectRadioButton = new System.Windows.Forms.RadioButton();
            this.wizardTabControl = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.optionTabPage = new System.Windows.Forms.TabPage();
            this.importTabPage = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.projectToImportTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.importTabPage1 = new System.Windows.Forms.TabPage();
            this.projectNameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.projectFilePathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.shareTabPage = new System.Windows.Forms.TabPage();
            this.projectsListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.shareTabPage1 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.projectOutputPathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.controlPanel1.SuspendLayout();
            this.wizardTabControl.SuspendLayout();
            this.optionTabPage.SuspendLayout();
            this.importTabPage.SuspendLayout();
            this.importTabPage1.SuspendLayout();
            this.shareTabPage.SuspendLayout();
            this.shareTabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.backButton);
            this.controlPanel1.Controls.Add(this.continueButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 139);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(359, 40);
            this.controlPanel1.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cancelButton.Location = new System.Drawing.Point(269, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // backButton
            // 
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(103, 8);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 1;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(181, 8);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.headerLabel.Location = new System.Drawing.Point(8, 12);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(179, 20);
            this.headerLabel.TabIndex = 3;
            this.headerLabel.Text = "Please choose an option...";
            // 
            // importProjectRadioButton
            // 
            this.importProjectRadioButton.AutoSize = true;
            this.importProjectRadioButton.Checked = true;
            this.importProjectRadioButton.Location = new System.Drawing.Point(25, 47);
            this.importProjectRadioButton.Name = "importProjectRadioButton";
            this.importProjectRadioButton.Size = new System.Drawing.Size(258, 17);
            this.importProjectRadioButton.TabIndex = 4;
            this.importProjectRadioButton.TabStop = true;
            this.importProjectRadioButton.Text = "Import a project into nUpdate Administration";
            this.importProjectRadioButton.UseVisualStyleBackColor = true;
            // 
            // shareProjectRadioButton
            // 
            this.shareProjectRadioButton.AutoSize = true;
            this.shareProjectRadioButton.Location = new System.Drawing.Point(25, 70);
            this.shareProjectRadioButton.Name = "shareProjectRadioButton";
            this.shareProjectRadioButton.Size = new System.Drawing.Size(139, 17);
            this.shareProjectRadioButton.TabIndex = 5;
            this.shareProjectRadioButton.Text = "Share/Export a project";
            this.shareProjectRadioButton.UseVisualStyleBackColor = true;
            // 
            // wizardTabControl
            // 
            this.wizardTabControl.Controls.Add(this.optionTabPage);
            this.wizardTabControl.Controls.Add(this.importTabPage);
            this.wizardTabControl.Controls.Add(this.importTabPage1);
            this.wizardTabControl.Controls.Add(this.shareTabPage);
            this.wizardTabControl.Controls.Add(this.shareTabPage1);
            this.wizardTabControl.Location = new System.Drawing.Point(0, 0);
            this.wizardTabControl.Name = "wizardTabControl";
            this.wizardTabControl.SelectedIndex = 0;
            this.wizardTabControl.Size = new System.Drawing.Size(359, 151);
            this.wizardTabControl.TabIndex = 6;
            // 
            // optionTabPage
            // 
            this.optionTabPage.Controls.Add(this.shareProjectRadioButton);
            this.optionTabPage.Controls.Add(this.importProjectRadioButton);
            this.optionTabPage.Controls.Add(this.headerLabel);
            this.optionTabPage.Location = new System.Drawing.Point(4, 22);
            this.optionTabPage.Name = "optionTabPage";
            this.optionTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.optionTabPage.Size = new System.Drawing.Size(351, 125);
            this.optionTabPage.TabIndex = 0;
            this.optionTabPage.Text = "Option";
            this.optionTabPage.UseVisualStyleBackColor = true;
            // 
            // importTabPage
            // 
            this.importTabPage.Controls.Add(this.label6);
            this.importTabPage.Controls.Add(this.label7);
            this.importTabPage.Controls.Add(this.projectToImportTextBox);
            this.importTabPage.Location = new System.Drawing.Point(4, 22);
            this.importTabPage.Name = "importTabPage";
            this.importTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.importTabPage.Size = new System.Drawing.Size(351, 125);
            this.importTabPage.TabIndex = 1;
            this.importTabPage.Text = "Import";
            this.importTabPage.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(8, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(222, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Please choose a shared project...";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Path:";
            // 
            // projectToImportTextBox
            // 
            this.projectToImportTextBox.ButtonText = "...";
            this.projectToImportTextBox.Cue = "The path of the shared project to import";
            this.projectToImportTextBox.Location = new System.Drawing.Point(63, 48);
            this.projectToImportTextBox.Name = "projectToImportTextBox";
            this.projectToImportTextBox.Size = new System.Drawing.Size(264, 22);
            this.projectToImportTextBox.TabIndex = 8;
            this.projectToImportTextBox.ButtonClicked += new System.EventHandler<System.EventArgs>(this.projectToImportTextBox_ButtonClicked);
            // 
            // importTabPage1
            // 
            this.importTabPage1.Controls.Add(this.projectNameTextBox);
            this.importTabPage1.Controls.Add(this.nameLabel);
            this.importTabPage1.Controls.Add(this.label1);
            this.importTabPage1.Controls.Add(this.pathLabel);
            this.importTabPage1.Controls.Add(this.projectFilePathTextBox);
            this.importTabPage1.Location = new System.Drawing.Point(4, 22);
            this.importTabPage1.Name = "importTabPage1";
            this.importTabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.importTabPage1.Size = new System.Drawing.Size(351, 125);
            this.importTabPage1.TabIndex = 4;
            this.importTabPage1.Text = "Import";
            this.importTabPage1.UseVisualStyleBackColor = true;
            // 
            // projectNameTextBox
            // 
            this.projectNameTextBox.Cue = "The name of the project";
            this.projectNameTextBox.Location = new System.Drawing.Point(63, 47);
            this.projectNameTextBox.Name = "projectNameTextBox";
            this.projectNameTextBox.Size = new System.Drawing.Size(264, 22);
            this.projectNameTextBox.TabIndex = 12;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(18, 50);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 13);
            this.nameLabel.TabIndex = 11;
            this.nameLabel.Text = "Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Please choose a path...";
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(18, 78);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(33, 13);
            this.pathLabel.TabIndex = 9;
            this.pathLabel.Text = "Path:";
            // 
            // projectFilePathTextBox
            // 
            this.projectFilePathTextBox.ButtonText = "...";
            this.projectFilePathTextBox.Cue = "The path for saving the project file (.nupdproj)";
            this.projectFilePathTextBox.Location = new System.Drawing.Point(63, 75);
            this.projectFilePathTextBox.Name = "projectFilePathTextBox";
            this.projectFilePathTextBox.Size = new System.Drawing.Size(264, 22);
            this.projectFilePathTextBox.TabIndex = 8;
            this.projectFilePathTextBox.ButtonClicked += new System.EventHandler<System.EventArgs>(this.projectFilePathTextBox_ButtonClicked);
            // 
            // shareTabPage
            // 
            this.shareTabPage.Controls.Add(this.projectsListBox);
            this.shareTabPage.Controls.Add(this.label2);
            this.shareTabPage.Location = new System.Drawing.Point(4, 22);
            this.shareTabPage.Name = "shareTabPage";
            this.shareTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.shareTabPage.Size = new System.Drawing.Size(351, 125);
            this.shareTabPage.TabIndex = 2;
            this.shareTabPage.Text = "Share";
            this.shareTabPage.UseVisualStyleBackColor = true;
            // 
            // projectsListBox
            // 
            this.projectsListBox.FormattingEnabled = true;
            this.projectsListBox.Location = new System.Drawing.Point(12, 46);
            this.projectsListBox.Name = "projectsListBox";
            this.projectsListBox.Size = new System.Drawing.Size(331, 82);
            this.projectsListBox.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(8, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Please choose a project to export...";
            // 
            // shareTabPage1
            // 
            this.shareTabPage1.Controls.Add(this.label3);
            this.shareTabPage1.Controls.Add(this.label4);
            this.shareTabPage1.Controls.Add(this.projectOutputPathTextBox);
            this.shareTabPage1.Location = new System.Drawing.Point(4, 22);
            this.shareTabPage1.Name = "shareTabPage1";
            this.shareTabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.shareTabPage1.Size = new System.Drawing.Size(351, 125);
            this.shareTabPage1.TabIndex = 3;
            this.shareTabPage1.Text = "Share";
            this.shareTabPage1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(8, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Please choose a path...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Path:";
            // 
            // projectOutputPathTextBox
            // 
            this.projectOutputPathTextBox.ButtonText = "...";
            this.projectOutputPathTextBox.Cue = "The path for saving the project";
            this.projectOutputPathTextBox.Location = new System.Drawing.Point(62, 48);
            this.projectOutputPathTextBox.Name = "projectOutputPathTextBox";
            this.projectOutputPathTextBox.Size = new System.Drawing.Size(248, 22);
            this.projectOutputPathTextBox.TabIndex = 5;
            this.projectOutputPathTextBox.ButtonClicked += new System.EventHandler<System.EventArgs>(this.projectOutputPathTextBox_ButtonClicked);
            // 
            // ProjectImportDialog
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(359, 179);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.wizardTabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProjectImportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Share/Import a project - {0}";
            this.Load += new System.EventHandler(this.ProjectImportDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.wizardTabControl.ResumeLayout(false);
            this.optionTabPage.ResumeLayout(false);
            this.optionTabPage.PerformLayout();
            this.importTabPage.ResumeLayout(false);
            this.importTabPage.PerformLayout();
            this.importTabPage1.ResumeLayout(false);
            this.importTabPage1.PerformLayout();
            this.shareTabPage.ResumeLayout(false);
            this.shareTabPage.PerformLayout();
            this.shareTabPage1.ResumeLayout(false);
            this.shareTabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.RadioButton importProjectRadioButton;
        private System.Windows.Forms.RadioButton shareProjectRadioButton;
        private Controls.TablessTabControl wizardTabControl;
        private System.Windows.Forms.TabPage optionTabPage;
        private System.Windows.Forms.TabPage shareTabPage;
        private System.Windows.Forms.ListBox projectsListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage shareTabPage1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Controls.ButtonTextBox projectOutputPathTextBox;
        private System.Windows.Forms.TabPage importTabPage1;
        private Controls.CueTextBox projectNameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label pathLabel;
        private Controls.ButtonTextBox projectFilePathTextBox;
        private System.Windows.Forms.TabPage importTabPage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private Controls.ButtonTextBox projectToImportTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}