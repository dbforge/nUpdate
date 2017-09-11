using nUpdate.Internal.UI.Controls;

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
            this.label3 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.controlPanel1 = new BottomPanel();
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            resources.ApplyResources(this.languagesComboBox, "languagesComboBox");
            this.languagesComboBox.FormattingEnabled = true;
            this.languagesComboBox.Name = "languagesComboBox";
            this.languagesComboBox.SelectedIndexChanged += new System.EventHandler(this.languagesComboBox_SelectedIndexChanged);
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
            this.Controls.Add(this.label3);
            this.Controls.Add(this.controlPanel1);
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
        private BottomPanel controlPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox languagesComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button editLanguageButton;
        private System.Windows.Forms.Button openLanguagesPathButton;
        private System.Windows.Forms.Label label5;
        private Controls.ButtonTextBox programPathTextBox;
    }
}