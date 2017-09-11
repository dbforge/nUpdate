using nUpdate.Internal.UI.Controls;

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
                _ftp.Dispose();
                FtpPassword.Dispose();
                ProxyPassword.Dispose();
                SqlPassword.Dispose();
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
            this.continueButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.controlPanel1 = new BottomPanel();
            this.backButton = new System.Windows.Forms.Button();
            this.tablessTabControl1 = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.generalPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.saveCredentialsCheckBox = new System.Windows.Forms.CheckBox();
            this.localPathTextBox = new nUpdate.Administration.UI.Controls.ButtonTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.localPathLabel = new System.Windows.Forms.Label();
            this.updateUrlTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.updateUrlLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.generalHeaderLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
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
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.tablessTabControl1.SuspendLayout();
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
            this.SuspendLayout();
            // 
            // continueButton
            // 
            this.continueButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.continueButton.Location = new System.Drawing.Point(326, 8);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 9;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cancelButton.Location = new System.Drawing.Point(414, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // loadingPanel
            // 
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Controls.Add(this.pictureBox2);
            this.loadingPanel.Location = new System.Drawing.Point(136, 278);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(310, 47);
            this.loadingPanel.TabIndex = 92;
            this.loadingPanel.Visible = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoEllipsis = true;
            this.loadingLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.loadingLabel.Location = new System.Drawing.Point(34, 17);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(262, 15);
            this.loadingLabel.TabIndex = 1;
            this.loadingLabel.Text = "Initializing...";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox2.Location = new System.Drawing.Point(17, 16);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.backButton);
            this.controlPanel1.Controls.Add(this.continueButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 228);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(502, 40);
            this.controlPanel1.TabIndex = 90;
            // 
            // backButton
            // 
            this.backButton.Enabled = false;
            this.backButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.backButton.Location = new System.Drawing.Point(248, 8);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 10;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // tablessTabControl1
            // 
            this.tablessTabControl1.Controls.Add(this.generalTabPage);
            this.tablessTabControl1.Controls.Add(this.httpAuthenticationTabPage);
            this.tablessTabControl1.Controls.Add(this.ftpTabPage);
            this.tablessTabControl1.Controls.Add(this.statisticsServerTabPage);
            this.tablessTabControl1.Controls.Add(this.proxyTabPage);
            this.tablessTabControl1.Location = new System.Drawing.Point(1, 1);
            this.tablessTabControl1.Name = "tablessTabControl1";
            this.tablessTabControl1.SelectedIndex = 0;
            this.tablessTabControl1.Size = new System.Drawing.Size(503, 229);
            this.tablessTabControl1.TabIndex = 91;
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.generalPanel);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(495, 203);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "General";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // generalPanel
            // 
            this.generalPanel.Controls.Add(this.label5);
            this.generalPanel.Controls.Add(this.saveCredentialsCheckBox);
            this.generalPanel.Controls.Add(this.localPathTextBox);
            this.generalPanel.Controls.Add(this.label4);
            this.generalPanel.Controls.Add(this.pictureBox1);
            this.generalPanel.Controls.Add(this.localPathLabel);
            this.generalPanel.Controls.Add(this.updateUrlTextBox);
            this.generalPanel.Controls.Add(this.updateUrlLabel);
            this.generalPanel.Controls.Add(this.nameTextBox);
            this.generalPanel.Controls.Add(this.generalHeaderLabel);
            this.generalPanel.Controls.Add(this.nameLabel);
            this.generalPanel.Location = new System.Drawing.Point(0, 0);
            this.generalPanel.Name = "generalPanel";
            this.generalPanel.Size = new System.Drawing.Size(495, 203);
            this.generalPanel.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "Credentials:";
            // 
            // saveCredentialsCheckBox
            // 
            this.saveCredentialsCheckBox.AutoSize = true;
            this.saveCredentialsCheckBox.Location = new System.Drawing.Point(118, 172);
            this.saveCredentialsCheckBox.Name = "saveCredentialsCheckBox";
            this.saveCredentialsCheckBox.Size = new System.Drawing.Size(108, 17);
            this.saveCredentialsCheckBox.TabIndex = 39;
            this.saveCredentialsCheckBox.Text = "Save credentials";
            this.saveCredentialsCheckBox.UseVisualStyleBackColor = true;
            // 
            // localPathTextBox
            // 
            this.localPathTextBox.ButtonText = "...";
            this.localPathTextBox.Cue = "The path to the local file";
            this.localPathTextBox.Location = new System.Drawing.Point(118, 110);
            this.localPathTextBox.Name = "localPathTextBox";
            this.localPathTextBox.Size = new System.Drawing.Size(323, 22);
            this.localPathTextBox.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic);
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(136, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(326, 16);
            this.label4.TabIndex = 37;
            this.label4.Text = "If you want to move the project file, enter a new local path above.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(118, 141);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 36;
            this.pictureBox1.TabStop = false;
            // 
            // localPathLabel
            // 
            this.localPathLabel.AutoSize = true;
            this.localPathLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.localPathLabel.Location = new System.Drawing.Point(19, 113);
            this.localPathLabel.Name = "localPathLabel";
            this.localPathLabel.Size = new System.Drawing.Size(63, 13);
            this.localPathLabel.TabIndex = 28;
            this.localPathLabel.Text = "Local path:";
            // 
            // updateUrlTextBox
            // 
            this.updateUrlTextBox.Cue = "http(s)://www.yourserver.com/updatedirectory";
            this.updateUrlTextBox.Location = new System.Drawing.Point(118, 80);
            this.updateUrlTextBox.Name = "updateUrlTextBox";
            this.updateUrlTextBox.Size = new System.Drawing.Size(323, 22);
            this.updateUrlTextBox.TabIndex = 27;
            // 
            // updateUrlLabel
            // 
            this.updateUrlLabel.AutoSize = true;
            this.updateUrlLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.updateUrlLabel.Location = new System.Drawing.Point(18, 83);
            this.updateUrlLabel.Name = "updateUrlLabel";
            this.updateUrlLabel.Size = new System.Drawing.Size(72, 13);
            this.updateUrlLabel.TabIndex = 26;
            this.updateUrlLabel.Text = "Update-URL:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Cue = "The name of the project";
            this.nameTextBox.Location = new System.Drawing.Point(118, 51);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(323, 22);
            this.nameTextBox.TabIndex = 25;
            // 
            // generalHeaderLabel
            // 
            this.generalHeaderLabel.AutoSize = true;
            this.generalHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.generalHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.generalHeaderLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.generalHeaderLabel.Location = new System.Drawing.Point(12, 9);
            this.generalHeaderLabel.Name = "generalHeaderLabel";
            this.generalHeaderLabel.Size = new System.Drawing.Size(60, 20);
            this.generalHeaderLabel.TabIndex = 18;
            this.generalHeaderLabel.Text = "General";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.nameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.nameLabel.Location = new System.Drawing.Point(18, 54);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 13);
            this.nameLabel.TabIndex = 17;
            this.nameLabel.Text = "Name:";
            // 
            // httpAuthenticationTabPage
            // 
            this.httpAuthenticationTabPage.Controls.Add(this.httpAuthenticationPanel);
            this.httpAuthenticationTabPage.Controls.Add(this.httpAuthenticationCheckBox);
            this.httpAuthenticationTabPage.Controls.Add(this.label6);
            this.httpAuthenticationTabPage.Location = new System.Drawing.Point(4, 22);
            this.httpAuthenticationTabPage.Name = "httpAuthenticationTabPage";
            this.httpAuthenticationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.httpAuthenticationTabPage.Size = new System.Drawing.Size(495, 203);
            this.httpAuthenticationTabPage.TabIndex = 6;
            this.httpAuthenticationTabPage.Text = "Authentication";
            this.httpAuthenticationTabPage.UseVisualStyleBackColor = true;
            // 
            // httpAuthenticationPanel
            // 
            this.httpAuthenticationPanel.Controls.Add(this.httpAuthenticationPasswordTextBox);
            this.httpAuthenticationPanel.Controls.Add(this.label7);
            this.httpAuthenticationPanel.Controls.Add(this.httpAuthenticationUserTextBox);
            this.httpAuthenticationPanel.Controls.Add(this.label9);
            this.httpAuthenticationPanel.Enabled = false;
            this.httpAuthenticationPanel.Location = new System.Drawing.Point(20, 75);
            this.httpAuthenticationPanel.Name = "httpAuthenticationPanel";
            this.httpAuthenticationPanel.Size = new System.Drawing.Size(455, 67);
            this.httpAuthenticationPanel.TabIndex = 21;
            // 
            // httpAuthenticationPasswordTextBox
            // 
            this.httpAuthenticationPasswordTextBox.Cue = "The password to authenticate with";
            this.httpAuthenticationPasswordTextBox.Location = new System.Drawing.Point(118, 36);
            this.httpAuthenticationPasswordTextBox.Name = "httpAuthenticationPasswordTextBox";
            this.httpAuthenticationPasswordTextBox.Size = new System.Drawing.Size(323, 22);
            this.httpAuthenticationPasswordTextBox.TabIndex = 31;
            this.httpAuthenticationPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(18, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Password:";
            // 
            // httpAuthenticationUserTextBox
            // 
            this.httpAuthenticationUserTextBox.Cue = "The username to authenticate with";
            this.httpAuthenticationUserTextBox.Location = new System.Drawing.Point(118, 8);
            this.httpAuthenticationUserTextBox.Name = "httpAuthenticationUserTextBox";
            this.httpAuthenticationUserTextBox.Size = new System.Drawing.Size(323, 22);
            this.httpAuthenticationUserTextBox.TabIndex = 29;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(18, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Username:";
            // 
            // httpAuthenticationCheckBox
            // 
            this.httpAuthenticationCheckBox.AutoSize = true;
            this.httpAuthenticationCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.httpAuthenticationCheckBox.Location = new System.Drawing.Point(20, 52);
            this.httpAuthenticationCheckBox.Name = "httpAuthenticationCheckBox";
            this.httpAuthenticationCheckBox.Size = new System.Drawing.Size(183, 17);
            this.httpAuthenticationCheckBox.TabIndex = 20;
            this.httpAuthenticationCheckBox.Text = "Authenticate using credentials";
            this.httpAuthenticationCheckBox.UseVisualStyleBackColor = true;
            this.httpAuthenticationCheckBox.CheckedChanged += new System.EventHandler(this.httpAuthenticationCheckBox_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(163, 20);
            this.label6.TabIndex = 19;
            this.label6.Text = "HTTP(S) Authentication";
            // 
            // ftpTabPage
            // 
            this.ftpTabPage.Controls.Add(this.ftpPanel);
            this.ftpTabPage.Location = new System.Drawing.Point(4, 22);
            this.ftpTabPage.Name = "ftpTabPage";
            this.ftpTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ftpTabPage.Size = new System.Drawing.Size(495, 203);
            this.ftpTabPage.TabIndex = 2;
            this.ftpTabPage.Text = "FTP";
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
            this.ftpPanel.Location = new System.Drawing.Point(0, 0);
            this.ftpPanel.Name = "ftpPanel";
            this.ftpPanel.Size = new System.Drawing.Size(495, 203);
            this.ftpPanel.TabIndex = 24;
            // 
            // ipVersionComboBox
            // 
            this.ipVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ipVersionComboBox.FormattingEnabled = true;
            this.ipVersionComboBox.Items.AddRange(new object[] {
            "IPv4",
            "IPv6"});
            this.ipVersionComboBox.Location = new System.Drawing.Point(88, 182);
            this.ipVersionComboBox.Name = "ipVersionComboBox";
            this.ipVersionComboBox.Size = new System.Drawing.Size(142, 21);
            this.ipVersionComboBox.TabIndex = 44;
            // 
            // ipVersionLabel
            // 
            this.ipVersionLabel.AutoSize = true;
            this.ipVersionLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ipVersionLabel.Location = new System.Drawing.Point(17, 185);
            this.ipVersionLabel.Name = "ipVersionLabel";
            this.ipVersionLabel.Size = new System.Drawing.Size(60, 13);
            this.ipVersionLabel.TabIndex = 43;
            this.ipVersionLabel.Text = "IP-version:";
            // 
            // ftpImportButton
            // 
            this.ftpImportButton.Image = ((System.Drawing.Image)(resources.GetObject("ftpImportButton.Image")));
            this.ftpImportButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpImportButton.Location = new System.Drawing.Point(456, 9);
            this.ftpImportButton.Name = "ftpImportButton";
            this.ftpImportButton.Size = new System.Drawing.Size(33, 23);
            this.ftpImportButton.TabIndex = 40;
            this.ftpImportButton.UseVisualStyleBackColor = true;
            this.ftpImportButton.Click += new System.EventHandler(this.ftpImportButton_Click);
            // 
            // securityInfoButton
            // 
            this.securityInfoButton.Image = ((System.Drawing.Image)(resources.GetObject("securityInfoButton.Image")));
            this.securityInfoButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.securityInfoButton.Location = new System.Drawing.Point(443, 79);
            this.securityInfoButton.Name = "securityInfoButton";
            this.securityInfoButton.Size = new System.Drawing.Size(34, 23);
            this.securityInfoButton.TabIndex = 39;
            this.securityInfoButton.UseVisualStyleBackColor = true;
            this.securityInfoButton.Click += new System.EventHandler(this.securityInfoButton_Click);
            // 
            // searchOnServerButton
            // 
            this.searchOnServerButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.searchOnServerButton.Location = new System.Drawing.Point(317, 152);
            this.searchOnServerButton.Name = "searchOnServerButton";
            this.searchOnServerButton.Size = new System.Drawing.Size(119, 23);
            this.searchOnServerButton.TabIndex = 38;
            this.searchOnServerButton.Text = "Search on server";
            this.searchOnServerButton.UseVisualStyleBackColor = true;
            this.searchOnServerButton.Click += new System.EventHandler(this.searchOnServerButton_Click);
            // 
            // line1
            // 
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(14, 136);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(419, 10);
            this.line1.TabIndex = 37;
            this.line1.Text = "line1";
            // 
            // ftpProtocolComboBox
            // 
            this.ftpProtocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ftpProtocolComboBox.FormattingEnabled = true;
            this.ftpProtocolComboBox.Items.AddRange(new object[] {
            "FTP (not recommended)",
            "FTPS (TLS1 explicit)",
            "FTPS (TLS1 or SSL3 expl.)",
            "FTPS (SSL3 explicit)",
            "FTPS (SSL2 explicit)",
            "FTPS (TLS1 implicit)",
            "FTPS (TLS1 or SSL3 impl.)",
            "FTPS (SSL3 implicit)",
            "FTPS (SSL2 implicit)",
            "Custom"});
            this.ftpProtocolComboBox.Location = new System.Drawing.Point(305, 108);
            this.ftpProtocolComboBox.Name = "ftpProtocolComboBox";
            this.ftpProtocolComboBox.Size = new System.Drawing.Size(132, 21);
            this.ftpProtocolComboBox.TabIndex = 36;
            this.ftpProtocolComboBox.SelectedIndexChanged += new System.EventHandler(this.ftpProtocolComboBox_SelectedIndexChanged);
            // 
            // ftpProtocolLabel
            // 
            this.ftpProtocolLabel.AutoSize = true;
            this.ftpProtocolLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpProtocolLabel.Location = new System.Drawing.Point(239, 111);
            this.ftpProtocolLabel.Name = "ftpProtocolLabel";
            this.ftpProtocolLabel.Size = new System.Drawing.Size(53, 13);
            this.ftpProtocolLabel.TabIndex = 35;
            this.ftpProtocolLabel.Text = "Protocol:";
            // 
            // ftpModeComboBox
            // 
            this.ftpModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ftpModeComboBox.FormattingEnabled = true;
            this.ftpModeComboBox.Items.AddRange(new object[] {
            "Passive (adviced)",
            "Active"});
            this.ftpModeComboBox.Location = new System.Drawing.Point(89, 108);
            this.ftpModeComboBox.Name = "ftpModeComboBox";
            this.ftpModeComboBox.Size = new System.Drawing.Size(142, 21);
            this.ftpModeComboBox.TabIndex = 34;
            // 
            // ftpModeLabel
            // 
            this.ftpModeLabel.AutoSize = true;
            this.ftpModeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpModeLabel.Location = new System.Drawing.Point(18, 111);
            this.ftpModeLabel.Name = "ftpModeLabel";
            this.ftpModeLabel.Size = new System.Drawing.Size(40, 13);
            this.ftpModeLabel.TabIndex = 33;
            this.ftpModeLabel.Text = "Mode:";
            // 
            // ftpDirectoryLabel
            // 
            this.ftpDirectoryLabel.AutoSize = true;
            this.ftpDirectoryLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpDirectoryLabel.Location = new System.Drawing.Point(18, 156);
            this.ftpDirectoryLabel.Name = "ftpDirectoryLabel";
            this.ftpDirectoryLabel.Size = new System.Drawing.Size(56, 13);
            this.ftpDirectoryLabel.TabIndex = 32;
            this.ftpDirectoryLabel.Text = "Directory:";
            // 
            // ftpDirectoryTextBox
            // 
            this.ftpDirectoryTextBox.Location = new System.Drawing.Point(88, 153);
            this.ftpDirectoryTextBox.Name = "ftpDirectoryTextBox";
            this.ftpDirectoryTextBox.Size = new System.Drawing.Size(222, 22);
            this.ftpDirectoryTextBox.TabIndex = 31;
            this.ftpDirectoryTextBox.Text = "/";
            // 
            // ftpPortTextBox
            // 
            this.ftpPortTextBox.Cue = null;
            this.ftpPortTextBox.Location = new System.Drawing.Point(305, 51);
            this.ftpPortTextBox.Name = "ftpPortTextBox";
            this.ftpPortTextBox.Size = new System.Drawing.Size(72, 22);
            this.ftpPortTextBox.TabIndex = 30;
            this.ftpPortTextBox.Text = "21";
            this.ftpPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ftpPortTextBox_KeyPress);
            // 
            // ftpUserTextBox
            // 
            this.ftpUserTextBox.Cue = "The username";
            this.ftpUserTextBox.Location = new System.Drawing.Point(89, 79);
            this.ftpUserTextBox.Name = "ftpUserTextBox";
            this.ftpUserTextBox.Size = new System.Drawing.Size(142, 22);
            this.ftpUserTextBox.TabIndex = 29;
            // 
            // ftpHostTextBox
            // 
            this.ftpHostTextBox.Cue = "server.host.com";
            this.ftpHostTextBox.Location = new System.Drawing.Point(89, 51);
            this.ftpHostTextBox.Name = "ftpHostTextBox";
            this.ftpHostTextBox.Size = new System.Drawing.Size(142, 22);
            this.ftpHostTextBox.TabIndex = 28;
            // 
            // ftpPasswordTextBox
            // 
            this.ftpPasswordTextBox.Location = new System.Drawing.Point(304, 79);
            this.ftpPasswordTextBox.Name = "ftpPasswordTextBox";
            this.ftpPasswordTextBox.Size = new System.Drawing.Size(133, 22);
            this.ftpPasswordTextBox.TabIndex = 26;
            this.ftpPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // ftpPortLabel
            // 
            this.ftpPortLabel.AutoSize = true;
            this.ftpPortLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpPortLabel.Location = new System.Drawing.Point(239, 54);
            this.ftpPortLabel.Name = "ftpPortLabel";
            this.ftpPortLabel.Size = new System.Drawing.Size(31, 13);
            this.ftpPortLabel.TabIndex = 22;
            this.ftpPortLabel.Text = "Port:";
            // 
            // ftpPasswordLabel
            // 
            this.ftpPasswordLabel.AutoSize = true;
            this.ftpPasswordLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpPasswordLabel.Location = new System.Drawing.Point(239, 82);
            this.ftpPasswordLabel.Name = "ftpPasswordLabel";
            this.ftpPasswordLabel.Size = new System.Drawing.Size(59, 13);
            this.ftpPasswordLabel.TabIndex = 21;
            this.ftpPasswordLabel.Text = "Password:";
            // 
            // ftpUserLabel
            // 
            this.ftpUserLabel.AutoSize = true;
            this.ftpUserLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpUserLabel.Location = new System.Drawing.Point(18, 82);
            this.ftpUserLabel.Name = "ftpUserLabel";
            this.ftpUserLabel.Size = new System.Drawing.Size(33, 13);
            this.ftpUserLabel.TabIndex = 20;
            this.ftpUserLabel.Text = "User:";
            // 
            // ftpHostLabel
            // 
            this.ftpHostLabel.AutoSize = true;
            this.ftpHostLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpHostLabel.Location = new System.Drawing.Point(18, 54);
            this.ftpHostLabel.Name = "ftpHostLabel";
            this.ftpHostLabel.Size = new System.Drawing.Size(55, 13);
            this.ftpHostLabel.TabIndex = 17;
            this.ftpHostLabel.Text = "FTP-Host:";
            // 
            // ftpHeaderLabel
            // 
            this.ftpHeaderLabel.AutoSize = true;
            this.ftpHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.ftpHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.ftpHeaderLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpHeaderLabel.Location = new System.Drawing.Point(12, 9);
            this.ftpHeaderLabel.Name = "ftpHeaderLabel";
            this.ftpHeaderLabel.Size = new System.Drawing.Size(70, 20);
            this.ftpHeaderLabel.TabIndex = 14;
            this.ftpHeaderLabel.Text = "FTP-Data";
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
            this.statisticsServerTabPage.Location = new System.Drawing.Point(4, 22);
            this.statisticsServerTabPage.Name = "statisticsServerTabPage";
            this.statisticsServerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statisticsServerTabPage.Size = new System.Drawing.Size(495, 203);
            this.statisticsServerTabPage.TabIndex = 3;
            this.statisticsServerTabPage.Text = "Statistics";
            this.statisticsServerTabPage.UseVisualStyleBackColor = true;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.SystemColors.Window;
            this.line2.LineAlignment = Line.Alignment.Horizontal;
            this.line2.Location = new System.Drawing.Point(5, 117);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(481, 10);
            this.line2.TabIndex = 4;
            this.line2.Text = "line2";
            // 
            // selectServerButton
            // 
            this.selectServerButton.Enabled = false;
            this.selectServerButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.selectServerButton.Location = new System.Drawing.Point(358, 93);
            this.selectServerButton.Name = "selectServerButton";
            this.selectServerButton.Size = new System.Drawing.Size(124, 23);
            this.selectServerButton.TabIndex = 63;
            this.selectServerButton.Text = "Select server...";
            this.selectServerButton.UseVisualStyleBackColor = true;
            this.selectServerButton.Click += new System.EventHandler(this.selectServerButton_Click);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(13, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(476, 35);
            this.label2.TabIndex = 62;
            this.label2.Text = "If you would like to receive stats about the downloads of your update packages, t" +
    "hen you \r\ncan configurate a statistics server here. All you need for it is PHP a" +
    "nd a MySQL-database.";
            // 
            // doNotUseStatisticsServerButton
            // 
            this.doNotUseStatisticsServerButton.AutoSize = true;
            this.doNotUseStatisticsServerButton.Checked = true;
            this.doNotUseStatisticsServerButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.doNotUseStatisticsServerButton.Location = new System.Drawing.Point(26, 76);
            this.doNotUseStatisticsServerButton.Name = "doNotUseStatisticsServerButton";
            this.doNotUseStatisticsServerButton.Size = new System.Drawing.Size(164, 17);
            this.doNotUseStatisticsServerButton.TabIndex = 61;
            this.doNotUseStatisticsServerButton.TabStop = true;
            this.doNotUseStatisticsServerButton.Text = "Don\'t use a statistics server";
            this.doNotUseStatisticsServerButton.UseVisualStyleBackColor = true;
            // 
            // useStatisticsServerRadioButton
            // 
            this.useStatisticsServerRadioButton.AutoSize = true;
            this.useStatisticsServerRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.useStatisticsServerRadioButton.Location = new System.Drawing.Point(26, 96);
            this.useStatisticsServerRadioButton.Name = "useStatisticsServerRadioButton";
            this.useStatisticsServerRadioButton.Size = new System.Drawing.Size(133, 17);
            this.useStatisticsServerRadioButton.TabIndex = 60;
            this.useStatisticsServerRadioButton.Text = "Use a statistics server";
            this.useStatisticsServerRadioButton.UseVisualStyleBackColor = true;
            this.useStatisticsServerRadioButton.CheckedChanged += new System.EventHandler(this.useStatisticsServerRadioButton_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(12, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 20);
            this.label8.TabIndex = 40;
            this.label8.Text = "Statistics server";
            // 
            // statisticsInfoPanel
            // 
            this.statisticsInfoPanel.Controls.Add(this.databaseNameLabel);
            this.statisticsInfoPanel.Controls.Add(this.sqlPasswordTextBox);
            this.statisticsInfoPanel.Controls.Add(this.label1);
            this.statisticsInfoPanel.Controls.Add(this.statisticsServerLabel);
            this.statisticsInfoPanel.Enabled = false;
            this.statisticsInfoPanel.Location = new System.Drawing.Point(26, 135);
            this.statisticsInfoPanel.Name = "statisticsInfoPanel";
            this.statisticsInfoPanel.Size = new System.Drawing.Size(302, 60);
            this.statisticsInfoPanel.TabIndex = 59;
            // 
            // databaseNameLabel
            // 
            this.databaseNameLabel.AutoSize = true;
            this.databaseNameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.databaseNameLabel.Location = new System.Drawing.Point(118, 5);
            this.databaseNameLabel.Name = "databaseNameLabel";
            this.databaseNameLabel.Size = new System.Drawing.Size(11, 13);
            this.databaseNameLabel.TabIndex = 4;
            this.databaseNameLabel.Text = "-";
            // 
            // sqlPasswordTextBox
            // 
            this.sqlPasswordTextBox.Location = new System.Drawing.Point(121, 30);
            this.sqlPasswordTextBox.Name = "sqlPasswordTextBox";
            this.sqlPasswordTextBox.Size = new System.Drawing.Size(169, 22);
            this.sqlPasswordTextBox.TabIndex = 3;
            this.sqlPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(19, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Password:";
            // 
            // statisticsServerLabel
            // 
            this.statisticsServerLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statisticsServerLabel.Location = new System.Drawing.Point(18, 5);
            this.statisticsServerLabel.Name = "statisticsServerLabel";
            this.statisticsServerLabel.Size = new System.Drawing.Size(78, 18);
            this.statisticsServerLabel.TabIndex = 0;
            this.statisticsServerLabel.Text = "Database: ";
            // 
            // proxyTabPage
            // 
            this.proxyTabPage.Controls.Add(this.doNotUseProxyRadioButton);
            this.proxyTabPage.Controls.Add(this.useProxyRadioButton);
            this.proxyTabPage.Controls.Add(this.label3);
            this.proxyTabPage.Controls.Add(this.proxyHeaderLabel);
            this.proxyTabPage.Controls.Add(this.proxyPanel);
            this.proxyTabPage.Location = new System.Drawing.Point(4, 22);
            this.proxyTabPage.Name = "proxyTabPage";
            this.proxyTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.proxyTabPage.Size = new System.Drawing.Size(495, 203);
            this.proxyTabPage.TabIndex = 4;
            this.proxyTabPage.Text = "Proxy";
            this.proxyTabPage.UseVisualStyleBackColor = true;
            // 
            // doNotUseProxyRadioButton
            // 
            this.doNotUseProxyRadioButton.AutoSize = true;
            this.doNotUseProxyRadioButton.Checked = true;
            this.doNotUseProxyRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.doNotUseProxyRadioButton.Location = new System.Drawing.Point(27, 68);
            this.doNotUseProxyRadioButton.Name = "doNotUseProxyRadioButton";
            this.doNotUseProxyRadioButton.Size = new System.Drawing.Size(148, 17);
            this.doNotUseProxyRadioButton.TabIndex = 91;
            this.doNotUseProxyRadioButton.TabStop = true;
            this.doNotUseProxyRadioButton.Text = "Don\'t use a proxy server";
            this.doNotUseProxyRadioButton.UseVisualStyleBackColor = true;
            this.doNotUseProxyRadioButton.CheckedChanged += new System.EventHandler(this.doNotUseProxyRadioButton_CheckedChanged);
            // 
            // useProxyRadioButton
            // 
            this.useProxyRadioButton.AutoSize = true;
            this.useProxyRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.useProxyRadioButton.Location = new System.Drawing.Point(27, 88);
            this.useProxyRadioButton.Name = "useProxyRadioButton";
            this.useProxyRadioButton.Size = new System.Drawing.Size(117, 17);
            this.useProxyRadioButton.TabIndex = 90;
            this.useProxyRadioButton.Text = "Use a proxy server";
            this.useProxyRadioButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(13, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(450, 13);
            this.label3.TabIndex = 88;
            this.label3.Text = "If this project should use a proxy for the up- and downloads, then configurate it" +
    " here.";
            // 
            // proxyHeaderLabel
            // 
            this.proxyHeaderLabel.AutoSize = true;
            this.proxyHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.proxyHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.proxyHeaderLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.proxyHeaderLabel.Location = new System.Drawing.Point(12, 9);
            this.proxyHeaderLabel.Name = "proxyHeaderLabel";
            this.proxyHeaderLabel.Size = new System.Drawing.Size(45, 20);
            this.proxyHeaderLabel.TabIndex = 41;
            this.proxyHeaderLabel.Text = "Proxy";
            // 
            // proxyPanel
            // 
            this.proxyPanel.Controls.Add(this.proxyUserTextBox);
            this.proxyPanel.Controls.Add(this.proxyHostLabel);
            this.proxyPanel.Controls.Add(this.proxyUserLabel);
            this.proxyPanel.Controls.Add(this.proxyPasswordLabel);
            this.proxyPanel.Controls.Add(this.proxyPasswordTextBox);
            this.proxyPanel.Controls.Add(this.proxyHostTextBox);
            this.proxyPanel.Enabled = false;
            this.proxyPanel.Location = new System.Drawing.Point(15, 113);
            this.proxyPanel.Name = "proxyPanel";
            this.proxyPanel.Size = new System.Drawing.Size(457, 60);
            this.proxyPanel.TabIndex = 92;
            // 
            // proxyUserTextBox
            // 
            this.proxyUserTextBox.Cue = "The username";
            this.proxyUserTextBox.Location = new System.Drawing.Point(102, 33);
            this.proxyUserTextBox.Name = "proxyUserTextBox";
            this.proxyUserTextBox.Size = new System.Drawing.Size(142, 22);
            this.proxyUserTextBox.TabIndex = 48;
            // 
            // proxyHostLabel
            // 
            this.proxyHostLabel.AutoSize = true;
            this.proxyHostLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.proxyHostLabel.Location = new System.Drawing.Point(31, 8);
            this.proxyHostLabel.Name = "proxyHostLabel";
            this.proxyHostLabel.Size = new System.Drawing.Size(65, 13);
            this.proxyHostLabel.TabIndex = 42;
            this.proxyHostLabel.Text = "Proxy-Host:";
            // 
            // proxyUserLabel
            // 
            this.proxyUserLabel.AutoSize = true;
            this.proxyUserLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.proxyUserLabel.Location = new System.Drawing.Point(31, 36);
            this.proxyUserLabel.Name = "proxyUserLabel";
            this.proxyUserLabel.Size = new System.Drawing.Size(33, 13);
            this.proxyUserLabel.TabIndex = 43;
            this.proxyUserLabel.Text = "User:";
            // 
            // proxyPasswordLabel
            // 
            this.proxyPasswordLabel.AutoSize = true;
            this.proxyPasswordLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.proxyPasswordLabel.Location = new System.Drawing.Point(252, 36);
            this.proxyPasswordLabel.Name = "proxyPasswordLabel";
            this.proxyPasswordLabel.Size = new System.Drawing.Size(59, 13);
            this.proxyPasswordLabel.TabIndex = 44;
            this.proxyPasswordLabel.Text = "Password:";
            // 
            // proxyPasswordTextBox
            // 
            this.proxyPasswordTextBox.Location = new System.Drawing.Point(317, 33);
            this.proxyPasswordTextBox.Name = "proxyPasswordTextBox";
            this.proxyPasswordTextBox.Size = new System.Drawing.Size(133, 22);
            this.proxyPasswordTextBox.TabIndex = 46;
            this.proxyPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // proxyHostTextBox
            // 
            this.proxyHostTextBox.Cue = "http(s)://proxyserver:80";
            this.proxyHostTextBox.Location = new System.Drawing.Point(102, 5);
            this.proxyHostTextBox.Name = "proxyHostTextBox";
            this.proxyHostTextBox.Size = new System.Drawing.Size(348, 22);
            this.proxyHostTextBox.TabIndex = 47;
            // 
            // ProjectEditDialog
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(502, 268);
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.tablessTabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProjectEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit project - {0} - {1}";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectEditDialog_FormClosing);
            this.Load += new System.EventHandler(this.ProjectEditDialog_Load);
            this.loadingPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.tablessTabControl1.ResumeLayout(false);
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
            this.ResumeLayout(false);

        }

        #endregion

        private Line line2;
        private System.Windows.Forms.Button selectServerButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton doNotUseStatisticsServerButton;
        private System.Windows.Forms.RadioButton useStatisticsServerRadioButton;
        private System.Windows.Forms.Panel statisticsInfoPanel;
        private System.Windows.Forms.Label databaseNameLabel;
        private System.Windows.Forms.TextBox sqlPasswordTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statisticsServerLabel;
        private System.Windows.Forms.Button ftpImportButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button securityInfoButton;
        private System.Windows.Forms.Button searchOnServerButton;
        private Line line1;
        private System.Windows.Forms.ComboBox ftpProtocolComboBox;
        private System.Windows.Forms.Label ftpProtocolLabel;
        private System.Windows.Forms.ComboBox ftpModeComboBox;
        private System.Windows.Forms.Label ftpModeLabel;
        private System.Windows.Forms.Label ftpDirectoryLabel;
        private System.Windows.Forms.TabPage statisticsServerTabPage;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.TextBox proxyPasswordTextBox;
        private Controls.CueTextBox proxyHostTextBox;
        private System.Windows.Forms.RadioButton doNotUseProxyRadioButton;
        private System.Windows.Forms.RadioButton useProxyRadioButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label proxyHeaderLabel;
        private System.Windows.Forms.Label proxyUserLabel;
        private System.Windows.Forms.Label proxyPasswordLabel;
        private System.Windows.Forms.Panel proxyPanel;
        private Controls.CueTextBox proxyUserTextBox;
        private System.Windows.Forms.Label proxyHostLabel;
        private System.Windows.Forms.TextBox ftpDirectoryTextBox;
        private System.Windows.Forms.TabPage proxyTabPage;
        private Controls.CueTextBox ftpPortTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button backButton;
        private Controls.TablessTabControl tablessTabControl1;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.Panel generalPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label localPathLabel;
        private Controls.CueTextBox updateUrlTextBox;
        private System.Windows.Forms.Label updateUrlLabel;
        private Controls.CueTextBox nameTextBox;
        private System.Windows.Forms.Label generalHeaderLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TabPage ftpTabPage;
        private System.Windows.Forms.Panel ftpPanel;
        private Controls.CueTextBox ftpUserTextBox;
        private Controls.CueTextBox ftpHostTextBox;
        private System.Windows.Forms.TextBox ftpPasswordTextBox;
        private System.Windows.Forms.Label ftpPortLabel;
        private System.Windows.Forms.Label ftpPasswordLabel;
        private System.Windows.Forms.Label ftpUserLabel;
        private System.Windows.Forms.Label ftpHostLabel;
        private System.Windows.Forms.Label ftpHeaderLabel;
        private Controls.ButtonTextBox localPathTextBox;
        private System.Windows.Forms.CheckBox saveCredentialsCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ipVersionComboBox;
        private System.Windows.Forms.Label ipVersionLabel;
        private System.Windows.Forms.TabPage httpAuthenticationTabPage;
        private System.Windows.Forms.Panel httpAuthenticationPanel;
        private Controls.CueTextBox httpAuthenticationPasswordTextBox;
        private System.Windows.Forms.Label label7;
        private Controls.CueTextBox httpAuthenticationUserTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox httpAuthenticationCheckBox;
        private System.Windows.Forms.Label label6;
    }
}