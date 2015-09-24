namespace nUpdate.Administration.UI.Dialogs
{
    partial class PreferencesDialog
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
                _manager.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.searchUpdatesButton = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.includeBetaCheckBox = new System.Windows.Forms.CheckBox();
            this.includeAlphaCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.controlPanel1 = new nUpdate.UI.Controls.BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.languagesComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.editLanguageButton = new System.Windows.Forms.Button();
            this.openLanguagesPathButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.programPathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label1.Name = "label1";
            // 
            // searchUpdatesButton
            // 
            resources.ApplyResources(this.searchUpdatesButton, "searchUpdatesButton");
            this.searchUpdatesButton.Name = "searchUpdatesButton";
            this.searchUpdatesButton.UseVisualStyleBackColor = true;
            this.searchUpdatesButton.Click += new System.EventHandler(this.searchUpdatesButton_Click);
            // 
            // versionLabel
            // 
            resources.ApplyResources(this.versionLabel, "versionLabel");
            this.versionLabel.Name = "versionLabel";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // includeBetaCheckBox
            // 
            resources.ApplyResources(this.includeBetaCheckBox, "includeBetaCheckBox");
            this.includeBetaCheckBox.Name = "includeBetaCheckBox";
            this.includeBetaCheckBox.UseVisualStyleBackColor = true;
            this.includeBetaCheckBox.CheckedChanged += new System.EventHandler(this.includeBetaCheckBox_CheckedChanged);
            // 
            // includeAlphaCheckBox
            // 
            resources.ApplyResources(this.includeAlphaCheckBox, "includeAlphaCheckBox");
            this.includeAlphaCheckBox.Name = "includeAlphaCheckBox";
            this.includeAlphaCheckBox.UseVisualStyleBackColor = true;
            this.includeAlphaCheckBox.CheckedChanged += new System.EventHandler(this.includeAlphaCheckBox_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.saveButton);
            resources.ApplyResources(this.controlPanel1, "controlPanel1");
            this.controlPanel1.Name = "controlPanel1";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // languagesComboBox
            // 
            this.languagesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languagesComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.languagesComboBox, "languagesComboBox");
            this.languagesComboBox.Name = "languagesComboBox";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // editLanguageButton
            // 
            resources.ApplyResources(this.editLanguageButton, "editLanguageButton");
            this.editLanguageButton.Name = "editLanguageButton";
            this.editLanguageButton.UseVisualStyleBackColor = true;
            this.editLanguageButton.Click += new System.EventHandler(this.editLanguageButton_Click);
            // 
            // openLanguagesPathButton
            // 
            resources.ApplyResources(this.openLanguagesPathButton, "openLanguagesPathButton");
            this.openLanguagesPathButton.Name = "openLanguagesPathButton";
            this.openLanguagesPathButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // programPathTextBox
            // 
            this.programPathTextBox.ButtonText = "...";
            resources.ApplyResources(this.programPathTextBox, "programPathTextBox");
            this.programPathTextBox.Name = "programPathTextBox";
            this.programPathTextBox.ButtonClicked += new System.EventHandler<System.EventArgs>(this.programPathTextBox_ButtonClicked);
            // 
            // PreferencesDialog
            // 
            this.AcceptButton = this.saveButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.programPathTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.openLanguagesPathButton);
            this.Controls.Add(this.editLanguageButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.languagesComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.includeBetaCheckBox);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.includeAlphaCheckBox);
            this.Controls.Add(this.searchUpdatesButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreferencesDialog";
            this.Load += new System.EventHandler(this.PreferencesDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button searchUpdatesButton;
        private nUpdate.UI.Controls.BottomPanel controlPanel1;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox includeBetaCheckBox;
        private System.Windows.Forms.CheckBox includeAlphaCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox languagesComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button editLanguageButton;
        private System.Windows.Forms.Button openLanguagesPathButton;
        private System.Windows.Forms.Label label5;
        private Controls.ButtonTextBox programPathTextBox;
    }
}