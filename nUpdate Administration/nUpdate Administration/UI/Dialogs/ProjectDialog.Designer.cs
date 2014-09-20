using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class ProjectDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectDialog));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Released", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Not released", System.Windows.Forms.HorizontalAlignment.Left);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.overviewTabPage = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.line6 = new nUpdate.Administration.UI.Controls.Line();
            this.label4 = new System.Windows.Forms.Label();
            this.browseAssemblyButton = new System.Windows.Forms.Button();
            this.assemblyPathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.line5 = new nUpdate.Administration.UI.Controls.Line();
            this.label3 = new System.Windows.Forms.Label();
            this.stepTwoLabel = new System.Windows.Forms.Label();
            this.programmingLanguageComboBox = new System.Windows.Forms.ComboBox();
            this.stepOneLabel = new System.Windows.Forms.Label();
            this.assumeInfoLabel = new System.Windows.Forms.Label();
            this.copySourceButton = new System.Windows.Forms.Button();
            this.assumeHeader = new System.Windows.Forms.Label();
            this.editFtpButton = new System.Windows.Forms.Button();
            this.editProjectButton = new System.Windows.Forms.Button();
            this.checkingUrlPictureBox = new System.Windows.Forms.PictureBox();
            this.tickPictureBox = new System.Windows.Forms.PictureBox();
            this.line3 = new nUpdate.Administration.UI.Controls.Line();
            this.newestPackageLabel = new System.Windows.Forms.Label();
            this.amountLabel = new System.Windows.Forms.Label();
            this.checkUpdateConfigurationLinkLabel = new System.Windows.Forms.LinkLabel();
            this.infoFileloadingLabel = new System.Windows.Forms.Label();
            this.ftpDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ftpHostTextBox = new System.Windows.Forms.TextBox();
            this.updateUrlTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.updateUrlLabel = new System.Windows.Forms.Label();
            this.projectIdTextBox = new System.Windows.Forms.TextBox();
            this.projectIdLabel = new System.Windows.Forms.Label();
            this.publicKeyTextBox = new System.Windows.Forms.TextBox();
            this.publicKeyLabel = new System.Windows.Forms.Label();
            this.line2 = new nUpdate.Administration.UI.Controls.Line();
            this.projectDataHeader = new System.Windows.Forms.Label();
            this.line1 = new nUpdate.Administration.UI.Controls.Line();
            this.ftpDirectoryLabel = new System.Windows.Forms.Label();
            this.ftpHostLabel = new System.Windows.Forms.Label();
            this.overviewHeader = new System.Windows.Forms.Label();
            this.newestPackageReleasedLabel = new System.Windows.Forms.Label();
            this.releasedPackagesAmountLabel = new System.Windows.Forms.Label();
            this.line4 = new nUpdate.Administration.UI.Controls.Line();
            this.statisticsServerPanel = new System.Windows.Forms.Panel();
            this.selectStatisticsServerButton = new System.Windows.Forms.Button();
            this.dataBaseLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.loadFromAssemblyRadioButton = new System.Windows.Forms.RadioButton();
            this.enterVersionManuallyRadioButton = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.useStatisticsServerRadioButton = new System.Windows.Forms.RadioButton();
            this.doNotUseStatisticsServerRadioButton = new System.Windows.Forms.RadioButton();
            this.packagesTabPage = new System.Windows.Forms.TabPage();
            this.packagesList = new nUpdate.Administration.UI.Controls.ExtendedListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.uploadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.historyButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.searchTextBox = new nUpdate.Administration.UI.Controls.WatermarkTextBox();
            this.statisticsTabPage = new System.Windows.Forms.TabPage();
            this.statisticsDataGridView = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.lastUpdatedLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.noStatisticsPanel = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.overviewTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkingUrlPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tickPictureBox)).BeginInit();
            this.statisticsServerPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.packagesTabPage.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statisticsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statisticsDataGridView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.noStatisticsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "box.png");
            this.imageList1.Images.SetKeyName(1, "document-share.png");
            this.imageList1.Images.SetKeyName(2, "wrench.png");
            this.imageList1.Images.SetKeyName(3, "counter-count-up.png");
            // 
            // loadingPanel
            // 
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Controls.Add(this.pictureBox1);
            this.loadingPanel.Location = new System.Drawing.Point(307, 438);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(210, 47);
            this.loadingPanel.TabIndex = 32;
            this.loadingPanel.Visible = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.Location = new System.Drawing.Point(34, 17);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(150, 13);
            this.loadingLabel.TabIndex = 1;
            this.loadingLabel.Text = "Creating new project data...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(19, 19);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.overviewTabPage);
            this.tabControl1.Controls.Add(this.packagesTabPage);
            this.tabControl1.Controls.Add(this.statisticsTabPage);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(574, 413);
            this.tabControl1.TabIndex = 3;
            // 
            // overviewTabPage
            // 
            this.overviewTabPage.AutoScroll = true;
            this.overviewTabPage.AutoScrollMargin = new System.Drawing.Size(1, 20);
            this.overviewTabPage.AutoScrollMinSize = new System.Drawing.Size(540, 900);
            this.overviewTabPage.Controls.Add(this.label8);
            this.overviewTabPage.Controls.Add(this.line6);
            this.overviewTabPage.Controls.Add(this.label4);
            this.overviewTabPage.Controls.Add(this.browseAssemblyButton);
            this.overviewTabPage.Controls.Add(this.assemblyPathTextBox);
            this.overviewTabPage.Controls.Add(this.label2);
            this.overviewTabPage.Controls.Add(this.line5);
            this.overviewTabPage.Controls.Add(this.label3);
            this.overviewTabPage.Controls.Add(this.stepTwoLabel);
            this.overviewTabPage.Controls.Add(this.programmingLanguageComboBox);
            this.overviewTabPage.Controls.Add(this.stepOneLabel);
            this.overviewTabPage.Controls.Add(this.assumeInfoLabel);
            this.overviewTabPage.Controls.Add(this.copySourceButton);
            this.overviewTabPage.Controls.Add(this.assumeHeader);
            this.overviewTabPage.Controls.Add(this.editFtpButton);
            this.overviewTabPage.Controls.Add(this.editProjectButton);
            this.overviewTabPage.Controls.Add(this.checkingUrlPictureBox);
            this.overviewTabPage.Controls.Add(this.tickPictureBox);
            this.overviewTabPage.Controls.Add(this.line3);
            this.overviewTabPage.Controls.Add(this.newestPackageLabel);
            this.overviewTabPage.Controls.Add(this.amountLabel);
            this.overviewTabPage.Controls.Add(this.checkUpdateConfigurationLinkLabel);
            this.overviewTabPage.Controls.Add(this.infoFileloadingLabel);
            this.overviewTabPage.Controls.Add(this.ftpDirectoryTextBox);
            this.overviewTabPage.Controls.Add(this.ftpHostTextBox);
            this.overviewTabPage.Controls.Add(this.updateUrlTextBox);
            this.overviewTabPage.Controls.Add(this.nameTextBox);
            this.overviewTabPage.Controls.Add(this.nameLabel);
            this.overviewTabPage.Controls.Add(this.updateUrlLabel);
            this.overviewTabPage.Controls.Add(this.projectIdTextBox);
            this.overviewTabPage.Controls.Add(this.projectIdLabel);
            this.overviewTabPage.Controls.Add(this.publicKeyTextBox);
            this.overviewTabPage.Controls.Add(this.publicKeyLabel);
            this.overviewTabPage.Controls.Add(this.line2);
            this.overviewTabPage.Controls.Add(this.projectDataHeader);
            this.overviewTabPage.Controls.Add(this.line1);
            this.overviewTabPage.Controls.Add(this.ftpDirectoryLabel);
            this.overviewTabPage.Controls.Add(this.ftpHostLabel);
            this.overviewTabPage.Controls.Add(this.overviewHeader);
            this.overviewTabPage.Controls.Add(this.newestPackageReleasedLabel);
            this.overviewTabPage.Controls.Add(this.releasedPackagesAmountLabel);
            this.overviewTabPage.Controls.Add(this.line4);
            this.overviewTabPage.Controls.Add(this.statisticsServerPanel);
            this.overviewTabPage.Controls.Add(this.panel2);
            this.overviewTabPage.Controls.Add(this.panel3);
            this.overviewTabPage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overviewTabPage.ImageIndex = 1;
            this.overviewTabPage.Location = new System.Drawing.Point(4, 23);
            this.overviewTabPage.Name = "overviewTabPage";
            this.overviewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.overviewTabPage.Size = new System.Drawing.Size(566, 386);
            this.overviewTabPage.TabIndex = 1;
            this.overviewTabPage.Text = "Overview";
            this.overviewTabPage.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(44, 521);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "Path:";
            // 
            // line6
            // 
            this.line6.BackColor = System.Drawing.Color.White;
            this.line6.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line6.Location = new System.Drawing.Point(95, 557);
            this.line6.Name = "line6";
            this.line6.Size = new System.Drawing.Size(435, 12);
            this.line6.TabIndex = 53;
            this.line6.Text = "line6";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(20, 550);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 20);
            this.label4.TabIndex = 52;
            this.label4.Text = "Statistics";
            // 
            // browseAssemblyButton
            // 
            this.browseAssemblyButton.Enabled = false;
            this.browseAssemblyButton.Location = new System.Drawing.Point(489, 515);
            this.browseAssemblyButton.Name = "browseAssemblyButton";
            this.browseAssemblyButton.Size = new System.Drawing.Size(31, 24);
            this.browseAssemblyButton.TabIndex = 50;
            this.browseAssemblyButton.Text = "...";
            this.browseAssemblyButton.UseVisualStyleBackColor = true;
            this.browseAssemblyButton.Click += new System.EventHandler(this.browseAssemblyButton_Click);
            // 
            // assemblyPathTextBox
            // 
            this.assemblyPathTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.assemblyPathTextBox.Enabled = false;
            this.assemblyPathTextBox.Location = new System.Drawing.Point(95, 516);
            this.assemblyPathTextBox.Name = "assemblyPathTextBox";
            this.assemblyPathTextBox.ReadOnly = true;
            this.assemblyPathTextBox.Size = new System.Drawing.Size(389, 22);
            this.assemblyPathTextBox.TabIndex = 47;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 419);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(494, 26);
            this.label2.TabIndex = 44;
            this.label2.Text = "Set if the version should be entered manually or loaded from an assembly when add" +
    "ing a new \r\npackage. ";
            // 
            // line5
            // 
            this.line5.BackColor = System.Drawing.Color.White;
            this.line5.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line5.Location = new System.Drawing.Point(132, 390);
            this.line5.Name = "line5";
            this.line5.Size = new System.Drawing.Size(398, 13);
            this.line5.TabIndex = 43;
            this.line5.Text = "line5";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(20, 383);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 20);
            this.label3.TabIndex = 42;
            this.label3.Text = "Version linking";
            // 
            // stepTwoLabel
            // 
            this.stepTwoLabel.AutoSize = true;
            this.stepTwoLabel.Location = new System.Drawing.Point(26, 847);
            this.stepTwoLabel.Name = "stepTwoLabel";
            this.stepTwoLabel.Size = new System.Drawing.Size(318, 13);
            this.stepTwoLabel.TabIndex = 41;
            this.stepTwoLabel.Text = "2. Click \"{0}\" and paste the copied code into your app\'s code.";
            // 
            // programmingLanguageComboBox
            // 
            this.programmingLanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.programmingLanguageComboBox.FormattingEnabled = true;
            this.programmingLanguageComboBox.Location = new System.Drawing.Point(383, 813);
            this.programmingLanguageComboBox.Name = "programmingLanguageComboBox";
            this.programmingLanguageComboBox.Size = new System.Drawing.Size(139, 21);
            this.programmingLanguageComboBox.TabIndex = 40;
            // 
            // stepOneLabel
            // 
            this.stepOneLabel.AutoSize = true;
            this.stepOneLabel.Location = new System.Drawing.Point(26, 816);
            this.stepOneLabel.Name = "stepOneLabel";
            this.stepOneLabel.Size = new System.Drawing.Size(318, 13);
            this.stepOneLabel.TabIndex = 39;
            this.stepOneLabel.Text = "1. Please choose the programming language of your project:\r\n";
            // 
            // assumeInfoLabel
            // 
            this.assumeInfoLabel.AutoSize = true;
            this.assumeInfoLabel.Location = new System.Drawing.Point(26, 775);
            this.assumeInfoLabel.Name = "assumeInfoLabel";
            this.assumeInfoLabel.Size = new System.Drawing.Size(502, 26);
            this.assumeInfoLabel.TabIndex = 38;
            this.assumeInfoLabel.Text = "If you have problems implementing the code for the client in your application the" +
    "n follow these \r\nsteps:";
            // 
            // copySourceButton
            // 
            this.copySourceButton.Location = new System.Drawing.Point(410, 841);
            this.copySourceButton.Name = "copySourceButton";
            this.copySourceButton.Size = new System.Drawing.Size(112, 23);
            this.copySourceButton.TabIndex = 37;
            this.copySourceButton.Text = "Copy source";
            this.copySourceButton.UseVisualStyleBackColor = true;
            this.copySourceButton.Click += new System.EventHandler(this.copySourceButton_Click);
            // 
            // assumeHeader
            // 
            this.assumeHeader.AutoSize = true;
            this.assumeHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.assumeHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.assumeHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.assumeHeader.Location = new System.Drawing.Point(20, 744);
            this.assumeHeader.Name = "assumeHeader";
            this.assumeHeader.Size = new System.Drawing.Size(77, 20);
            this.assumeHeader.TabIndex = 35;
            this.assumeHeader.Text = "Copy data";
            // 
            // editFtpButton
            // 
            this.editFtpButton.Location = new System.Drawing.Point(297, 354);
            this.editFtpButton.Name = "editFtpButton";
            this.editFtpButton.Size = new System.Drawing.Size(109, 23);
            this.editFtpButton.TabIndex = 34;
            this.editFtpButton.Text = "Edit FTP-data";
            this.editFtpButton.UseVisualStyleBackColor = true;
            this.editFtpButton.Click += new System.EventHandler(this.editFtpButton_Click);
            // 
            // editProjectButton
            // 
            this.editProjectButton.Location = new System.Drawing.Point(410, 354);
            this.editProjectButton.Name = "editProjectButton";
            this.editProjectButton.Size = new System.Drawing.Size(111, 23);
            this.editProjectButton.TabIndex = 33;
            this.editProjectButton.Text = "Edit project-data";
            this.editProjectButton.UseVisualStyleBackColor = true;
            this.editProjectButton.Click += new System.EventHandler(this.editProjectButton_Click);
            // 
            // checkingUrlPictureBox
            // 
            this.checkingUrlPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("checkingUrlPictureBox.Image")));
            this.checkingUrlPictureBox.Location = new System.Drawing.Point(308, 233);
            this.checkingUrlPictureBox.Name = "checkingUrlPictureBox";
            this.checkingUrlPictureBox.Size = new System.Drawing.Size(16, 16);
            this.checkingUrlPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.checkingUrlPictureBox.TabIndex = 31;
            this.checkingUrlPictureBox.TabStop = false;
            this.checkingUrlPictureBox.Visible = false;
            // 
            // tickPictureBox
            // 
            this.tickPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("tickPictureBox.Image")));
            this.tickPictureBox.Location = new System.Drawing.Point(308, 233);
            this.tickPictureBox.Name = "tickPictureBox";
            this.tickPictureBox.Size = new System.Drawing.Size(16, 16);
            this.tickPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tickPictureBox.TabIndex = 30;
            this.tickPictureBox.TabStop = false;
            this.tickPictureBox.Visible = false;
            // 
            // line3
            // 
            this.line3.BackColor = System.Drawing.Color.White;
            this.line3.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line3.Location = new System.Drawing.Point(25, 167);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(505, 10);
            this.line3.TabIndex = 29;
            this.line3.Text = "line3";
            // 
            // newestPackageLabel
            // 
            this.newestPackageLabel.AutoSize = true;
            this.newestPackageLabel.Location = new System.Drawing.Point(232, 210);
            this.newestPackageLabel.Name = "newestPackageLabel";
            this.newestPackageLabel.Size = new System.Drawing.Size(26, 13);
            this.newestPackageLabel.TabIndex = 28;
            this.newestPackageLabel.Text = "N/A";
            // 
            // amountLabel
            // 
            this.amountLabel.AutoSize = true;
            this.amountLabel.Location = new System.Drawing.Point(232, 186);
            this.amountLabel.Name = "amountLabel";
            this.amountLabel.Size = new System.Drawing.Size(26, 13);
            this.amountLabel.TabIndex = 27;
            this.amountLabel.Text = "N/A";
            // 
            // checkUpdateConfigurationLinkLabel
            // 
            this.checkUpdateConfigurationLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.checkUpdateConfigurationLinkLabel.AutoSize = true;
            this.checkUpdateConfigurationLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.checkUpdateConfigurationLinkLabel.Location = new System.Drawing.Point(232, 234);
            this.checkUpdateConfigurationLinkLabel.Name = "checkUpdateConfigurationLinkLabel";
            this.checkUpdateConfigurationLinkLabel.Size = new System.Drawing.Size(75, 13);
            this.checkUpdateConfigurationLinkLabel.TabIndex = 26;
            this.checkUpdateConfigurationLinkLabel.TabStop = true;
            this.checkUpdateConfigurationLinkLabel.Text = "Check status ";
            this.checkUpdateConfigurationLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.checkUpdateConfigurationLinkLabel_LinkClicked);
            // 
            // infoFileloadingLabel
            // 
            this.infoFileloadingLabel.AutoSize = true;
            this.infoFileloadingLabel.Location = new System.Drawing.Point(24, 234);
            this.infoFileloadingLabel.Name = "infoFileloadingLabel";
            this.infoFileloadingLabel.Size = new System.Drawing.Size(160, 13);
            this.infoFileloadingLabel.TabIndex = 24;
            this.infoFileloadingLabel.Text = "Status of the update-info file:";
            // 
            // ftpDirectoryTextBox
            // 
            this.ftpDirectoryTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ftpDirectoryTextBox.Location = new System.Drawing.Point(235, 136);
            this.ftpDirectoryTextBox.Name = "ftpDirectoryTextBox";
            this.ftpDirectoryTextBox.ReadOnly = true;
            this.ftpDirectoryTextBox.Size = new System.Drawing.Size(285, 22);
            this.ftpDirectoryTextBox.TabIndex = 23;
            // 
            // ftpHostTextBox
            // 
            this.ftpHostTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ftpHostTextBox.Location = new System.Drawing.Point(235, 108);
            this.ftpHostTextBox.Name = "ftpHostTextBox";
            this.ftpHostTextBox.ReadOnly = true;
            this.ftpHostTextBox.Size = new System.Drawing.Size(285, 22);
            this.ftpHostTextBox.TabIndex = 22;
            // 
            // updateUrlTextBox
            // 
            this.updateUrlTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.updateUrlTextBox.Location = new System.Drawing.Point(235, 80);
            this.updateUrlTextBox.Name = "updateUrlTextBox";
            this.updateUrlTextBox.ReadOnly = true;
            this.updateUrlTextBox.Size = new System.Drawing.Size(285, 22);
            this.updateUrlTextBox.TabIndex = 21;
            // 
            // nameTextBox
            // 
            this.nameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.nameTextBox.Location = new System.Drawing.Point(235, 52);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ReadOnly = true;
            this.nameTextBox.Size = new System.Drawing.Size(285, 22);
            this.nameTextBox.TabIndex = 20;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(24, 55);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 13);
            this.nameLabel.TabIndex = 19;
            this.nameLabel.Text = "Name:";
            // 
            // updateUrlLabel
            // 
            this.updateUrlLabel.AutoSize = true;
            this.updateUrlLabel.Location = new System.Drawing.Point(24, 83);
            this.updateUrlLabel.Name = "updateUrlLabel";
            this.updateUrlLabel.Size = new System.Drawing.Size(72, 13);
            this.updateUrlLabel.TabIndex = 18;
            this.updateUrlLabel.Text = "Update-URL:";
            // 
            // projectIdTextBox
            // 
            this.projectIdTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.projectIdTextBox.Location = new System.Drawing.Point(235, 326);
            this.projectIdTextBox.Name = "projectIdTextBox";
            this.projectIdTextBox.ReadOnly = true;
            this.projectIdTextBox.Size = new System.Drawing.Size(285, 22);
            this.projectIdTextBox.TabIndex = 17;
            // 
            // projectIdLabel
            // 
            this.projectIdLabel.AutoSize = true;
            this.projectIdLabel.Location = new System.Drawing.Point(24, 329);
            this.projectIdLabel.Name = "projectIdLabel";
            this.projectIdLabel.Size = new System.Drawing.Size(60, 13);
            this.projectIdLabel.TabIndex = 16;
            this.projectIdLabel.Text = "Project-ID:";
            // 
            // publicKeyTextBox
            // 
            this.publicKeyTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.publicKeyTextBox.Location = new System.Drawing.Point(235, 298);
            this.publicKeyTextBox.Name = "publicKeyTextBox";
            this.publicKeyTextBox.ReadOnly = true;
            this.publicKeyTextBox.Size = new System.Drawing.Size(285, 22);
            this.publicKeyTextBox.TabIndex = 15;
            // 
            // publicKeyLabel
            // 
            this.publicKeyLabel.AutoSize = true;
            this.publicKeyLabel.Location = new System.Drawing.Point(24, 301);
            this.publicKeyLabel.Name = "publicKeyLabel";
            this.publicKeyLabel.Size = new System.Drawing.Size(61, 13);
            this.publicKeyLabel.TabIndex = 14;
            this.publicKeyLabel.Text = "Public key:";
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.White;
            this.line2.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line2.Location = new System.Drawing.Point(110, 270);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(420, 13);
            this.line2.TabIndex = 13;
            this.line2.Text = "line2";
            // 
            // projectDataHeader
            // 
            this.projectDataHeader.AutoSize = true;
            this.projectDataHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.projectDataHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.projectDataHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.projectDataHeader.Location = new System.Drawing.Point(20, 263);
            this.projectDataHeader.Name = "projectDataHeader";
            this.projectDataHeader.Size = new System.Drawing.Size(91, 20);
            this.projectDataHeader.TabIndex = 12;
            this.projectDataHeader.Text = "Project-data";
            // 
            // line1
            // 
            this.line1.BackColor = System.Drawing.Color.White;
            this.line1.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(144, 24);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(386, 13);
            this.line1.TabIndex = 11;
            this.line1.Text = "line1";
            // 
            // ftpDirectoryLabel
            // 
            this.ftpDirectoryLabel.AutoSize = true;
            this.ftpDirectoryLabel.Location = new System.Drawing.Point(24, 139);
            this.ftpDirectoryLabel.Name = "ftpDirectoryLabel";
            this.ftpDirectoryLabel.Size = new System.Drawing.Size(77, 13);
            this.ftpDirectoryLabel.TabIndex = 10;
            this.ftpDirectoryLabel.Text = "FTP-Directory:";
            // 
            // ftpHostLabel
            // 
            this.ftpHostLabel.AutoSize = true;
            this.ftpHostLabel.Location = new System.Drawing.Point(24, 111);
            this.ftpHostLabel.Name = "ftpHostLabel";
            this.ftpHostLabel.Size = new System.Drawing.Size(55, 13);
            this.ftpHostLabel.TabIndex = 9;
            this.ftpHostLabel.Text = "FTP-Host:";
            // 
            // overviewHeader
            // 
            this.overviewHeader.AutoSize = true;
            this.overviewHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.overviewHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.overviewHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.overviewHeader.Location = new System.Drawing.Point(20, 17);
            this.overviewHeader.Name = "overviewHeader";
            this.overviewHeader.Size = new System.Drawing.Size(124, 20);
            this.overviewHeader.TabIndex = 8;
            this.overviewHeader.Text = "Project-overview ";
            // 
            // newestPackageReleasedLabel
            // 
            this.newestPackageReleasedLabel.AutoSize = true;
            this.newestPackageReleasedLabel.Location = new System.Drawing.Point(24, 210);
            this.newestPackageReleasedLabel.Name = "newestPackageReleasedLabel";
            this.newestPackageReleasedLabel.Size = new System.Drawing.Size(140, 13);
            this.newestPackageReleasedLabel.TabIndex = 7;
            this.newestPackageReleasedLabel.Text = "Newest package released:";
            // 
            // releasedPackagesAmountLabel
            // 
            this.releasedPackagesAmountLabel.AutoSize = true;
            this.releasedPackagesAmountLabel.Location = new System.Drawing.Point(24, 186);
            this.releasedPackagesAmountLabel.Name = "releasedPackagesAmountLabel";
            this.releasedPackagesAmountLabel.Size = new System.Drawing.Size(162, 13);
            this.releasedPackagesAmountLabel.TabIndex = 6;
            this.releasedPackagesAmountLabel.Text = "Amount of packages released:";
            // 
            // line4
            // 
            this.line4.BackColor = System.Drawing.Color.White;
            this.line4.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line4.Location = new System.Drawing.Point(98, 751);
            this.line4.Name = "line4";
            this.line4.Size = new System.Drawing.Size(432, 10);
            this.line4.TabIndex = 36;
            this.line4.Text = "line4";
            // 
            // statisticsServerPanel
            // 
            this.statisticsServerPanel.Controls.Add(this.textBox1);
            this.statisticsServerPanel.Controls.Add(this.label1);
            this.statisticsServerPanel.Controls.Add(this.selectStatisticsServerButton);
            this.statisticsServerPanel.Controls.Add(this.dataBaseLabel);
            this.statisticsServerPanel.Enabled = false;
            this.statisticsServerPanel.Location = new System.Drawing.Point(31, 633);
            this.statisticsServerPanel.Name = "statisticsServerPanel";
            this.statisticsServerPanel.Size = new System.Drawing.Size(305, 96);
            this.statisticsServerPanel.TabIndex = 57;
            // 
            // selectStatisticsServerButton
            // 
            this.selectStatisticsServerButton.Location = new System.Drawing.Point(16, 61);
            this.selectStatisticsServerButton.Name = "selectStatisticsServerButton";
            this.selectStatisticsServerButton.Size = new System.Drawing.Size(147, 24);
            this.selectStatisticsServerButton.TabIndex = 56;
            this.selectStatisticsServerButton.Text = "Select a statistics server";
            this.selectStatisticsServerButton.UseVisualStyleBackColor = true;
            this.selectStatisticsServerButton.Click += new System.EventHandler(this.selectStatisticsServerButton_Click);
            // 
            // dataBaseLabel
            // 
            this.dataBaseLabel.Location = new System.Drawing.Point(13, 7);
            this.dataBaseLabel.Name = "dataBaseLabel";
            this.dataBaseLabel.Size = new System.Drawing.Size(185, 19);
            this.dataBaseLabel.TabIndex = 54;
            this.dataBaseLabel.Text = "Database: -";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.loadFromAssemblyRadioButton);
            this.panel2.Controls.Add(this.enterVersionManuallyRadioButton);
            this.panel2.Location = new System.Drawing.Point(18, 452);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(191, 59);
            this.panel2.TabIndex = 64;
            // 
            // loadFromAssemblyRadioButton
            // 
            this.loadFromAssemblyRadioButton.AutoSize = true;
            this.loadFromAssemblyRadioButton.Location = new System.Drawing.Point(9, 32);
            this.loadFromAssemblyRadioButton.Name = "loadFromAssemblyRadioButton";
            this.loadFromAssemblyRadioButton.Size = new System.Drawing.Size(169, 17);
            this.loadFromAssemblyRadioButton.TabIndex = 60;
            this.loadFromAssemblyRadioButton.Text = "Load version from assembly:";
            this.loadFromAssemblyRadioButton.UseVisualStyleBackColor = true;
            this.loadFromAssemblyRadioButton.CheckedChanged += new System.EventHandler(this.loadFromAssemblyRadioButton_CheckedChanged);
            // 
            // enterVersionManuallyRadioButton
            // 
            this.enterVersionManuallyRadioButton.AutoSize = true;
            this.enterVersionManuallyRadioButton.Checked = true;
            this.enterVersionManuallyRadioButton.Location = new System.Drawing.Point(9, 9);
            this.enterVersionManuallyRadioButton.Name = "enterVersionManuallyRadioButton";
            this.enterVersionManuallyRadioButton.Size = new System.Drawing.Size(141, 17);
            this.enterVersionManuallyRadioButton.TabIndex = 59;
            this.enterVersionManuallyRadioButton.TabStop = true;
            this.enterVersionManuallyRadioButton.Text = "Enter version manually";
            this.enterVersionManuallyRadioButton.UseVisualStyleBackColor = true;
            this.enterVersionManuallyRadioButton.CheckedChanged += new System.EventHandler(this.enterVersionManuallyRadioButton_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.useStatisticsServerRadioButton);
            this.panel3.Controls.Add(this.doNotUseStatisticsServerRadioButton);
            this.panel3.Location = new System.Drawing.Point(18, 580);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(192, 52);
            this.panel3.TabIndex = 65;
            // 
            // useStatisticsServerRadioButton
            // 
            this.useStatisticsServerRadioButton.AutoSize = true;
            this.useStatisticsServerRadioButton.Location = new System.Drawing.Point(8, 28);
            this.useStatisticsServerRadioButton.Name = "useStatisticsServerRadioButton";
            this.useStatisticsServerRadioButton.Size = new System.Drawing.Size(133, 17);
            this.useStatisticsServerRadioButton.TabIndex = 63;
            this.useStatisticsServerRadioButton.Text = "Use a statistics server";
            this.useStatisticsServerRadioButton.UseVisualStyleBackColor = true;
            this.useStatisticsServerRadioButton.CheckedChanged += new System.EventHandler(this.useStatisticsServerRadioButton_CheckedChanged);
            // 
            // doNotUseStatisticsServerRadioButton
            // 
            this.doNotUseStatisticsServerRadioButton.AutoSize = true;
            this.doNotUseStatisticsServerRadioButton.Checked = true;
            this.doNotUseStatisticsServerRadioButton.Location = new System.Drawing.Point(8, 5);
            this.doNotUseStatisticsServerRadioButton.Name = "doNotUseStatisticsServerRadioButton";
            this.doNotUseStatisticsServerRadioButton.Size = new System.Drawing.Size(164, 17);
            this.doNotUseStatisticsServerRadioButton.TabIndex = 62;
            this.doNotUseStatisticsServerRadioButton.TabStop = true;
            this.doNotUseStatisticsServerRadioButton.Text = "Don\'t use a statistics server";
            this.doNotUseStatisticsServerRadioButton.UseVisualStyleBackColor = true;
            this.doNotUseStatisticsServerRadioButton.CheckedChanged += new System.EventHandler(this.doNotUseStatisticsServerRadioButton_CheckedChanged);
            // 
            // packagesTabPage
            // 
            this.packagesTabPage.Controls.Add(this.packagesList);
            this.packagesTabPage.Controls.Add(this.toolStrip1);
            this.packagesTabPage.Controls.Add(this.searchTextBox);
            this.packagesTabPage.ImageIndex = 0;
            this.packagesTabPage.Location = new System.Drawing.Point(4, 23);
            this.packagesTabPage.Name = "packagesTabPage";
            this.packagesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.packagesTabPage.Size = new System.Drawing.Size(566, 386);
            this.packagesTabPage.TabIndex = 0;
            this.packagesTabPage.Text = "Packages";
            this.packagesTabPage.UseVisualStyleBackColor = true;
            // 
            // packagesList
            // 
            this.packagesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.packagesList.FullRowSelect = true;
            listViewGroup1.Header = "Released";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "Not released";
            listViewGroup2.Name = "listViewGroup2";
            this.packagesList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.packagesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.packagesList.Location = new System.Drawing.Point(4, 31);
            this.packagesList.MultiSelect = false;
            this.packagesList.Name = "packagesList";
            this.packagesList.Size = new System.Drawing.Size(558, 351);
            this.packagesList.TabIndex = 3;
            this.packagesList.UseCompatibleStateImageBehavior = false;
            this.packagesList.View = System.Windows.Forms.View.Details;
            this.packagesList.SelectedIndexChanged += new System.EventHandler(this.packagesList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Version";
            this.columnHeader1.Width = 116;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Released";
            this.columnHeader2.Width = 148;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Size";
            this.columnHeader3.Width = 95;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Description";
            this.columnHeader4.Width = 194;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addButton,
            this.toolStripSeparator1,
            this.editButton,
            this.toolStripSeparator2,
            this.deleteButton,
            this.toolStripSeparator3,
            this.uploadButton,
            this.toolStripSeparator4,
            this.historyButton,
            this.toolStripSeparator5});
            this.toolStrip1.Location = new System.Drawing.Point(2, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(424, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addButton
            // 
            this.addButton.AutoToolTip = false;
            this.addButton.Image = ((System.Drawing.Image)(resources.GetObject("addButton.Image")));
            this.addButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(52, 22);
            this.addButton.Text = "Add ";
            this.addButton.ToolTipText = "Add a new update package that contains updated files/folders.";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // editButton
            // 
            this.editButton.AutoToolTip = false;
            this.editButton.Enabled = false;
            this.editButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.editButton.Image = ((System.Drawing.Image)(resources.GetObject("editButton.Image")));
            this.editButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(50, 22);
            this.editButton.Text = "Edit ";
            this.editButton.ToolTipText = "Edit the configuration of the selected package.";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // deleteButton
            // 
            this.deleteButton.AutoToolTip = false;
            this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(63, 22);
            this.deleteButton.Text = "Delete ";
            this.deleteButton.ToolTipText = "Delete the selected package from the server and locally.";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // uploadButton
            // 
            this.uploadButton.AutoToolTip = false;
            this.uploadButton.Enabled = false;
            this.uploadButton.Image = ((System.Drawing.Image)(resources.GetObject("uploadButton.Image")));
            this.uploadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(68, 22);
            this.uploadButton.Text = "Upload ";
            this.uploadButton.ToolTipText = "Upload the selected package to the FTP-server to make it available for all client" +
    "s.";
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // historyButton
            // 
            this.historyButton.AutoToolTip = false;
            this.historyButton.Image = ((System.Drawing.Image)(resources.GetObject("historyButton.Image")));
            this.historyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.historyButton.Name = "historyButton";
            this.historyButton.Size = new System.Drawing.Size(65, 22);
            this.historyButton.Text = "History";
            this.historyButton.ToolTipText = "Load the history for the packages.";
            this.historyButton.Click += new System.EventHandler(this.historyButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Cue = null;
            this.searchTextBox.Location = new System.Drawing.Point(429, 4);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(132, 22);
            this.searchTextBox.TabIndex = 2;
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyDown);
            // 
            // statisticsTabPage
            // 
            this.statisticsTabPage.Controls.Add(this.statisticsDataGridView);
            this.statisticsTabPage.Controls.Add(this.statusStrip1);
            this.statisticsTabPage.Controls.Add(this.toolStrip2);
            this.statisticsTabPage.Controls.Add(this.noStatisticsPanel);
            this.statisticsTabPage.ImageIndex = 3;
            this.statisticsTabPage.Location = new System.Drawing.Point(4, 23);
            this.statisticsTabPage.Name = "statisticsTabPage";
            this.statisticsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.statisticsTabPage.Size = new System.Drawing.Size(566, 386);
            this.statisticsTabPage.TabIndex = 2;
            this.statisticsTabPage.Text = "Statistics";
            this.statisticsTabPage.UseVisualStyleBackColor = true;
            // 
            // statisticsDataGridView
            // 
            this.statisticsDataGridView.AllowUserToAddRows = false;
            this.statisticsDataGridView.AllowUserToDeleteRows = false;
            this.statisticsDataGridView.AllowUserToOrderColumns = true;
            this.statisticsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.statisticsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.statisticsDataGridView.Location = new System.Drawing.Point(3, 31);
            this.statisticsDataGridView.Name = "statisticsDataGridView";
            this.statisticsDataGridView.ReadOnly = true;
            this.statisticsDataGridView.RowHeadersWidth = 240;
            this.statisticsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.statisticsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.statisticsDataGridView.ShowEditingIcon = false;
            this.statisticsDataGridView.Size = new System.Drawing.Size(560, 326);
            this.statisticsDataGridView.TabIndex = 88;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Window;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(3, 361);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(560, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 90;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(115, 17);
            this.toolStripStatusLabel1.Text = "Total downloads: {0}";
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.Window;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator6,
            this.lastUpdatedLabel,
            this.toolStripButton2,
            this.toolStripSeparator7});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(560, 25);
            this.toolStrip2.TabIndex = 89;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(92, 22);
            this.toolStripButton1.Text = "Update stats";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // lastUpdatedLabel
            // 
            this.lastUpdatedLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lastUpdatedLabel.Name = "lastUpdatedLabel";
            this.lastUpdatedLabel.Size = new System.Drawing.Size(81, 22);
            this.lastUpdatedLabel.Text = "Last updated: ";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(78, 22);
            this.toolStripButton2.Text = "Save stats";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // noStatisticsPanel
            // 
            this.noStatisticsPanel.Controls.Add(this.pictureBox2);
            this.noStatisticsPanel.Controls.Add(this.label5);
            this.noStatisticsPanel.Location = new System.Drawing.Point(185, 150);
            this.noStatisticsPanel.Name = "noStatisticsPanel";
            this.noStatisticsPanel.Size = new System.Drawing.Size(202, 42);
            this.noStatisticsPanel.TabIndex = 2;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(13, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(24, 24);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 26);
            this.label5.TabIndex = 0;
            this.label5.Text = "This project was not linked \r\nwith a statistics server.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "Password:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(78, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(215, 22);
            this.textBox1.TabIndex = 58;
            // 
            // ProjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(572, 412);
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProjectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{0} - nUpdate Administration 1.1.0.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectDialog_FormClosing);
            this.Load += new System.EventHandler(this.ProjectDialog_Load);
            this.Shown += new System.EventHandler(this.ProjectDialog_Shown);
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.overviewTabPage.ResumeLayout(false);
            this.overviewTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkingUrlPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tickPictureBox)).EndInit();
            this.statisticsServerPanel.ResumeLayout(false);
            this.statisticsServerPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.packagesTabPage.ResumeLayout(false);
            this.packagesTabPage.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statisticsTabPage.ResumeLayout(false);
            this.statisticsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statisticsDataGridView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.noStatisticsPanel.ResumeLayout(false);
            this.noStatisticsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton editButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private WatermarkTextBox searchTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton uploadButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton historyButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage overviewTabPage;
        private System.Windows.Forms.TabPage packagesTabPage;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label newestPackageReleasedLabel;
        private System.Windows.Forms.Label releasedPackagesAmountLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label updateUrlLabel;
        private System.Windows.Forms.TextBox projectIdTextBox;
        private System.Windows.Forms.Label projectIdLabel;
        private System.Windows.Forms.TextBox publicKeyTextBox;
        private System.Windows.Forms.Label publicKeyLabel;
        private Controls.Line line2;
        private System.Windows.Forms.Label projectDataHeader;
        private Controls.Line line1;
        private System.Windows.Forms.Label ftpDirectoryLabel;
        private System.Windows.Forms.Label ftpHostLabel;
        private System.Windows.Forms.Label overviewHeader;
        private System.Windows.Forms.TextBox updateUrlTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox ftpDirectoryTextBox;
        private System.Windows.Forms.TextBox ftpHostTextBox;
        private System.Windows.Forms.Label infoFileloadingLabel;
        private System.Windows.Forms.Label newestPackageLabel;
        private System.Windows.Forms.Label amountLabel;
        private System.Windows.Forms.LinkLabel checkUpdateConfigurationLinkLabel;
        private Controls.Line line3;
        private System.Windows.Forms.PictureBox tickPictureBox;
        private System.Windows.Forms.PictureBox checkingUrlPictureBox;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button editFtpButton;
        private System.Windows.Forms.Button editProjectButton;
        private System.Windows.Forms.Label assumeHeader;
        private Controls.Line line4;
        private System.Windows.Forms.Button copySourceButton;
        private System.Windows.Forms.Label stepTwoLabel;
        private System.Windows.Forms.ComboBox programmingLanguageComboBox;
        private System.Windows.Forms.Label stepOneLabel;
        private System.Windows.Forms.Label assumeInfoLabel;
        private Controls.ExtendedListView packagesList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button browseAssemblyButton;
        private System.Windows.Forms.TextBox assemblyPathTextBox;
        private System.Windows.Forms.Label label2;
        private Controls.Line line5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button selectStatisticsServerButton;
        private System.Windows.Forms.Label dataBaseLabel;
        private Controls.Line line6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage statisticsTabPage;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel noStatisticsPanel;
        private System.Windows.Forms.DataGridView statisticsDataGridView;
        private System.Windows.Forms.Panel statisticsServerPanel;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel lastUpdatedLabel;
        private System.Windows.Forms.RadioButton useStatisticsServerRadioButton;
        private System.Windows.Forms.RadioButton doNotUseStatisticsServerRadioButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton loadFromAssemblyRadioButton;
        private System.Windows.Forms.RadioButton enterVersionManuallyRadioButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
    }
}