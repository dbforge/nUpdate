namespace nUpdate.Administration.UI.Dialogs
{
    partial class NewProjectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectForm));
            this.continueButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ftpPanel = new System.Windows.Forms.Panel();
            this.searchOnServerButton = new System.Windows.Forms.Button();
            this.line1 = new nUpdate.Administration.UI.Controls.Line();
            this.protocolComboBox = new System.Windows.Forms.ComboBox();
            this.protocolLabel = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.modeLabel = new System.Windows.Forms.Label();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.userTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.adressTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.userLabel = new System.Windows.Forms.Label();
            this.adressLabel = new System.Windows.Forms.Label();
            this.ftpHeaderLabel = new System.Windows.Forms.Label();
            this.generalPanel = new System.Windows.Forms.Panel();
            this.localPathLabel = new System.Windows.Forms.Label();
            this.updateUrlTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.updateUrlLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.generalHeaderLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.localPathTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.searchPathButton = new System.Windows.Forms.Button();
            this.keyPairPanel = new System.Windows.Forms.Panel();
            this.keyPairGenerationLabel = new System.Windows.Forms.Label();
            this.keyPairHeaderLabel = new System.Windows.Forms.Label();
            this.keyPairInfoLabel = new System.Windows.Forms.Label();
            this.keyPairLoadingPictureBox = new System.Windows.Forms.PictureBox();
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.backButton = new System.Windows.Forms.Button();
            this.ftpPanel.SuspendLayout();
            this.generalPanel.SuspendLayout();
            this.keyPairPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyPairLoadingPictureBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // continueButton
            // 
            resources.ApplyResources(this.continueButton, "continueButton");
            this.continueButton.Name = "continueButton";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ftpPanel
            // 
            this.ftpPanel.BackColor = System.Drawing.SystemColors.Window;
            this.ftpPanel.Controls.Add(this.searchOnServerButton);
            this.ftpPanel.Controls.Add(this.line1);
            this.ftpPanel.Controls.Add(this.protocolComboBox);
            this.ftpPanel.Controls.Add(this.protocolLabel);
            this.ftpPanel.Controls.Add(this.modeComboBox);
            this.ftpPanel.Controls.Add(this.modeLabel);
            this.ftpPanel.Controls.Add(this.directoryLabel);
            this.ftpPanel.Controls.Add(this.directoryTextBox);
            this.ftpPanel.Controls.Add(this.portTextBox);
            this.ftpPanel.Controls.Add(this.userTextBox);
            this.ftpPanel.Controls.Add(this.adressTextBox);
            this.ftpPanel.Controls.Add(this.passwordTextBox);
            this.ftpPanel.Controls.Add(this.portLabel);
            this.ftpPanel.Controls.Add(this.passwordLabel);
            this.ftpPanel.Controls.Add(this.userLabel);
            this.ftpPanel.Controls.Add(this.adressLabel);
            this.ftpPanel.Controls.Add(this.ftpHeaderLabel);
            resources.ApplyResources(this.ftpPanel, "ftpPanel");
            this.ftpPanel.Name = "ftpPanel";
            // 
            // searchOnServerButton
            // 
            resources.ApplyResources(this.searchOnServerButton, "searchOnServerButton");
            this.searchOnServerButton.Name = "searchOnServerButton";
            this.searchOnServerButton.UseVisualStyleBackColor = true;
            this.searchOnServerButton.Click += new System.EventHandler(this.searchOnServerButton_Click);
            // 
            // line1
            // 
            this.line1.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            resources.ApplyResources(this.line1, "line1");
            this.line1.Name = "line1";
            // 
            // protocolComboBox
            // 
            this.protocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.protocolComboBox.FormattingEnabled = true;
            this.protocolComboBox.Items.AddRange(new object[] {
            resources.GetString("protocolComboBox.Items"),
            resources.GetString("protocolComboBox.Items1")});
            resources.ApplyResources(this.protocolComboBox, "protocolComboBox");
            this.protocolComboBox.Name = "protocolComboBox";
            // 
            // protocolLabel
            // 
            resources.ApplyResources(this.protocolLabel, "protocolLabel");
            this.protocolLabel.Name = "protocolLabel";
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            resources.GetString("modeComboBox.Items"),
            resources.GetString("modeComboBox.Items1")});
            resources.ApplyResources(this.modeComboBox, "modeComboBox");
            this.modeComboBox.Name = "modeComboBox";
            // 
            // modeLabel
            // 
            resources.ApplyResources(this.modeLabel, "modeLabel");
            this.modeLabel.Name = "modeLabel";
            // 
            // directoryLabel
            // 
            resources.ApplyResources(this.directoryLabel, "directoryLabel");
            this.directoryLabel.Name = "directoryLabel";
            // 
            // directoryTextBox
            // 
            resources.ApplyResources(this.directoryTextBox, "directoryTextBox");
            this.directoryTextBox.Name = "directoryTextBox";
            // 
            // portTextBox
            // 
            resources.ApplyResources(this.portTextBox, "portTextBox");
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.portTextBox_KeyPress);
            // 
            // userTextBox
            // 
            resources.ApplyResources(this.userTextBox, "userTextBox");
            this.userTextBox.Name = "userTextBox";
            // 
            // adressTextBox
            // 
            resources.ApplyResources(this.adressTextBox, "adressTextBox");
            this.adressTextBox.Name = "adressTextBox";
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // portLabel
            // 
            resources.ApplyResources(this.portLabel, "portLabel");
            this.portLabel.Name = "portLabel";
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.Name = "passwordLabel";
            // 
            // userLabel
            // 
            resources.ApplyResources(this.userLabel, "userLabel");
            this.userLabel.Name = "userLabel";
            // 
            // adressLabel
            // 
            resources.ApplyResources(this.adressLabel, "adressLabel");
            this.adressLabel.Name = "adressLabel";
            // 
            // ftpHeaderLabel
            // 
            resources.ApplyResources(this.ftpHeaderLabel, "ftpHeaderLabel");
            this.ftpHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.ftpHeaderLabel.Name = "ftpHeaderLabel";
            // 
            // generalPanel
            // 
            this.generalPanel.Controls.Add(this.localPathLabel);
            this.generalPanel.Controls.Add(this.updateUrlTextBox);
            this.generalPanel.Controls.Add(this.updateUrlLabel);
            this.generalPanel.Controls.Add(this.nameTextBox);
            this.generalPanel.Controls.Add(this.languageComboBox);
            this.generalPanel.Controls.Add(this.languageLabel);
            this.generalPanel.Controls.Add(this.generalHeaderLabel);
            this.generalPanel.Controls.Add(this.nameLabel);
            this.generalPanel.Controls.Add(this.localPathTextBox);
            this.generalPanel.Controls.Add(this.searchPathButton);
            resources.ApplyResources(this.generalPanel, "generalPanel");
            this.generalPanel.Name = "generalPanel";
            // 
            // localPathLabel
            // 
            resources.ApplyResources(this.localPathLabel, "localPathLabel");
            this.localPathLabel.Name = "localPathLabel";
            // 
            // updateUrlTextBox
            // 
            resources.ApplyResources(this.updateUrlTextBox, "updateUrlTextBox");
            this.updateUrlTextBox.Name = "updateUrlTextBox";
            // 
            // updateUrlLabel
            // 
            resources.ApplyResources(this.updateUrlLabel, "updateUrlLabel");
            this.updateUrlLabel.Name = "updateUrlLabel";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            resources.GetString("languageComboBox.Items"),
            resources.GetString("languageComboBox.Items1")});
            resources.ApplyResources(this.languageComboBox, "languageComboBox");
            this.languageComboBox.Name = "languageComboBox";
            // 
            // languageLabel
            // 
            resources.ApplyResources(this.languageLabel, "languageLabel");
            this.languageLabel.Name = "languageLabel";
            // 
            // generalHeaderLabel
            // 
            resources.ApplyResources(this.generalHeaderLabel, "generalHeaderLabel");
            this.generalHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.generalHeaderLabel.Name = "generalHeaderLabel";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // localPathTextBox
            // 
            resources.ApplyResources(this.localPathTextBox, "localPathTextBox");
            this.localPathTextBox.Name = "localPathTextBox";
            // 
            // searchPathButton
            // 
            resources.ApplyResources(this.searchPathButton, "searchPathButton");
            this.searchPathButton.Name = "searchPathButton";
            this.searchPathButton.UseVisualStyleBackColor = true;
            this.searchPathButton.Click += new System.EventHandler(this.searchPathButton_Click);
            // 
            // keyPairPanel
            // 
            this.keyPairPanel.Controls.Add(this.generalPanel);
            this.keyPairPanel.Controls.Add(this.keyPairGenerationLabel);
            this.keyPairPanel.Controls.Add(this.keyPairHeaderLabel);
            this.keyPairPanel.Controls.Add(this.keyPairInfoLabel);
            this.keyPairPanel.Controls.Add(this.keyPairLoadingPictureBox);
            resources.ApplyResources(this.keyPairPanel, "keyPairPanel");
            this.keyPairPanel.Name = "keyPairPanel";
            // 
            // keyPairGenerationLabel
            // 
            resources.ApplyResources(this.keyPairGenerationLabel, "keyPairGenerationLabel");
            this.keyPairGenerationLabel.Name = "keyPairGenerationLabel";
            // 
            // keyPairHeaderLabel
            // 
            resources.ApplyResources(this.keyPairHeaderLabel, "keyPairHeaderLabel");
            this.keyPairHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.keyPairHeaderLabel.Name = "keyPairHeaderLabel";
            // 
            // keyPairInfoLabel
            // 
            resources.ApplyResources(this.keyPairInfoLabel, "keyPairInfoLabel");
            this.keyPairInfoLabel.Name = "keyPairInfoLabel";
            // 
            // keyPairLoadingPictureBox
            // 
            resources.ApplyResources(this.keyPairLoadingPictureBox, "keyPairLoadingPictureBox");
            this.keyPairLoadingPictureBox.Name = "keyPairLoadingPictureBox";
            this.keyPairLoadingPictureBox.TabStop = false;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.backButton);
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.continueButton);
            resources.ApplyResources(this.controlPanel1, "controlPanel1");
            this.controlPanel1.Name = "controlPanel1";
            // 
            // backButton
            // 
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.Name = "backButton";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // NewProjectForm
            // 
            this.AcceptButton = this.continueButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.ftpPanel);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.keyPairPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewProjectForm_FormClosing);
            this.Load += new System.EventHandler(this.NewProjectForm_Load);
            this.ftpPanel.ResumeLayout(false);
            this.ftpPanel.PerformLayout();
            this.generalPanel.ResumeLayout(false);
            this.generalPanel.PerformLayout();
            this.keyPairPanel.ResumeLayout(false);
            this.keyPairPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyPairLoadingPictureBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel keyPairPanel;
        private System.Windows.Forms.Label keyPairGenerationLabel;
        private System.Windows.Forms.Label keyPairHeaderLabel;
        private System.Windows.Forms.Label keyPairInfoLabel;
        private System.Windows.Forms.PictureBox keyPairLoadingPictureBox;
        private System.Windows.Forms.Panel generalPanel;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.Label generalHeaderLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Panel ftpPanel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.Label adressLabel;
        private System.Windows.Forms.Label ftpHeaderLabel;
        private WatermarkTextBox nameTextBox;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private WatermarkTextBox userTextBox;
        private WatermarkTextBox adressTextBox;
        private WatermarkTextBox portTextBox;
        private WatermarkTextBox updateUrlTextBox;
        private System.Windows.Forms.Label updateUrlLabel;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.ComboBox protocolComboBox;
        private System.Windows.Forms.Label protocolLabel;
        private System.Windows.Forms.Button searchOnServerButton;
        private Controls.Line line1;
        private System.Windows.Forms.Button searchPathButton;
        private WatermarkTextBox localPathTextBox;
        private System.Windows.Forms.Label localPathLabel;
    }
}