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
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.backButton = new System.Windows.Forms.Button();
            this.tablessTabControl1 = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.keyPairTabPage = new System.Windows.Forms.TabPage();
            this.keyPairPanel = new System.Windows.Forms.Panel();
            this.keyPairGenerationLabel = new System.Windows.Forms.Label();
            this.keyPairHeaderLabel = new System.Windows.Forms.Label();
            this.keyPairInfoLabel = new System.Windows.Forms.Label();
            this.keyPairLoadingPictureBox = new System.Windows.Forms.PictureBox();
            this.generalTabPage = new System.Windows.Forms.TabPage();
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
            this.ftpTabPage = new System.Windows.Forms.TabPage();
            this.ftpPanel = new System.Windows.Forms.Panel();
            this.securityInfoButton = new System.Windows.Forms.Button();
            this.searchOnServerButton = new System.Windows.Forms.Button();
            this.line1 = new nUpdate.Administration.UI.Controls.Line();
            this.ftpProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.ftpProtocolLabel = new System.Windows.Forms.Label();
            this.ftpModeComboBox = new System.Windows.Forms.ComboBox();
            this.ftpModeLabel = new System.Windows.Forms.Label();
            this.ftpDirectoryLabel = new System.Windows.Forms.Label();
            this.ftpDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ftpPortTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.ftpUserTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.ftpHostTextBox = new nUpdate.Administration.WatermarkTextBox();
            this.ftpPasswordTextBox = new System.Windows.Forms.TextBox();
            this.ftpPortLabel = new System.Windows.Forms.Label();
            this.ftpPasswordLabel = new System.Windows.Forms.Label();
            this.ftpUserLabel = new System.Windows.Forms.Label();
            this.ftpHostLabel = new System.Windows.Forms.Label();
            this.ftpHeaderLabel = new System.Windows.Forms.Label();
            this.statisticsServerTabPage = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.useStatisticsServerRadioButton = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.proxyTabPage = new System.Windows.Forms.TabPage();
            this.ftpImportButton = new System.Windows.Forms.Button();
            this.controlPanel1.SuspendLayout();
            this.tablessTabControl1.SuspendLayout();
            this.keyPairTabPage.SuspendLayout();
            this.keyPairPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyPairLoadingPictureBox)).BeginInit();
            this.generalTabPage.SuspendLayout();
            this.generalPanel.SuspendLayout();
            this.ftpTabPage.SuspendLayout();
            this.ftpPanel.SuspendLayout();
            this.statisticsServerTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
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
            // tablessTabControl1
            // 
            this.tablessTabControl1.Controls.Add(this.keyPairTabPage);
            this.tablessTabControl1.Controls.Add(this.generalTabPage);
            this.tablessTabControl1.Controls.Add(this.ftpTabPage);
            this.tablessTabControl1.Controls.Add(this.statisticsServerTabPage);
            this.tablessTabControl1.Controls.Add(this.proxyTabPage);
            resources.ApplyResources(this.tablessTabControl1, "tablessTabControl1");
            this.tablessTabControl1.Name = "tablessTabControl1";
            this.tablessTabControl1.SelectedIndex = 0;
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
            this.line1.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            resources.ApplyResources(this.line1, "line1");
            this.line1.Name = "line1";
            // 
            // ftpProtocolComboBox
            // 
            this.ftpProtocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ftpProtocolComboBox.FormattingEnabled = true;
            this.ftpProtocolComboBox.Items.AddRange(new object[] {
            resources.GetString("ftpProtocolComboBox.Items"),
            resources.GetString("ftpProtocolComboBox.Items1")});
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
            this.ftpPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.portTextBox_KeyPress);
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
            this.statisticsServerTabPage.Controls.Add(this.pictureBox2);
            this.statisticsServerTabPage.Controls.Add(this.label2);
            this.statisticsServerTabPage.Controls.Add(this.radioButton2);
            this.statisticsServerTabPage.Controls.Add(this.useStatisticsServerRadioButton);
            this.statisticsServerTabPage.Controls.Add(this.label8);
            this.statisticsServerTabPage.Controls.Add(this.panel1);
            resources.ApplyResources(this.statisticsServerTabPage, "statisticsServerTabPage");
            this.statisticsServerTabPage.Name = "statisticsServerTabPage";
            this.statisticsServerTabPage.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // radioButton2
            // 
            resources.ApplyResources(this.radioButton2, "radioButton2");
            this.radioButton2.Checked = true;
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.TabStop = true;
            this.radioButton2.UseVisualStyleBackColor = true;
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
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // proxyTabPage
            // 
            resources.ApplyResources(this.proxyTabPage, "proxyTabPage");
            this.proxyTabPage.Name = "proxyTabPage";
            this.proxyTabPage.UseVisualStyleBackColor = true;
            // 
            // ftpImportButton
            // 
            resources.ApplyResources(this.ftpImportButton, "ftpImportButton");
            this.ftpImportButton.Name = "ftpImportButton";
            this.ftpImportButton.UseVisualStyleBackColor = true;
            this.ftpImportButton.Click += new System.EventHandler(this.ftpImportButton_Click);
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.continueButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.tablessTabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectDialog";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewProjectDialog_FormClosing);
            this.Load += new System.EventHandler(this.NewProjectDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.tablessTabControl1.ResumeLayout(false);
            this.keyPairTabPage.ResumeLayout(false);
            this.keyPairPanel.ResumeLayout(false);
            this.keyPairPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keyPairLoadingPictureBox)).EndInit();
            this.generalTabPage.ResumeLayout(false);
            this.generalPanel.ResumeLayout(false);
            this.generalPanel.PerformLayout();
            this.ftpTabPage.ResumeLayout(false);
            this.ftpPanel.ResumeLayout(false);
            this.ftpPanel.PerformLayout();
            this.statisticsServerTabPage.ResumeLayout(false);
            this.statisticsServerTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Label ftpPortLabel;
        private System.Windows.Forms.Label ftpPasswordLabel;
        private System.Windows.Forms.Label ftpUserLabel;
        private System.Windows.Forms.Label ftpHostLabel;
        private System.Windows.Forms.Label ftpHeaderLabel;
        private WatermarkTextBox nameTextBox;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.TextBox ftpPasswordTextBox;
        private WatermarkTextBox ftpUserTextBox;
        private WatermarkTextBox ftpHostTextBox;
        private WatermarkTextBox ftpPortTextBox;
        private WatermarkTextBox updateUrlTextBox;
        private System.Windows.Forms.Label updateUrlLabel;
        private System.Windows.Forms.Label ftpDirectoryLabel;
        private System.Windows.Forms.TextBox ftpDirectoryTextBox;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.ComboBox ftpModeComboBox;
        private System.Windows.Forms.Label ftpModeLabel;
        private System.Windows.Forms.ComboBox ftpProtocolComboBox;
        private System.Windows.Forms.Label ftpProtocolLabel;
        private System.Windows.Forms.Button searchOnServerButton;
        private Controls.Line line1;
        private System.Windows.Forms.Button searchPathButton;
        private WatermarkTextBox localPathTextBox;
        private System.Windows.Forms.Label localPathLabel;
        private System.Windows.Forms.Button securityInfoButton;
        private Controls.TablessTabControl tablessTabControl1;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.TabPage keyPairTabPage;
        private System.Windows.Forms.TabPage ftpTabPage;
        private System.Windows.Forms.TabPage statisticsServerTabPage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton useStatisticsServerRadioButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TabPage proxyTabPage;
        private System.Windows.Forms.Button ftpImportButton;
    }
}