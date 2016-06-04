using nUpdate.Administration.UI.Controls;
using nUpdate.UI.WinForms.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class NewProjectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectDialog));
            this.continueButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.controlPanel1 = new nUpdate.UI.WinForms.Controls.BottomPanel();
            this.backButton = new System.Windows.Forms.Button();
            this.informationCategoriesTabControl = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.keyPairTabPage = new System.Windows.Forms.TabPage();
            this.keyPairPanel = new System.Windows.Forms.Panel();
            this.keyPairGenerationLabel = new System.Windows.Forms.Label();
            this.keyPairHeaderLabel = new System.Windows.Forms.Label();
            this.keyPairInfoLabel = new System.Windows.Forms.Label();
            this.keyPairLoadingPictureBox = new System.Windows.Forms.PictureBox();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.generalPanel = new System.Windows.Forms.Panel();
            this.protocolComboBox = new System.Windows.Forms.ComboBox();
            this.protocolLabel = new System.Windows.Forms.Label();
            this.localPathLabel = new System.Windows.Forms.Label();
            this.updateDirectoryUriTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.updateUrlLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.generalHeaderLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.localPathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.httpTabPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.authenticateCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.phpOutputPathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.line3 = new nUpdate.UI.WinForms.Controls.Line();
            this.phpScriptUriTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.httpAuthenticatePanel = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.httpPasswordTextBox = new System.Windows.Forms.TextBox();
            this.httpUsernameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.ftpTabPage = new System.Windows.Forms.TabPage();
            this.ftpPanel = new System.Windows.Forms.Panel();
            this.ftpImportButton = new System.Windows.Forms.Button();
            this.searchOnServerButton = new System.Windows.Forms.Button();
            this.line1 = new nUpdate.UI.WinForms.Controls.Line();
            this.ftpProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.ftpProtocolLabel = new System.Windows.Forms.Label();
            this.ftpModeComboBox = new System.Windows.Forms.ComboBox();
            this.ftpModeLabel = new System.Windows.Forms.Label();
            this.ftpDirectoryLabel = new System.Windows.Forms.Label();
            this.ftpDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ftpPortTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.ftpUserTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.ftpHostTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.ftpPasswordTextBox = new System.Windows.Forms.TextBox();
            this.ftpPortLabel = new System.Windows.Forms.Label();
            this.ftpPasswordLabel = new System.Windows.Forms.Label();
            this.ftpUserLabel = new System.Windows.Forms.Label();
            this.ftpHostLabel = new System.Windows.Forms.Label();
            this.ftpHeaderLabel = new System.Windows.Forms.Label();
            this.customTabPage = new System.Windows.Forms.TabPage();
            this.line4 = new nUpdate.UI.WinForms.Controls.Line();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.transferAssemblyFilePathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.parametersListBox = new System.Windows.Forms.ListBox();
            this.removeParameterButton = new System.Windows.Forms.Button();
            this.addParameterButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.statisticsServerTabPage = new System.Windows.Forms.TabPage();
            this.line2 = new nUpdate.UI.WinForms.Controls.Line();
            this.selectServerButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.doNotUseStatisticsServerButton = new System.Windows.Forms.RadioButton();
            this.useStatisticsServerRadioButton = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.statisticsInfoPanel = new System.Windows.Forms.Panel();
            this.databaseNameLabel = new System.Windows.Forms.Label();
            this.sqlPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statisticsServerLabel = new System.Windows.Forms.Label();
            this.proxyTabPage = new System.Windows.Forms.TabPage();
            this.doNotUseProxyRadioButton = new System.Windows.Forms.RadioButton();
            this.useProxyRadioButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.proxyHeaderLabel = new System.Windows.Forms.Label();
            this.proxyPanel = new System.Windows.Forms.Panel();
            this.proxyUserTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.proxyHostLabel = new System.Windows.Forms.Label();
            this.proxyUserLabel = new System.Windows.Forms.Label();
            this.proxyPasswordLabel = new System.Windows.Forms.Label();
            this.proxyPasswordTextBox = new System.Windows.Forms.TextBox();
            this.proxyHostTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.controlPanel1.SuspendLayout();
            this.informationCategoriesTabControl.SuspendLayout();
            this.keyPairTabPage.SuspendLayout();
            this.keyPairPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyPairLoadingPictureBox)).BeginInit();
            this.generalTabPage.SuspendLayout();
            this.generalPanel.SuspendLayout();
            this.httpTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.httpAuthenticatePanel.SuspendLayout();
            this.ftpTabPage.SuspendLayout();
            this.ftpPanel.SuspendLayout();
            this.customTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statisticsServerTabPage.SuspendLayout();
            this.statisticsInfoPanel.SuspendLayout();
            this.proxyTabPage.SuspendLayout();
            this.proxyPanel.SuspendLayout();
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.backButton);
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
            // informationCategoriesTabControl
            // 
            this.informationCategoriesTabControl.Controls.Add(this.keyPairTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.generalTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.httpTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.ftpTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.customTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.statisticsServerTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.proxyTabPage);
            resources.ApplyResources(this.informationCategoriesTabControl, "informationCategoriesTabControl");
            this.informationCategoriesTabControl.Name = "informationCategoriesTabControl";
            this.informationCategoriesTabControl.SelectedIndex = 0;
            // 
            // keyPairTabPage
            // 
            this.keyPairTabPage.Controls.Add(this.keyPairPanel);
            resources.ApplyResources(this.keyPairTabPage, "keyPairTabPage");
            this.keyPairTabPage.Name = "keyPairTabPage";
            this.keyPairTabPage.UseVisualStyleBackColor = true;
            // 
            // keyPairPanel
            // 
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
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.generalPanel);
            resources.ApplyResources(this.generalTabPage, "generalTabPage");
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // generalPanel
            // 
            this.generalPanel.Controls.Add(this.protocolComboBox);
            this.generalPanel.Controls.Add(this.protocolLabel);
            this.generalPanel.Controls.Add(this.localPathLabel);
            this.generalPanel.Controls.Add(this.updateDirectoryUriTextBox);
            this.generalPanel.Controls.Add(this.updateUrlLabel);
            this.generalPanel.Controls.Add(this.nameTextBox);
            this.generalPanel.Controls.Add(this.generalHeaderLabel);
            this.generalPanel.Controls.Add(this.nameLabel);
            this.generalPanel.Controls.Add(this.localPathTextBox);
            resources.ApplyResources(this.generalPanel, "generalPanel");
            this.generalPanel.Name = "generalPanel";
            // 
            // protocolComboBox
            // 
            this.protocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.protocolComboBox.FormattingEnabled = true;
            this.protocolComboBox.Items.AddRange(new object[] {
            resources.GetString("protocolComboBox.Items"),
            resources.GetString("protocolComboBox.Items1"),
            resources.GetString("protocolComboBox.Items2")});
            resources.ApplyResources(this.protocolComboBox, "protocolComboBox");
            this.protocolComboBox.Name = "protocolComboBox";
            // 
            // protocolLabel
            // 
            resources.ApplyResources(this.protocolLabel, "protocolLabel");
            this.protocolLabel.Name = "protocolLabel";
            // 
            // localPathLabel
            // 
            resources.ApplyResources(this.localPathLabel, "localPathLabel");
            this.localPathLabel.Name = "localPathLabel";
            // 
            // updateDirectoryUriTextBox
            // 
            resources.ApplyResources(this.updateDirectoryUriTextBox, "updateDirectoryUriTextBox");
            this.updateDirectoryUriTextBox.Name = "updateDirectoryUriTextBox";
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
            this.localPathTextBox.ButtonText = "...";
            resources.ApplyResources(this.localPathTextBox, "localPathTextBox");
            this.localPathTextBox.Name = "localPathTextBox";
            // 
            // httpTabPage
            // 
            this.httpTabPage.Controls.Add(this.panel1);
            resources.ApplyResources(this.httpTabPage, "httpTabPage");
            this.httpTabPage.Name = "httpTabPage";
            this.httpTabPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.authenticateCheckBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.phpOutputPathTextBox);
            this.panel1.Controls.Add(this.line3);
            this.panel1.Controls.Add(this.phpScriptUriTextBox);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.httpAuthenticatePanel);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // authenticateCheckBox
            // 
            resources.ApplyResources(this.authenticateCheckBox, "authenticateCheckBox");
            this.authenticateCheckBox.Name = "authenticateCheckBox";
            this.authenticateCheckBox.UseVisualStyleBackColor = true;
            this.authenticateCheckBox.CheckedChanged += new System.EventHandler(this.authenticateCheckBox_CheckedChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // phpOutputPathTextBox
            // 
            this.phpOutputPathTextBox.ButtonText = "...";
            resources.ApplyResources(this.phpOutputPathTextBox, "phpOutputPathTextBox");
            this.phpOutputPathTextBox.Name = "phpOutputPathTextBox";
            // 
            // line3
            // 
            this.line3.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            resources.ApplyResources(this.line3, "line3");
            this.line3.Name = "line3";
            // 
            // phpScriptUriTextBox
            // 
            resources.ApplyResources(this.phpScriptUriTextBox, "phpScriptUriTextBox");
            this.phpScriptUriTextBox.Name = "phpScriptUriTextBox";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label14.Name = "label14";
            // 
            // httpAuthenticatePanel
            // 
            this.httpAuthenticatePanel.Controls.Add(this.label12);
            this.httpAuthenticatePanel.Controls.Add(this.label11);
            this.httpAuthenticatePanel.Controls.Add(this.httpPasswordTextBox);
            this.httpAuthenticatePanel.Controls.Add(this.httpUsernameTextBox);
            resources.ApplyResources(this.httpAuthenticatePanel, "httpAuthenticatePanel");
            this.httpAuthenticatePanel.Name = "httpAuthenticatePanel";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // httpPasswordTextBox
            // 
            resources.ApplyResources(this.httpPasswordTextBox, "httpPasswordTextBox");
            this.httpPasswordTextBox.Name = "httpPasswordTextBox";
            this.httpPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // httpUsernameTextBox
            // 
            resources.ApplyResources(this.httpUsernameTextBox, "httpUsernameTextBox");
            this.httpUsernameTextBox.Name = "httpUsernameTextBox";
            // 
            // ftpTabPage
            // 
            this.ftpTabPage.Controls.Add(this.ftpPanel);
            resources.ApplyResources(this.ftpTabPage, "ftpTabPage");
            this.ftpTabPage.Name = "ftpTabPage";
            this.ftpTabPage.UseVisualStyleBackColor = true;
            // 
            // ftpPanel
            // 
            this.ftpPanel.BackColor = System.Drawing.SystemColors.Window;
            this.ftpPanel.Controls.Add(this.ftpImportButton);
            this.ftpPanel.Controls.Add(this.searchOnServerButton);
            this.ftpPanel.Controls.Add(this.line1);
            this.ftpPanel.Controls.Add(this.ftpProtocolComboBox);
            this.ftpPanel.Controls.Add(this.ftpProtocolLabel);
            this.ftpPanel.Controls.Add(this.ftpModeComboBox);
            this.ftpPanel.Controls.Add(this.ftpModeLabel);
            this.ftpPanel.Controls.Add(this.ftpDirectoryLabel);
            this.ftpPanel.Controls.Add(this.ftpDirectoryTextBox);
            this.ftpPanel.Controls.Add(this.ftpPortTextBox);
            this.ftpPanel.Controls.Add(this.ftpUserTextBox);
            this.ftpPanel.Controls.Add(this.ftpHostTextBox);
            this.ftpPanel.Controls.Add(this.ftpPasswordTextBox);
            this.ftpPanel.Controls.Add(this.ftpPortLabel);
            this.ftpPanel.Controls.Add(this.ftpPasswordLabel);
            this.ftpPanel.Controls.Add(this.ftpUserLabel);
            this.ftpPanel.Controls.Add(this.ftpHostLabel);
            this.ftpPanel.Controls.Add(this.ftpHeaderLabel);
            resources.ApplyResources(this.ftpPanel, "ftpPanel");
            this.ftpPanel.Name = "ftpPanel";
            // 
            // ftpImportButton
            // 
            resources.ApplyResources(this.ftpImportButton, "ftpImportButton");
            this.ftpImportButton.Name = "ftpImportButton";
            this.ftpImportButton.UseVisualStyleBackColor = true;
            this.ftpImportButton.Click += new System.EventHandler(this.ftpImportButton_Click);
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
            this.line1.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            resources.ApplyResources(this.line1, "line1");
            this.line1.Name = "line1";
            // 
            // ftpProtocolComboBox
            // 
            this.ftpProtocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ftpProtocolComboBox.FormattingEnabled = true;
            this.ftpProtocolComboBox.Items.AddRange(new object[] {
            resources.GetString("ftpProtocolComboBox.Items"),
            resources.GetString("ftpProtocolComboBox.Items1"),
            resources.GetString("ftpProtocolComboBox.Items2"),
            resources.GetString("ftpProtocolComboBox.Items3"),
            resources.GetString("ftpProtocolComboBox.Items4"),
            resources.GetString("ftpProtocolComboBox.Items5"),
            resources.GetString("ftpProtocolComboBox.Items6"),
            resources.GetString("ftpProtocolComboBox.Items7"),
            resources.GetString("ftpProtocolComboBox.Items8")});
            resources.ApplyResources(this.ftpProtocolComboBox, "ftpProtocolComboBox");
            this.ftpProtocolComboBox.Name = "ftpProtocolComboBox";
            // 
            // ftpProtocolLabel
            // 
            resources.ApplyResources(this.ftpProtocolLabel, "ftpProtocolLabel");
            this.ftpProtocolLabel.Name = "ftpProtocolLabel";
            // 
            // ftpModeComboBox
            // 
            this.ftpModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ftpModeComboBox.FormattingEnabled = true;
            this.ftpModeComboBox.Items.AddRange(new object[] {
            resources.GetString("ftpModeComboBox.Items"),
            resources.GetString("ftpModeComboBox.Items1")});
            resources.ApplyResources(this.ftpModeComboBox, "ftpModeComboBox");
            this.ftpModeComboBox.Name = "ftpModeComboBox";
            // 
            // ftpModeLabel
            // 
            resources.ApplyResources(this.ftpModeLabel, "ftpModeLabel");
            this.ftpModeLabel.Name = "ftpModeLabel";
            // 
            // ftpDirectoryLabel
            // 
            resources.ApplyResources(this.ftpDirectoryLabel, "ftpDirectoryLabel");
            this.ftpDirectoryLabel.Name = "ftpDirectoryLabel";
            // 
            // ftpDirectoryTextBox
            // 
            resources.ApplyResources(this.ftpDirectoryTextBox, "ftpDirectoryTextBox");
            this.ftpDirectoryTextBox.Name = "ftpDirectoryTextBox";
            // 
            // ftpPortTextBox
            // 
            resources.ApplyResources(this.ftpPortTextBox, "ftpPortTextBox");
            this.ftpPortTextBox.Name = "ftpPortTextBox";
            this.ftpPortTextBox.ShortcutsEnabled = false;
            this.ftpPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ftpPortTextBox_KeyPress);
            // 
            // ftpUserTextBox
            // 
            resources.ApplyResources(this.ftpUserTextBox, "ftpUserTextBox");
            this.ftpUserTextBox.Name = "ftpUserTextBox";
            // 
            // ftpHostTextBox
            // 
            resources.ApplyResources(this.ftpHostTextBox, "ftpHostTextBox");
            this.ftpHostTextBox.Name = "ftpHostTextBox";
            // 
            // ftpPasswordTextBox
            // 
            resources.ApplyResources(this.ftpPasswordTextBox, "ftpPasswordTextBox");
            this.ftpPasswordTextBox.Name = "ftpPasswordTextBox";
            this.ftpPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // ftpPortLabel
            // 
            resources.ApplyResources(this.ftpPortLabel, "ftpPortLabel");
            this.ftpPortLabel.Name = "ftpPortLabel";
            // 
            // ftpPasswordLabel
            // 
            resources.ApplyResources(this.ftpPasswordLabel, "ftpPasswordLabel");
            this.ftpPasswordLabel.Name = "ftpPasswordLabel";
            // 
            // ftpUserLabel
            // 
            resources.ApplyResources(this.ftpUserLabel, "ftpUserLabel");
            this.ftpUserLabel.Name = "ftpUserLabel";
            // 
            // ftpHostLabel
            // 
            resources.ApplyResources(this.ftpHostLabel, "ftpHostLabel");
            this.ftpHostLabel.Name = "ftpHostLabel";
            // 
            // ftpHeaderLabel
            // 
            resources.ApplyResources(this.ftpHeaderLabel, "ftpHeaderLabel");
            this.ftpHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.ftpHeaderLabel.Name = "ftpHeaderLabel";
            // 
            // customTabPage
            // 
            this.customTabPage.Controls.Add(this.line4);
            this.customTabPage.Controls.Add(this.label7);
            this.customTabPage.Controls.Add(this.pictureBox1);
            this.customTabPage.Controls.Add(this.label5);
            this.customTabPage.Controls.Add(this.transferAssemblyFilePathTextBox);
            this.customTabPage.Controls.Add(this.label9);
            this.customTabPage.Controls.Add(this.parametersListBox);
            this.customTabPage.Controls.Add(this.removeParameterButton);
            this.customTabPage.Controls.Add(this.addParameterButton);
            this.customTabPage.Controls.Add(this.label6);
            resources.ApplyResources(this.customTabPage, "customTabPage");
            this.customTabPage.Name = "customTabPage";
            this.customTabPage.UseVisualStyleBackColor = true;
            // 
            // line4
            // 
            this.line4.BackColor = System.Drawing.SystemColors.Window;
            this.line4.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            resources.ApplyResources(this.line4, "line4");
            this.line4.Name = "line4";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // transferAssemblyFilePathTextBox
            // 
            this.transferAssemblyFilePathTextBox.ButtonText = "...";
            resources.ApplyResources(this.transferAssemblyFilePathTextBox, "transferAssemblyFilePathTextBox");
            this.transferAssemblyFilePathTextBox.Name = "transferAssemblyFilePathTextBox";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // parametersListBox
            // 
            this.parametersListBox.FormattingEnabled = true;
            resources.ApplyResources(this.parametersListBox, "parametersListBox");
            this.parametersListBox.Name = "parametersListBox";
            this.parametersListBox.SelectedIndexChanged += new System.EventHandler(this.parametersListBox_SelectedIndexChanged);
            // 
            // removeParameterButton
            // 
            resources.ApplyResources(this.removeParameterButton, "removeParameterButton");
            this.removeParameterButton.Name = "removeParameterButton";
            this.removeParameterButton.UseVisualStyleBackColor = true;
            // 
            // addParameterButton
            // 
            resources.ApplyResources(this.addParameterButton, "addParameterButton");
            this.addParameterButton.Name = "addParameterButton";
            this.addParameterButton.UseVisualStyleBackColor = true;
            this.addParameterButton.Click += new System.EventHandler(this.addParameterButton_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label6.Name = "label6";
            // 
            // statisticsServerTabPage
            // 
            this.statisticsServerTabPage.Controls.Add(this.line2);
            this.statisticsServerTabPage.Controls.Add(this.selectServerButton);
            this.statisticsServerTabPage.Controls.Add(this.label2);
            this.statisticsServerTabPage.Controls.Add(this.doNotUseStatisticsServerButton);
            this.statisticsServerTabPage.Controls.Add(this.useStatisticsServerRadioButton);
            this.statisticsServerTabPage.Controls.Add(this.label8);
            this.statisticsServerTabPage.Controls.Add(this.statisticsInfoPanel);
            resources.ApplyResources(this.statisticsServerTabPage, "statisticsServerTabPage");
            this.statisticsServerTabPage.Name = "statisticsServerTabPage";
            this.statisticsServerTabPage.UseVisualStyleBackColor = true;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.SystemColors.Window;
            this.line2.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            resources.ApplyResources(this.line2, "line2");
            this.line2.Name = "line2";
            // 
            // selectServerButton
            // 
            resources.ApplyResources(this.selectServerButton, "selectServerButton");
            this.selectServerButton.Name = "selectServerButton";
            this.selectServerButton.UseVisualStyleBackColor = true;
            this.selectServerButton.Click += new System.EventHandler(this.selectServerButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // doNotUseStatisticsServerButton
            // 
            resources.ApplyResources(this.doNotUseStatisticsServerButton, "doNotUseStatisticsServerButton");
            this.doNotUseStatisticsServerButton.Checked = true;
            this.doNotUseStatisticsServerButton.Name = "doNotUseStatisticsServerButton";
            this.doNotUseStatisticsServerButton.TabStop = true;
            this.doNotUseStatisticsServerButton.UseVisualStyleBackColor = true;
            // 
            // useStatisticsServerRadioButton
            // 
            resources.ApplyResources(this.useStatisticsServerRadioButton, "useStatisticsServerRadioButton");
            this.useStatisticsServerRadioButton.Name = "useStatisticsServerRadioButton";
            this.useStatisticsServerRadioButton.UseVisualStyleBackColor = true;
            this.useStatisticsServerRadioButton.CheckedChanged += new System.EventHandler(this.useStatisticsServerRadioButton_CheckedChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label8.Name = "label8";
            // 
            // statisticsInfoPanel
            // 
            this.statisticsInfoPanel.Controls.Add(this.databaseNameLabel);
            this.statisticsInfoPanel.Controls.Add(this.sqlPasswordTextBox);
            this.statisticsInfoPanel.Controls.Add(this.label1);
            this.statisticsInfoPanel.Controls.Add(this.statisticsServerLabel);
            resources.ApplyResources(this.statisticsInfoPanel, "statisticsInfoPanel");
            this.statisticsInfoPanel.Name = "statisticsInfoPanel";
            // 
            // databaseNameLabel
            // 
            resources.ApplyResources(this.databaseNameLabel, "databaseNameLabel");
            this.databaseNameLabel.Name = "databaseNameLabel";
            // 
            // sqlPasswordTextBox
            // 
            resources.ApplyResources(this.sqlPasswordTextBox, "sqlPasswordTextBox");
            this.sqlPasswordTextBox.Name = "sqlPasswordTextBox";
            this.sqlPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // statisticsServerLabel
            // 
            resources.ApplyResources(this.statisticsServerLabel, "statisticsServerLabel");
            this.statisticsServerLabel.Name = "statisticsServerLabel";
            // 
            // proxyTabPage
            // 
            this.proxyTabPage.Controls.Add(this.doNotUseProxyRadioButton);
            this.proxyTabPage.Controls.Add(this.useProxyRadioButton);
            this.proxyTabPage.Controls.Add(this.label3);
            this.proxyTabPage.Controls.Add(this.proxyHeaderLabel);
            this.proxyTabPage.Controls.Add(this.proxyPanel);
            resources.ApplyResources(this.proxyTabPage, "proxyTabPage");
            this.proxyTabPage.Name = "proxyTabPage";
            this.proxyTabPage.UseVisualStyleBackColor = true;
            // 
            // doNotUseProxyRadioButton
            // 
            resources.ApplyResources(this.doNotUseProxyRadioButton, "doNotUseProxyRadioButton");
            this.doNotUseProxyRadioButton.Checked = true;
            this.doNotUseProxyRadioButton.Name = "doNotUseProxyRadioButton";
            this.doNotUseProxyRadioButton.TabStop = true;
            this.doNotUseProxyRadioButton.UseVisualStyleBackColor = true;
            this.doNotUseProxyRadioButton.CheckedChanged += new System.EventHandler(this.doNotUseProxyRadioButton_CheckedChanged);
            // 
            // useProxyRadioButton
            // 
            resources.ApplyResources(this.useProxyRadioButton, "useProxyRadioButton");
            this.useProxyRadioButton.Name = "useProxyRadioButton";
            this.useProxyRadioButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // proxyHeaderLabel
            // 
            resources.ApplyResources(this.proxyHeaderLabel, "proxyHeaderLabel");
            this.proxyHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.proxyHeaderLabel.Name = "proxyHeaderLabel";
            // 
            // proxyPanel
            // 
            this.proxyPanel.Controls.Add(this.proxyUserTextBox);
            this.proxyPanel.Controls.Add(this.proxyHostLabel);
            this.proxyPanel.Controls.Add(this.proxyUserLabel);
            this.proxyPanel.Controls.Add(this.proxyPasswordLabel);
            this.proxyPanel.Controls.Add(this.proxyPasswordTextBox);
            this.proxyPanel.Controls.Add(this.proxyHostTextBox);
            resources.ApplyResources(this.proxyPanel, "proxyPanel");
            this.proxyPanel.Name = "proxyPanel";
            // 
            // proxyUserTextBox
            // 
            resources.ApplyResources(this.proxyUserTextBox, "proxyUserTextBox");
            this.proxyUserTextBox.Name = "proxyUserTextBox";
            // 
            // proxyHostLabel
            // 
            resources.ApplyResources(this.proxyHostLabel, "proxyHostLabel");
            this.proxyHostLabel.Name = "proxyHostLabel";
            // 
            // proxyUserLabel
            // 
            resources.ApplyResources(this.proxyUserLabel, "proxyUserLabel");
            this.proxyUserLabel.Name = "proxyUserLabel";
            // 
            // proxyPasswordLabel
            // 
            resources.ApplyResources(this.proxyPasswordLabel, "proxyPasswordLabel");
            this.proxyPasswordLabel.Name = "proxyPasswordLabel";
            // 
            // proxyPasswordTextBox
            // 
            resources.ApplyResources(this.proxyPasswordTextBox, "proxyPasswordTextBox");
            this.proxyPasswordTextBox.Name = "proxyPasswordTextBox";
            this.proxyPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // proxyHostTextBox
            // 
            resources.ApplyResources(this.proxyHostTextBox, "proxyHostTextBox");
            this.proxyHostTextBox.Name = "proxyHostTextBox";
            // 
            // loadingPanel
            // 
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Controls.Add(this.pictureBox2);
            resources.ApplyResources(this.loadingPanel, "loadingPanel");
            this.loadingPanel.Name = "loadingPanel";
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoEllipsis = true;
            resources.ApplyResources(this.loadingLabel, "loadingLabel");
            this.loadingLabel.Name = "loadingLabel";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.continueButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.informationCategoriesTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectDialog";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewProjectDialog_FormClosing);
            this.Load += new System.EventHandler(this.NewProjectDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.informationCategoriesTabControl.ResumeLayout(false);
            this.keyPairTabPage.ResumeLayout(false);
            this.keyPairPanel.ResumeLayout(false);
            this.keyPairPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyPairLoadingPictureBox)).EndInit();
            this.generalTabPage.ResumeLayout(false);
            this.generalPanel.ResumeLayout(false);
            this.generalPanel.PerformLayout();
            this.httpTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.httpAuthenticatePanel.ResumeLayout(false);
            this.httpAuthenticatePanel.PerformLayout();
            this.ftpTabPage.ResumeLayout(false);
            this.ftpPanel.ResumeLayout(false);
            this.ftpPanel.PerformLayout();
            this.customTabPage.ResumeLayout(false);
            this.customTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statisticsServerTabPage.ResumeLayout(false);
            this.statisticsServerTabPage.PerformLayout();
            this.statisticsInfoPanel.ResumeLayout(false);
            this.statisticsInfoPanel.PerformLayout();
            this.proxyTabPage.ResumeLayout(false);
            this.proxyTabPage.PerformLayout();
            this.proxyPanel.ResumeLayout(false);
            this.proxyPanel.PerformLayout();
            this.loadingPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel keyPairPanel;
        private System.Windows.Forms.Label keyPairGenerationLabel;
        private System.Windows.Forms.Label keyPairHeaderLabel;
        private System.Windows.Forms.Label keyPairInfoLabel;
        private System.Windows.Forms.PictureBox keyPairLoadingPictureBox;
        private System.Windows.Forms.Panel generalPanel;
        private System.Windows.Forms.Label generalHeaderLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Panel ftpPanel;
        private System.Windows.Forms.Label ftpPortLabel;
        private System.Windows.Forms.Label ftpPasswordLabel;
        private System.Windows.Forms.Label ftpUserLabel;
        private System.Windows.Forms.Label ftpHostLabel;
        private System.Windows.Forms.Label ftpHeaderLabel;
        private CueTextBox nameTextBox;
        private BottomPanel controlPanel1;
        private System.Windows.Forms.TextBox ftpPasswordTextBox;
        private CueTextBox ftpUserTextBox;
        private CueTextBox ftpHostTextBox;
        private CueTextBox ftpPortTextBox;
        private CueTextBox updateDirectoryUriTextBox;
        private System.Windows.Forms.Label updateUrlLabel;
        private System.Windows.Forms.Label ftpDirectoryLabel;
        private System.Windows.Forms.TextBox ftpDirectoryTextBox;
        private System.Windows.Forms.ComboBox ftpModeComboBox;
        private System.Windows.Forms.Label ftpModeLabel;
        private System.Windows.Forms.ComboBox ftpProtocolComboBox;
        private System.Windows.Forms.Label ftpProtocolLabel;
        private System.Windows.Forms.Button searchOnServerButton;
        private Line line1;
        private System.Windows.Forms.Label localPathLabel;
        private Controls.TablessTabControl informationCategoriesTabControl;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.TabPage keyPairTabPage;
        private System.Windows.Forms.TabPage ftpTabPage;
        private System.Windows.Forms.TabPage statisticsServerTabPage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel statisticsInfoPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton doNotUseStatisticsServerButton;
        private System.Windows.Forms.RadioButton useStatisticsServerRadioButton;
        private System.Windows.Forms.Label statisticsServerLabel;
        private System.Windows.Forms.TabPage proxyTabPage;
        private System.Windows.Forms.Button ftpImportButton;
        private CueTextBox proxyUserTextBox;
        private CueTextBox proxyHostTextBox;
        private System.Windows.Forms.TextBox proxyPasswordTextBox;
        private System.Windows.Forms.Label proxyPasswordLabel;
        private System.Windows.Forms.Label proxyUserLabel;
        private System.Windows.Forms.Label proxyHostLabel;
        private System.Windows.Forms.Label proxyHeaderLabel;
        private System.Windows.Forms.TextBox sqlPasswordTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private Line line2;
        private System.Windows.Forms.Button selectServerButton;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.RadioButton doNotUseProxyRadioButton;
        private System.Windows.Forms.RadioButton useProxyRadioButton;
        private System.Windows.Forms.Panel proxyPanel;
        private ButtonTextBox localPathTextBox;
        private System.Windows.Forms.TabPage customTabPage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button addParameterButton;
        private System.Windows.Forms.Button removeParameterButton;
        private System.Windows.Forms.ListBox parametersListBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox protocolComboBox;
        private System.Windows.Forms.Label protocolLabel;
        private System.Windows.Forms.TabPage httpTabPage;
        private System.Windows.Forms.Panel panel1;
        private Line line3;
        private CueTextBox httpUsernameTextBox;
        private CueTextBox phpScriptUriTextBox;
        private System.Windows.Forms.TextBox httpPasswordTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label4;
        private ButtonTextBox phpOutputPathTextBox;
        private System.Windows.Forms.CheckBox authenticateCheckBox;
        private System.Windows.Forms.Panel httpAuthenticatePanel;
        private System.Windows.Forms.Label label5;
        private ButtonTextBox transferAssemblyFilePathTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Line line4;
    }
}