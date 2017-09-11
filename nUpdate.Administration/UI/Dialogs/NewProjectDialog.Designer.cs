using nUpdate.Administration.UI.Controls;
using nUpdate.Internal.UI.Controls;

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
                _ftp.Dispose();
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
            this.controlPanel1 = new BottomPanel();
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
            this.label5 = new System.Windows.Forms.Label();
            this.saveCredentialsCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.localPathLabel = new System.Windows.Forms.Label();
            this.updateUrlTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.updateUrlLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.generalHeaderLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.localPathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.httpAuthenticationTabPage = new System.Windows.Forms.TabPage();
            this.httpAuthenticationPanel = new System.Windows.Forms.Panel();
            this.httpAuthenticationPasswordTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.httpAuthenticationUserTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.httpAuthenticationCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ftpTabPage = new System.Windows.Forms.TabPage();
            this.ftpPanel = new System.Windows.Forms.Panel();
            this.ipVersionComboBox = new System.Windows.Forms.ComboBox();
            this.ipVersionLabel = new System.Windows.Forms.Label();
            this.ftpImportButton = new System.Windows.Forms.Button();
            this.securityInfoButton = new System.Windows.Forms.Button();
            this.searchOnServerButton = new System.Windows.Forms.Button();
            this.line1 = new Line();
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
            this.statisticsServerTabPage = new System.Windows.Forms.TabPage();
            this.line2 = new Line();
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.httpAuthenticationTabPage.SuspendLayout();
            this.httpAuthenticationPanel.SuspendLayout();
            this.ftpTabPage.SuspendLayout();
            this.ftpPanel.SuspendLayout();
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
            this.informationCategoriesTabControl.Controls.Add(this.httpAuthenticationTabPage);
            this.informationCategoriesTabControl.Controls.Add(this.ftpTabPage);
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
            this.generalPanel.Controls.Add(this.label5);
            this.generalPanel.Controls.Add(this.saveCredentialsCheckBox);
            this.generalPanel.Controls.Add(this.label4);
            this.generalPanel.Controls.Add(this.pictureBox1);
            this.generalPanel.Controls.Add(this.localPathLabel);
            this.generalPanel.Controls.Add(this.updateUrlTextBox);
            this.generalPanel.Controls.Add(this.updateUrlLabel);
            this.generalPanel.Controls.Add(this.nameTextBox);
            this.generalPanel.Controls.Add(this.generalHeaderLabel);
            this.generalPanel.Controls.Add(this.nameLabel);
            this.generalPanel.Controls.Add(this.localPathTextBox);
            resources.ApplyResources(this.generalPanel, "generalPanel");
            this.generalPanel.Name = "generalPanel";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // saveCredentialsCheckBox
            // 
            resources.ApplyResources(this.saveCredentialsCheckBox, "saveCredentialsCheckBox");
            this.saveCredentialsCheckBox.Name = "saveCredentialsCheckBox";
            this.saveCredentialsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
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
            // httpAuthenticationTabPage
            // 
            this.httpAuthenticationTabPage.Controls.Add(this.httpAuthenticationPanel);
            this.httpAuthenticationTabPage.Controls.Add(this.httpAuthenticationCheckBox);
            this.httpAuthenticationTabPage.Controls.Add(this.label6);
            resources.ApplyResources(this.httpAuthenticationTabPage, "httpAuthenticationTabPage");
            this.httpAuthenticationTabPage.Name = "httpAuthenticationTabPage";
            this.httpAuthenticationTabPage.UseVisualStyleBackColor = true;
            // 
            // httpAuthenticationPanel
            // 
            this.httpAuthenticationPanel.Controls.Add(this.httpAuthenticationPasswordTextBox);
            this.httpAuthenticationPanel.Controls.Add(this.label7);
            this.httpAuthenticationPanel.Controls.Add(this.httpAuthenticationUserTextBox);
            this.httpAuthenticationPanel.Controls.Add(this.label9);
            resources.ApplyResources(this.httpAuthenticationPanel, "httpAuthenticationPanel");
            this.httpAuthenticationPanel.Name = "httpAuthenticationPanel";
            // 
            // httpAuthenticationPasswordTextBox
            // 
            resources.ApplyResources(this.httpAuthenticationPasswordTextBox, "httpAuthenticationPasswordTextBox");
            this.httpAuthenticationPasswordTextBox.Name = "httpAuthenticationPasswordTextBox";
            this.httpAuthenticationPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // httpAuthenticationUserTextBox
            // 
            resources.ApplyResources(this.httpAuthenticationUserTextBox, "httpAuthenticationUserTextBox");
            this.httpAuthenticationUserTextBox.Name = "httpAuthenticationUserTextBox";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // httpAuthenticationCheckBox
            // 
            resources.ApplyResources(this.httpAuthenticationCheckBox, "httpAuthenticationCheckBox");
            this.httpAuthenticationCheckBox.Name = "httpAuthenticationCheckBox";
            this.httpAuthenticationCheckBox.UseVisualStyleBackColor = true;
            this.httpAuthenticationCheckBox.CheckedChanged += new System.EventHandler(this.httpAuthenticationCheckBox_CheckedChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label6.Name = "label6";
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
            this.ftpPanel.Controls.Add(this.ipVersionComboBox);
            this.ftpPanel.Controls.Add(this.ipVersionLabel);
            this.ftpPanel.Controls.Add(this.ftpImportButton);
            this.ftpPanel.Controls.Add(this.securityInfoButton);
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
            // ipVersionComboBox
            // 
            this.ipVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ipVersionComboBox.FormattingEnabled = true;
            this.ipVersionComboBox.Items.AddRange(new object[] {
            resources.GetString("ipVersionComboBox.Items"),
            resources.GetString("ipVersionComboBox.Items1")});
            resources.ApplyResources(this.ipVersionComboBox, "ipVersionComboBox");
            this.ipVersionComboBox.Name = "ipVersionComboBox";
            // 
            // ipVersionLabel
            // 
            resources.ApplyResources(this.ipVersionLabel, "ipVersionLabel");
            this.ipVersionLabel.Name = "ipVersionLabel";
            // 
            // ftpImportButton
            // 
            resources.ApplyResources(this.ftpImportButton, "ftpImportButton");
            this.ftpImportButton.Name = "ftpImportButton";
            this.ftpImportButton.UseVisualStyleBackColor = true;
            this.ftpImportButton.Click += new System.EventHandler(this.ftpImportButton_Click);
            // 
            // securityInfoButton
            // 
            resources.ApplyResources(this.securityInfoButton, "securityInfoButton");
            this.securityInfoButton.Name = "securityInfoButton";
            this.securityInfoButton.UseVisualStyleBackColor = true;
            this.securityInfoButton.Click += new System.EventHandler(this.securityInfoButton_Click);
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
            this.line1.LineAlignment = Line.Alignment.Horizontal;
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
            resources.GetString("ftpProtocolComboBox.Items8"),
            resources.GetString("ftpProtocolComboBox.Items9")});
            resources.ApplyResources(this.ftpProtocolComboBox, "ftpProtocolComboBox");
            this.ftpProtocolComboBox.Name = "ftpProtocolComboBox";
            this.ftpProtocolComboBox.SelectedIndexChanged += new System.EventHandler(this.ftpProtocolComboBox_SelectedIndexChanged);
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
            this.line2.LineAlignment = Line.Alignment.Horizontal;
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.httpAuthenticationTabPage.ResumeLayout(false);
            this.httpAuthenticationTabPage.PerformLayout();
            this.httpAuthenticationPanel.ResumeLayout(false);
            this.httpAuthenticationPanel.PerformLayout();
            this.ftpTabPage.ResumeLayout(false);
            this.ftpPanel.ResumeLayout(false);
            this.ftpPanel.PerformLayout();
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
        private CueTextBox updateUrlTextBox;
        private System.Windows.Forms.Label updateUrlLabel;
        private System.Windows.Forms.Label ftpDirectoryLabel;
        private System.Windows.Forms.TextBox ftpDirectoryTextBox;
        private System.Windows.Forms.ComboBox ftpModeComboBox;
        private System.Windows.Forms.Label ftpModeLabel;
        private System.Windows.Forms.ComboBox ftpProtocolComboBox;
        private System.Windows.Forms.Label ftpProtocolLabel;
        private System.Windows.Forms.Button searchOnServerButton;
        private System.Windows.Forms.Label localPathLabel;
        private System.Windows.Forms.Button securityInfoButton;
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ButtonTextBox localPathTextBox;
        private System.Windows.Forms.CheckBox saveCredentialsCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ipVersionComboBox;
        private System.Windows.Forms.Label ipVersionLabel;
        private Line line1;
        private System.Windows.Forms.TabPage httpAuthenticationTabPage;
        private System.Windows.Forms.Panel httpAuthenticationPanel;
        private CueTextBox httpAuthenticationPasswordTextBox;
        private System.Windows.Forms.Label label7;
        private CueTextBox httpAuthenticationUserTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox httpAuthenticationCheckBox;
        private System.Windows.Forms.Label label6;
    }
}