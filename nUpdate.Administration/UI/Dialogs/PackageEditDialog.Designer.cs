using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class PackageEditDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageEditDialog));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General", 2, 2);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Changelog", 3, 3);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Availability", 0, 0);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Operations", 4, 4);
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Registry", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Processes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Services", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Scripts", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rename file",
            "Renames a given file to the new name set."}, 10);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Delete file",
            "Deletes a given file."}, 9);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Create sub key",
            "Creates a sub key in the registry."}, 14);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Delete sub key",
            "Deletes a sub key recursively in the registry."}, 12);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Set value",
            "Creates or edits a name-value-pair in the registry."}, 13);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "Delete name-value-pair",
            "Deletes a name-value-pair in the registry."}, 12);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "Start process",
            "Starts a given process."}, 8);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "Terminate process",
            "Terminates a given process if possible."}, 7);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "Start service.",
            "Starts a windows service. If it is already running it will be restarted. "}, 5);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "Stop service.",
            "Stops a running windows service."}, 6);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "Execute script",
            "Executes a C#-script."}, 15);
            this.categoryImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.cancelLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.categoryTreeView = new nUpdate.Administration.UI.Controls.ExplorerTreeView();
            this.controlPanel1 = new BottomPanel();
            this.categoryTabControl = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.generalPanel = new System.Windows.Forms.Panel();
            this.includeIntoStatisticsInfoLabel = new System.Windows.Forms.Label();
            this.includeIntoStatisticsCheckBox = new System.Windows.Forms.CheckBox();
            this.line2 = new Line();
            this.developmentBuildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.necessaryUpdateInfoLabel = new System.Windows.Forms.Label();
            this.necessaryUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.line3 = new Line();
            this.line1 = new Line();
            this.architectureInfoLabel = new System.Windows.Forms.Label();
            this.buildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.environmentLabel = new System.Windows.Forms.Label();
            this.majorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.architectureComboBox = new System.Windows.Forms.ComboBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.revisionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.versionLabel = new System.Windows.Forms.Label();
            this.developmentalStageComboBox = new System.Windows.Forms.ComboBox();
            this.devStageLabel = new System.Windows.Forms.Label();
            this.minorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.changelogTabPage = new System.Windows.Forms.TabPage();
            this.changelogPanel = new System.Windows.Forms.Panel();
            this.changelogContentTabControl = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.englishChangelogTabPage = new System.Windows.Forms.TabPage();
            this.englishChangelogTextBox = new System.Windows.Forms.TextBox();
            this.changelogToolStrip = new System.Windows.Forms.ToolStrip();
            this.changelogLanguageComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.changelogLoadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.changelogClearButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.specialCharactersComboBox = new System.Windows.Forms.ToolStripDropDownButton();
            this.bulletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.insideQuotationMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classicQuotationMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outsideQuotationMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.apostropheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.copyrightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allRightsReservedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundRecordingCopyrightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unregisteredTrademarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.availabilityTabPage = new System.Windows.Forms.TabPage();
            this.availabilityPanel = new System.Windows.Forms.Panel();
            this.unsupportedVersionsPanel = new System.Windows.Forms.Panel();
            this.unsupportedMajorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.addVersionButton = new System.Windows.Forms.Button();
            this.removeVersionButton = new System.Windows.Forms.Button();
            this.unsupportedRevisionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.unsupportedVersionsListBox = new System.Windows.Forms.ListBox();
            this.unsupportedMinorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.unsupportedBuildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.someVersionsInfoLabel = new System.Windows.Forms.Label();
            this.allVersionsRadioButton = new System.Windows.Forms.RadioButton();
            this.allVersionsInfoLabel = new System.Windows.Forms.Label();
            this.someVersionsRadioButton = new System.Windows.Forms.RadioButton();
            this.operationsTabPage = new System.Windows.Forms.TabPage();
            this.operationsPanel = new System.Windows.Forms.Panel();
            this.operationsListView = new nUpdate.Administration.UI.Controls.ExplorerListView();
            this.itemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.categoryTabControl.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.generalPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.developmentBuildNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.revisionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minorNumericUpDown)).BeginInit();
            this.changelogTabPage.SuspendLayout();
            this.changelogPanel.SuspendLayout();
            this.changelogContentTabControl.SuspendLayout();
            this.englishChangelogTabPage.SuspendLayout();
            this.changelogToolStrip.SuspendLayout();
            this.availabilityTabPage.SuspendLayout();
            this.availabilityPanel.SuspendLayout();
            this.unsupportedVersionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMajorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedRevisionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMinorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedBuildNumericUpDown)).BeginInit();
            this.operationsTabPage.SuspendLayout();
            this.operationsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // categoryImageList
            // 
            this.categoryImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("categoryImageList.ImageStream")));
            this.categoryImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.categoryImageList.Images.SetKeyName(0, "arrow-switch.png");
            this.categoryImageList.Images.SetKeyName(1, "blue-documents-stack.png");
            this.categoryImageList.Images.SetKeyName(2, "property.png");
            this.categoryImageList.Images.SetKeyName(3, "document--pencil.png");
            this.categoryImageList.Images.SetKeyName(4, "node-delete-previous.png");
            this.categoryImageList.Images.SetKeyName(5, "sofa--arrow.png");
            this.categoryImageList.Images.SetKeyName(6, "sofa--minus.png");
            this.categoryImageList.Images.SetKeyName(7, "system-monitor--minus.png");
            this.categoryImageList.Images.SetKeyName(8, "system-monitor--plus.png");
            this.categoryImageList.Images.SetKeyName(9, "document--minus.png");
            this.categoryImageList.Images.SetKeyName(10, "document-rename.png");
            this.categoryImageList.Images.SetKeyName(11, "document--arrow.png");
            this.categoryImageList.Images.SetKeyName(12, "wall--minus.png");
            this.categoryImageList.Images.SetKeyName(13, "wall--pencil.png");
            this.categoryImageList.Images.SetKeyName(14, "wall--plus.png");
            this.categoryImageList.Images.SetKeyName(15, "script-code.png");
            // 
            // cancelToolTip
            // 
            this.cancelToolTip.IsBalloon = true;
            this.cancelToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.cancelToolTip.ToolTipTitle = "Cancel the upload.";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(417, 7);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(121, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save package";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(544, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // loadingPanel
            // 
            this.loadingPanel.BackColor = System.Drawing.Color.White;
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.cancelLabel);
            this.loadingPanel.Controls.Add(this.pictureBox1);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Location = new System.Drawing.Point(189, 321);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(260, 51);
            this.loadingPanel.TabIndex = 45;
            this.loadingPanel.Visible = false;
            // 
            // cancelLabel
            // 
            this.cancelLabel.AutoSize = true;
            this.cancelLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.cancelLabel.Location = new System.Drawing.Point(241, -2);
            this.cancelLabel.Name = "cancelLabel";
            this.cancelLabel.Size = new System.Drawing.Size(14, 17);
            this.cancelLabel.TabIndex = 22;
            this.cancelLabel.Text = "x";
            this.cancelLabel.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(15, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 17);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoEllipsis = true;
            this.loadingLabel.Location = new System.Drawing.Point(34, 19);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(206, 16);
            this.loadingLabel.TabIndex = 11;
            this.loadingLabel.Text = "Waiting for thread...";
            // 
            // categoryTreeView
            // 
            this.categoryTreeView.AllowDrop = true;
            this.categoryTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.categoryTreeView.FullRowSelect = true;
            this.categoryTreeView.HideSelection = false;
            this.categoryTreeView.HotTracking = true;
            this.categoryTreeView.ImageIndex = 0;
            this.categoryTreeView.ImageList = this.categoryImageList;
            this.categoryTreeView.Indent = 5;
            this.categoryTreeView.ItemHeight = 24;
            this.categoryTreeView.Location = new System.Drawing.Point(8, 24);
            this.categoryTreeView.Name = "categoryTreeView";
            treeNode1.ImageIndex = 2;
            treeNode1.Name = "generalNode";
            treeNode1.SelectedImageIndex = 2;
            treeNode1.Text = "General";
            treeNode2.ImageIndex = 3;
            treeNode2.Name = "changelogNode";
            treeNode2.SelectedImageIndex = 3;
            treeNode2.Text = "Changelog";
            treeNode3.ImageIndex = 0;
            treeNode3.Name = "availabilityNode";
            treeNode3.SelectedImageIndex = 0;
            treeNode3.Text = "Availability";
            treeNode4.ImageIndex = 4;
            treeNode4.Name = "operationsNode";
            treeNode4.SelectedImageIndex = 4;
            treeNode4.Text = "Operations";
            this.categoryTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.categoryTreeView.SelectedImageIndex = 0;
            this.categoryTreeView.ShowLines = false;
            this.categoryTreeView.Size = new System.Drawing.Size(129, 231);
            this.categoryTreeView.TabIndex = 46;
            this.categoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.categoryTreeView_AfterSelect);
            this.categoryTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.categoryTreeView_DragDrop);
            this.categoryTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.categoryTreeView_DragEnter);
            this.categoryTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.categoryTreeView_KeyDown);
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.saveButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 265);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(632, 40);
            this.controlPanel1.TabIndex = 47;
            // 
            // categoryTabControl
            // 
            this.categoryTabControl.Controls.Add(this.generalTabPage);
            this.categoryTabControl.Controls.Add(this.changelogTabPage);
            this.categoryTabControl.Controls.Add(this.availabilityTabPage);
            this.categoryTabControl.Controls.Add(this.operationsTabPage);
            this.categoryTabControl.Location = new System.Drawing.Point(151, 24);
            this.categoryTabControl.Name = "categoryTabControl";
            this.categoryTabControl.SelectedIndex = 0;
            this.categoryTabControl.Size = new System.Drawing.Size(481, 262);
            this.categoryTabControl.TabIndex = 48;
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.generalPanel);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(473, 236);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "General";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // generalPanel
            // 
            this.generalPanel.AutoScroll = true;
            this.generalPanel.AutoScrollMargin = new System.Drawing.Size(0, 10);
            this.generalPanel.AutoScrollMinSize = new System.Drawing.Size(1, 20);
            this.generalPanel.Controls.Add(this.includeIntoStatisticsInfoLabel);
            this.generalPanel.Controls.Add(this.includeIntoStatisticsCheckBox);
            this.generalPanel.Controls.Add(this.line2);
            this.generalPanel.Controls.Add(this.developmentBuildNumericUpDown);
            this.generalPanel.Controls.Add(this.necessaryUpdateInfoLabel);
            this.generalPanel.Controls.Add(this.necessaryUpdateCheckBox);
            this.generalPanel.Controls.Add(this.line3);
            this.generalPanel.Controls.Add(this.line1);
            this.generalPanel.Controls.Add(this.architectureInfoLabel);
            this.generalPanel.Controls.Add(this.buildNumericUpDown);
            this.generalPanel.Controls.Add(this.descriptionLabel);
            this.generalPanel.Controls.Add(this.environmentLabel);
            this.generalPanel.Controls.Add(this.majorNumericUpDown);
            this.generalPanel.Controls.Add(this.architectureComboBox);
            this.generalPanel.Controls.Add(this.descriptionTextBox);
            this.generalPanel.Controls.Add(this.revisionNumericUpDown);
            this.generalPanel.Controls.Add(this.versionLabel);
            this.generalPanel.Controls.Add(this.developmentalStageComboBox);
            this.generalPanel.Controls.Add(this.devStageLabel);
            this.generalPanel.Controls.Add(this.minorNumericUpDown);
            this.generalPanel.Location = new System.Drawing.Point(0, 0);
            this.generalPanel.Name = "generalPanel";
            this.generalPanel.Size = new System.Drawing.Size(474, 236);
            this.generalPanel.TabIndex = 30;
            // 
            // includeIntoStatisticsInfoLabel
            // 
            this.includeIntoStatisticsInfoLabel.AutoSize = true;
            this.includeIntoStatisticsInfoLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.includeIntoStatisticsInfoLabel.Location = new System.Drawing.Point(9, 302);
            this.includeIntoStatisticsInfoLabel.Name = "includeIntoStatisticsInfoLabel";
            this.includeIntoStatisticsInfoLabel.Size = new System.Drawing.Size(433, 26);
            this.includeIntoStatisticsInfoLabel.TabIndex = 28;
            this.includeIntoStatisticsInfoLabel.Text = "Sets if this update package is included into the statistics when it is downloaded" +
    " by\r\na client. You can change this option later.";
            // 
            // includeIntoStatisticsCheckBox
            // 
            this.includeIntoStatisticsCheckBox.AutoSize = true;
            this.includeIntoStatisticsCheckBox.Location = new System.Drawing.Point(11, 279);
            this.includeIntoStatisticsCheckBox.Name = "includeIntoStatisticsCheckBox";
            this.includeIntoStatisticsCheckBox.Size = new System.Drawing.Size(135, 17);
            this.includeIntoStatisticsCheckBox.TabIndex = 27;
            this.includeIntoStatisticsCheckBox.Text = "Include into statistics";
            this.includeIntoStatisticsCheckBox.UseVisualStyleBackColor = true;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.SystemColors.Window;
            this.line2.LineAlignment = Line.Alignment.Horizontal;
            this.line2.Location = new System.Drawing.Point(9, 262);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(437, 10);
            this.line2.TabIndex = 26;
            this.line2.Text = "line2";
            // 
            // developmentBuildNumericUpDown
            // 
            this.developmentBuildNumericUpDown.Enabled = false;
            this.developmentBuildNumericUpDown.Location = new System.Drawing.Point(363, 8);
            this.developmentBuildNumericUpDown.Name = "developmentBuildNumericUpDown";
            this.developmentBuildNumericUpDown.Size = new System.Drawing.Size(43, 22);
            this.developmentBuildNumericUpDown.TabIndex = 25;
            this.developmentBuildNumericUpDown.ThousandsSeparator = true;
            // 
            // necessaryUpdateInfoLabel
            // 
            this.necessaryUpdateInfoLabel.AutoSize = true;
            this.necessaryUpdateInfoLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.necessaryUpdateInfoLabel.Location = new System.Drawing.Point(10, 210);
            this.necessaryUpdateInfoLabel.Name = "necessaryUpdateInfoLabel";
            this.necessaryUpdateInfoLabel.Size = new System.Drawing.Size(437, 39);
            this.necessaryUpdateInfoLabel.TabIndex = 24;
            this.necessaryUpdateInfoLabel.Text = resources.GetString("necessaryUpdateInfoLabel.Text");
            // 
            // necessaryUpdateCheckBox
            // 
            this.necessaryUpdateCheckBox.AutoSize = true;
            this.necessaryUpdateCheckBox.Location = new System.Drawing.Point(12, 187);
            this.necessaryUpdateCheckBox.Name = "necessaryUpdateCheckBox";
            this.necessaryUpdateCheckBox.Size = new System.Drawing.Size(116, 17);
            this.necessaryUpdateCheckBox.TabIndex = 23;
            this.necessaryUpdateCheckBox.Text = "Necessary update";
            this.necessaryUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // line3
            // 
            this.line3.BackColor = System.Drawing.SystemColors.Window;
            this.line3.LineAlignment = Line.Alignment.Horizontal;
            this.line3.Location = new System.Drawing.Point(10, 170);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(437, 10);
            this.line3.TabIndex = 22;
            this.line3.Text = "line3";
            // 
            // line1
            // 
            this.line1.BackColor = System.Drawing.SystemColors.Window;
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(7, 92);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(437, 14);
            this.line1.TabIndex = 21;
            this.line1.Text = "line1";
            // 
            // architectureInfoLabel
            // 
            this.architectureInfoLabel.AutoSize = true;
            this.architectureInfoLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.architectureInfoLabel.Location = new System.Drawing.Point(9, 137);
            this.architectureInfoLabel.Name = "architectureInfoLabel";
            this.architectureInfoLabel.Size = new System.Drawing.Size(438, 26);
            this.architectureInfoLabel.TabIndex = 20;
            this.architectureInfoLabel.Text = "Sets if the update package should only run on special architectures. To set any t" +
    "ype \r\nof architecture, choose \"AnyCPU\" as entry.";
            // 
            // buildNumericUpDown
            // 
            this.buildNumericUpDown.Location = new System.Drawing.Point(331, 35);
            this.buildNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.buildNumericUpDown.Name = "buildNumericUpDown";
            this.buildNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.buildNumericUpDown.TabIndex = 5;
            this.buildNumericUpDown.ThousandsSeparator = true;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(8, 66);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(122, 13);
            this.descriptionLabel.TabIndex = 10;
            this.descriptionLabel.Text = "Description (optional):";
            // 
            // environmentLabel
            // 
            this.environmentLabel.AutoSize = true;
            this.environmentLabel.Location = new System.Drawing.Point(8, 113);
            this.environmentLabel.Name = "environmentLabel";
            this.environmentLabel.Size = new System.Drawing.Size(116, 13);
            this.environmentLabel.TabIndex = 18;
            this.environmentLabel.Text = "Architecture settings:";
            // 
            // majorNumericUpDown
            // 
            this.majorNumericUpDown.Location = new System.Drawing.Point(207, 35);
            this.majorNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.majorNumericUpDown.Name = "majorNumericUpDown";
            this.majorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.majorNumericUpDown.TabIndex = 3;
            this.majorNumericUpDown.ThousandsSeparator = true;
            // 
            // architectureComboBox
            // 
            this.architectureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.architectureComboBox.FormattingEnabled = true;
            this.architectureComboBox.Items.AddRange(new object[] {
            "x86 (32-bit)",
            "x64 (64-bit)",
            "AnyCPU (independent)"});
            this.architectureComboBox.Location = new System.Drawing.Point(207, 109);
            this.architectureComboBox.Name = "architectureComboBox";
            this.architectureComboBox.Size = new System.Drawing.Size(150, 21);
            this.architectureComboBox.TabIndex = 17;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(207, 63);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(240, 22);
            this.descriptionTextBox.TabIndex = 11;
            // 
            // revisionNumericUpDown
            // 
            this.revisionNumericUpDown.Location = new System.Drawing.Point(391, 35);
            this.revisionNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.revisionNumericUpDown.Name = "revisionNumericUpDown";
            this.revisionNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.revisionNumericUpDown.TabIndex = 6;
            this.revisionNumericUpDown.ThousandsSeparator = true;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(8, 39);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(49, 13);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "Version:";
            // 
            // developmentalStageComboBox
            // 
            this.developmentalStageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.developmentalStageComboBox.FormattingEnabled = true;
            this.developmentalStageComboBox.Location = new System.Drawing.Point(207, 8);
            this.developmentalStageComboBox.Name = "developmentalStageComboBox";
            this.developmentalStageComboBox.Size = new System.Drawing.Size(150, 21);
            this.developmentalStageComboBox.TabIndex = 0;
            this.developmentalStageComboBox.SelectedIndexChanged += new System.EventHandler(this.developmentalStageComboBox_SelectedIndexChanged);
            // 
            // devStageLabel
            // 
            this.devStageLabel.AutoSize = true;
            this.devStageLabel.Location = new System.Drawing.Point(8, 11);
            this.devStageLabel.Name = "devStageLabel";
            this.devStageLabel.Size = new System.Drawing.Size(118, 13);
            this.devStageLabel.TabIndex = 1;
            this.devStageLabel.Text = "Developmental stage:";
            // 
            // minorNumericUpDown
            // 
            this.minorNumericUpDown.Location = new System.Drawing.Point(269, 35);
            this.minorNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.minorNumericUpDown.Name = "minorNumericUpDown";
            this.minorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.minorNumericUpDown.TabIndex = 4;
            this.minorNumericUpDown.ThousandsSeparator = true;
            // 
            // changelogTabPage
            // 
            this.changelogTabPage.Controls.Add(this.changelogPanel);
            this.changelogTabPage.Location = new System.Drawing.Point(4, 22);
            this.changelogTabPage.Name = "changelogTabPage";
            this.changelogTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.changelogTabPage.Size = new System.Drawing.Size(473, 236);
            this.changelogTabPage.TabIndex = 1;
            this.changelogTabPage.Text = "Changelog";
            this.changelogTabPage.UseVisualStyleBackColor = true;
            // 
            // changelogPanel
            // 
            this.changelogPanel.Controls.Add(this.changelogContentTabControl);
            this.changelogPanel.Controls.Add(this.changelogToolStrip);
            this.changelogPanel.Location = new System.Drawing.Point(0, 2);
            this.changelogPanel.Name = "changelogPanel";
            this.changelogPanel.Size = new System.Drawing.Size(474, 235);
            this.changelogPanel.TabIndex = 32;
            // 
            // changelogContentTabControl
            // 
            this.changelogContentTabControl.Controls.Add(this.englishChangelogTabPage);
            this.changelogContentTabControl.Location = new System.Drawing.Point(6, 24);
            this.changelogContentTabControl.Name = "changelogContentTabControl";
            this.changelogContentTabControl.SelectedIndex = 0;
            this.changelogContentTabControl.Size = new System.Drawing.Size(461, 220);
            this.changelogContentTabControl.TabIndex = 4;
            // 
            // englishChangelogTabPage
            // 
            this.englishChangelogTabPage.Controls.Add(this.englishChangelogTextBox);
            this.englishChangelogTabPage.Location = new System.Drawing.Point(4, 22);
            this.englishChangelogTabPage.Name = "englishChangelogTabPage";
            this.englishChangelogTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.englishChangelogTabPage.Size = new System.Drawing.Size(453, 194);
            this.englishChangelogTabPage.TabIndex = 0;
            this.englishChangelogTabPage.Tag = "";
            this.englishChangelogTabPage.Text = "English";
            this.englishChangelogTabPage.UseVisualStyleBackColor = true;
            // 
            // englishChangelogTextBox
            // 
            this.englishChangelogTextBox.AcceptsReturn = true;
            this.englishChangelogTextBox.Location = new System.Drawing.Point(0, 3);
            this.englishChangelogTextBox.Multiline = true;
            this.englishChangelogTextBox.Name = "englishChangelogTextBox";
            this.englishChangelogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.englishChangelogTextBox.Size = new System.Drawing.Size(453, 191);
            this.englishChangelogTextBox.TabIndex = 0;
            this.englishChangelogTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.englishChangelogTextBox_KeyDown);
            // 
            // changelogToolStrip
            // 
            this.changelogToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.changelogToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changelogLanguageComboBox,
            this.toolStripSeparator3,
            this.changelogLoadButton,
            this.toolStripSeparator5,
            this.changelogClearButton,
            this.toolStripSeparator6,
            this.specialCharactersComboBox});
            this.changelogToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.changelogToolStrip.Location = new System.Drawing.Point(0, 0);
            this.changelogToolStrip.Name = "changelogToolStrip";
            this.changelogToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.changelogToolStrip.Size = new System.Drawing.Size(474, 23);
            this.changelogToolStrip.TabIndex = 3;
            this.changelogToolStrip.Text = "toolStrip5";
            // 
            // changelogLanguageComboBox
            // 
            this.changelogLanguageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.changelogLanguageComboBox.Name = "changelogLanguageComboBox";
            this.changelogLanguageComboBox.Size = new System.Drawing.Size(121, 23);
            this.changelogLanguageComboBox.SelectedIndexChanged += new System.EventHandler(this.changelogLanguageComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // changelogLoadButton
            // 
            this.changelogLoadButton.Image = ((System.Drawing.Image)(resources.GetObject("changelogLoadButton.Image")));
            this.changelogLoadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.changelogLoadButton.Name = "changelogLoadButton";
            this.changelogLoadButton.Size = new System.Drawing.Size(101, 20);
            this.changelogLoadButton.Text = "Load from file";
            this.changelogLoadButton.Click += new System.EventHandler(this.changelogLoadButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 23);
            // 
            // changelogClearButton
            // 
            this.changelogClearButton.Image = ((System.Drawing.Image)(resources.GetObject("changelogClearButton.Image")));
            this.changelogClearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.changelogClearButton.Name = "changelogClearButton";
            this.changelogClearButton.Size = new System.Drawing.Size(54, 20);
            this.changelogClearButton.Text = "Clear";
            this.changelogClearButton.Click += new System.EventHandler(this.changelogClearButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 23);
            // 
            // specialCharactersComboBox
            // 
            this.specialCharactersComboBox.AutoToolTip = false;
            this.specialCharactersComboBox.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulletToolStripMenuItem,
            this.toolStripSeparator7,
            this.insideQuotationMarkToolStripMenuItem,
            this.classicQuotationMarkToolStripMenuItem,
            this.outsideQuotationMarkToolStripMenuItem,
            this.apostropheToolStripMenuItem,
            this.toolStripSeparator8,
            this.copyrightToolStripMenuItem,
            this.allRightsReservedToolStripMenuItem,
            this.soundRecordingCopyrightToolStripMenuItem,
            this.unregisteredTrademarkToolStripMenuItem,
            this.serviceMarkToolStripMenuItem});
            this.specialCharactersComboBox.Image = ((System.Drawing.Image)(resources.GetObject("specialCharactersComboBox.Image")));
            this.specialCharactersComboBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.specialCharactersComboBox.Name = "specialCharactersComboBox";
            this.specialCharactersComboBox.Size = new System.Drawing.Size(130, 20);
            this.specialCharactersComboBox.Text = "Special characters";
            this.specialCharactersComboBox.ToolTipText = "Special characters";
            // 
            // bulletToolStripMenuItem
            // 
            this.bulletToolStripMenuItem.Name = "bulletToolStripMenuItem";
            this.bulletToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.bulletToolStripMenuItem.Text = "Bullet: •";
            this.bulletToolStripMenuItem.Click += new System.EventHandler(this.bulletToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(232, 6);
            // 
            // insideQuotationMarkToolStripMenuItem
            // 
            this.insideQuotationMarkToolStripMenuItem.Name = "insideQuotationMarkToolStripMenuItem";
            this.insideQuotationMarkToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.insideQuotationMarkToolStripMenuItem.Text = "Quotation mark (inside): » « ";
            this.insideQuotationMarkToolStripMenuItem.Click += new System.EventHandler(this.insideQuotationMarkToolStripMenuItem_Click);
            // 
            // classicQuotationMarkToolStripMenuItem
            // 
            this.classicQuotationMarkToolStripMenuItem.Name = "classicQuotationMarkToolStripMenuItem";
            this.classicQuotationMarkToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.classicQuotationMarkToolStripMenuItem.Text = "Quotation mark classic: „ “";
            this.classicQuotationMarkToolStripMenuItem.Click += new System.EventHandler(this.classicQuotationMarkToolStripMenuItem_Click);
            // 
            // outsideQuotationMarkToolStripMenuItem
            // 
            this.outsideQuotationMarkToolStripMenuItem.Name = "outsideQuotationMarkToolStripMenuItem";
            this.outsideQuotationMarkToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.outsideQuotationMarkToolStripMenuItem.Text = "Quotation mark (outside): « »";
            this.outsideQuotationMarkToolStripMenuItem.Click += new System.EventHandler(this.outsideQuotationMarkToolStripMenuItem_Click);
            // 
            // apostropheToolStripMenuItem
            // 
            this.apostropheToolStripMenuItem.Name = "apostropheToolStripMenuItem";
            this.apostropheToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.apostropheToolStripMenuItem.Text = "Apostrophe: \'";
            this.apostropheToolStripMenuItem.Click += new System.EventHandler(this.apostropheToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(232, 6);
            // 
            // copyrightToolStripMenuItem
            // 
            this.copyrightToolStripMenuItem.Name = "copyrightToolStripMenuItem";
            this.copyrightToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.copyrightToolStripMenuItem.Text = "Copyright: ©";
            this.copyrightToolStripMenuItem.Click += new System.EventHandler(this.copyrightToolStripMenuItem_Click);
            // 
            // allRightsReservedToolStripMenuItem
            // 
            this.allRightsReservedToolStripMenuItem.Name = "allRightsReservedToolStripMenuItem";
            this.allRightsReservedToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.allRightsReservedToolStripMenuItem.Text = "Registered trademark: ®";
            this.allRightsReservedToolStripMenuItem.Click += new System.EventHandler(this.allRightsReservedToolStripMenuItem_Click);
            // 
            // soundRecordingCopyrightToolStripMenuItem
            // 
            this.soundRecordingCopyrightToolStripMenuItem.Name = "soundRecordingCopyrightToolStripMenuItem";
            this.soundRecordingCopyrightToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.soundRecordingCopyrightToolStripMenuItem.Text = "Sound-recording copyright: ℗";
            this.soundRecordingCopyrightToolStripMenuItem.Click += new System.EventHandler(this.soundRecordingCopyrightToolStripMenuItem_Click);
            // 
            // unregisteredTrademarkToolStripMenuItem
            // 
            this.unregisteredTrademarkToolStripMenuItem.Name = "unregisteredTrademarkToolStripMenuItem";
            this.unregisteredTrademarkToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.unregisteredTrademarkToolStripMenuItem.Text = "Trademark: ™";
            this.unregisteredTrademarkToolStripMenuItem.Click += new System.EventHandler(this.unregisteredTrademarkToolStripMenuItem_Click);
            // 
            // serviceMarkToolStripMenuItem
            // 
            this.serviceMarkToolStripMenuItem.Name = "serviceMarkToolStripMenuItem";
            this.serviceMarkToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.serviceMarkToolStripMenuItem.Text = "Service mark: ℠";
            this.serviceMarkToolStripMenuItem.Click += new System.EventHandler(this.serviceMarkToolStripMenuItem_Click);
            // 
            // availabilityTabPage
            // 
            this.availabilityTabPage.Controls.Add(this.availabilityPanel);
            this.availabilityTabPage.Location = new System.Drawing.Point(4, 22);
            this.availabilityTabPage.Name = "availabilityTabPage";
            this.availabilityTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.availabilityTabPage.Size = new System.Drawing.Size(473, 236);
            this.availabilityTabPage.TabIndex = 2;
            this.availabilityTabPage.Text = "Availability";
            this.availabilityTabPage.UseVisualStyleBackColor = true;
            // 
            // availabilityPanel
            // 
            this.availabilityPanel.AutoScrollMargin = new System.Drawing.Size(0, 40);
            this.availabilityPanel.Controls.Add(this.unsupportedVersionsPanel);
            this.availabilityPanel.Controls.Add(this.someVersionsInfoLabel);
            this.availabilityPanel.Controls.Add(this.allVersionsRadioButton);
            this.availabilityPanel.Controls.Add(this.allVersionsInfoLabel);
            this.availabilityPanel.Controls.Add(this.someVersionsRadioButton);
            this.availabilityPanel.Location = new System.Drawing.Point(0, 0);
            this.availabilityPanel.Name = "availabilityPanel";
            this.availabilityPanel.Size = new System.Drawing.Size(474, 235);
            this.availabilityPanel.TabIndex = 33;
            // 
            // unsupportedVersionsPanel
            // 
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedMajorNumericUpDown);
            this.unsupportedVersionsPanel.Controls.Add(this.addVersionButton);
            this.unsupportedVersionsPanel.Controls.Add(this.removeVersionButton);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedRevisionNumericUpDown);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedVersionsListBox);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedMinorNumericUpDown);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedBuildNumericUpDown);
            this.unsupportedVersionsPanel.Location = new System.Drawing.Point(26, 95);
            this.unsupportedVersionsPanel.Name = "unsupportedVersionsPanel";
            this.unsupportedVersionsPanel.Size = new System.Drawing.Size(373, 135);
            this.unsupportedVersionsPanel.TabIndex = 41;
            // 
            // unsupportedMajorNumericUpDown
            // 
            this.unsupportedMajorNumericUpDown.Location = new System.Drawing.Point(4, 105);
            this.unsupportedMajorNumericUpDown.Name = "unsupportedMajorNumericUpDown";
            this.unsupportedMajorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedMajorNumericUpDown.TabIndex = 26;
            // 
            // addVersionButton
            // 
            this.addVersionButton.Location = new System.Drawing.Point(286, 49);
            this.addVersionButton.Name = "addVersionButton";
            this.addVersionButton.Size = new System.Drawing.Size(82, 22);
            this.addVersionButton.TabIndex = 24;
            this.addVersionButton.Text = "Add";
            this.addVersionButton.UseVisualStyleBackColor = true;
            this.addVersionButton.Click += new System.EventHandler(this.addVersionButton_Click);
            // 
            // removeVersionButton
            // 
            this.removeVersionButton.Location = new System.Drawing.Point(286, 77);
            this.removeVersionButton.Name = "removeVersionButton";
            this.removeVersionButton.Size = new System.Drawing.Size(82, 22);
            this.removeVersionButton.TabIndex = 25;
            this.removeVersionButton.Text = "Remove";
            this.removeVersionButton.UseVisualStyleBackColor = true;
            this.removeVersionButton.Click += new System.EventHandler(this.removeVersionButton_Click);
            // 
            // unsupportedRevisionNumericUpDown
            // 
            this.unsupportedRevisionNumericUpDown.Location = new System.Drawing.Point(188, 105);
            this.unsupportedRevisionNumericUpDown.Name = "unsupportedRevisionNumericUpDown";
            this.unsupportedRevisionNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedRevisionNumericUpDown.TabIndex = 29;
            // 
            // unsupportedVersionsListBox
            // 
            this.unsupportedVersionsListBox.FormattingEnabled = true;
            this.unsupportedVersionsListBox.Location = new System.Drawing.Point(4, 4);
            this.unsupportedVersionsListBox.Name = "unsupportedVersionsListBox";
            this.unsupportedVersionsListBox.Size = new System.Drawing.Size(276, 95);
            this.unsupportedVersionsListBox.TabIndex = 23;
            // 
            // unsupportedMinorNumericUpDown
            // 
            this.unsupportedMinorNumericUpDown.Location = new System.Drawing.Point(66, 105);
            this.unsupportedMinorNumericUpDown.Name = "unsupportedMinorNumericUpDown";
            this.unsupportedMinorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedMinorNumericUpDown.TabIndex = 27;
            // 
            // unsupportedBuildNumericUpDown
            // 
            this.unsupportedBuildNumericUpDown.Location = new System.Drawing.Point(128, 105);
            this.unsupportedBuildNumericUpDown.Name = "unsupportedBuildNumericUpDown";
            this.unsupportedBuildNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedBuildNumericUpDown.TabIndex = 28;
            // 
            // someVersionsInfoLabel
            // 
            this.someVersionsInfoLabel.AutoSize = true;
            this.someVersionsInfoLabel.Location = new System.Drawing.Point(34, 67);
            this.someVersionsInfoLabel.Name = "someVersionsInfoLabel";
            this.someVersionsInfoLabel.Size = new System.Drawing.Size(292, 13);
            this.someVersionsInfoLabel.TabIndex = 40;
            this.someVersionsInfoLabel.Text = "This package is not available for the following versions:";
            // 
            // allVersionsRadioButton
            // 
            this.allVersionsRadioButton.AutoSize = true;
            this.allVersionsRadioButton.Checked = true;
            this.allVersionsRadioButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allVersionsRadioButton.Location = new System.Drawing.Point(12, 7);
            this.allVersionsRadioButton.Name = "allVersionsRadioButton";
            this.allVersionsRadioButton.Size = new System.Drawing.Size(182, 17);
            this.allVersionsRadioButton.TabIndex = 37;
            this.allVersionsRadioButton.TabStop = true;
            this.allVersionsRadioButton.Text = "Available for all older versions";
            this.allVersionsRadioButton.UseVisualStyleBackColor = true;
            this.allVersionsRadioButton.CheckedChanged += new System.EventHandler(this.allVersionsRadioButton_CheckedChanged);
            // 
            // allVersionsInfoLabel
            // 
            this.allVersionsInfoLabel.AutoSize = true;
            this.allVersionsInfoLabel.Location = new System.Drawing.Point(34, 27);
            this.allVersionsInfoLabel.Name = "allVersionsInfoLabel";
            this.allVersionsInfoLabel.Size = new System.Drawing.Size(372, 13);
            this.allVersionsInfoLabel.TabIndex = 39;
            this.allVersionsInfoLabel.Text = "This package is available and can be downloaded for all older versions.";
            // 
            // someVersionsRadioButton
            // 
            this.someVersionsRadioButton.AutoSize = true;
            this.someVersionsRadioButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.someVersionsRadioButton.Location = new System.Drawing.Point(12, 47);
            this.someVersionsRadioButton.Name = "someVersionsRadioButton";
            this.someVersionsRadioButton.Size = new System.Drawing.Size(219, 17);
            this.someVersionsRadioButton.TabIndex = 38;
            this.someVersionsRadioButton.Text = "Not available for some older versions";
            this.someVersionsRadioButton.UseVisualStyleBackColor = true;
            this.someVersionsRadioButton.CheckedChanged += new System.EventHandler(this.someVersionsRadioButton_CheckedChanged);
            // 
            // operationsTabPage
            // 
            this.operationsTabPage.Controls.Add(this.operationsPanel);
            this.operationsTabPage.Location = new System.Drawing.Point(4, 22);
            this.operationsTabPage.Name = "operationsTabPage";
            this.operationsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.operationsTabPage.Size = new System.Drawing.Size(473, 236);
            this.operationsTabPage.TabIndex = 3;
            this.operationsTabPage.Text = "Operations";
            this.operationsTabPage.UseVisualStyleBackColor = true;
            // 
            // operationsPanel
            // 
            this.operationsPanel.AutoScroll = true;
            this.operationsPanel.Controls.Add(this.operationsListView);
            this.operationsPanel.Location = new System.Drawing.Point(0, -2);
            this.operationsPanel.Name = "operationsPanel";
            this.operationsPanel.Size = new System.Drawing.Size(474, 235);
            this.operationsPanel.TabIndex = 5;
            // 
            // operationsListView
            // 
            this.operationsListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.operationsListView.AllowDrop = true;
            this.operationsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.operationsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.itemName,
            this.Description});
            this.operationsListView.FullRowSelect = true;
            listViewGroup1.Header = "Files";
            listViewGroup1.Name = "filesGroup";
            listViewGroup2.Header = "Registry";
            listViewGroup2.Name = "registryGroup";
            listViewGroup3.Header = "Processes";
            listViewGroup3.Name = "processGroup";
            listViewGroup4.Header = "Services";
            listViewGroup4.Name = "serviceGroup";
            listViewGroup5.Header = "Scripts";
            listViewGroup5.Name = "scriptGroup";
            this.operationsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5});
            this.operationsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.operationsListView.HoverSelection = true;
            listViewItem1.Group = listViewGroup1;
            listViewItem1.Tag = "RenameFile";
            listViewItem2.Group = listViewGroup1;
            listViewItem2.Tag = "DeleteFile";
            listViewItem3.Group = listViewGroup2;
            listViewItem3.Tag = "CreateRegistrySubKey";
            listViewItem4.Group = listViewGroup2;
            listViewItem4.Tag = "DeleteRegistrySubKey";
            listViewItem5.Group = listViewGroup2;
            listViewItem5.Tag = "SetRegistryValue";
            listViewItem6.Group = listViewGroup2;
            listViewItem6.Tag = "DeleteRegistryValue";
            listViewItem7.Group = listViewGroup3;
            listViewItem7.Tag = "StartProcess";
            listViewItem8.Group = listViewGroup3;
            listViewItem8.Tag = "TerminateProcess";
            listViewItem9.Group = listViewGroup4;
            listViewItem9.Tag = "StartService";
            listViewItem10.Group = listViewGroup4;
            listViewItem10.Tag = "StopService";
            listViewItem11.Group = listViewGroup5;
            listViewItem11.Tag = "ExecuteScript";
            this.operationsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11});
            this.operationsListView.LargeImageList = this.categoryImageList;
            this.operationsListView.Location = new System.Drawing.Point(0, 2);
            this.operationsListView.MultiSelect = false;
            this.operationsListView.Name = "operationsListView";
            this.operationsListView.Size = new System.Drawing.Size(474, 230);
            this.operationsListView.SmallImageList = this.categoryImageList;
            this.operationsListView.TabIndex = 1;
            this.operationsListView.TileSize = new System.Drawing.Size(450, 50);
            this.operationsListView.UseCompatibleStateImageBehavior = false;
            this.operationsListView.View = System.Windows.Forms.View.Tile;
            this.operationsListView.DragOver += new System.Windows.Forms.DragEventHandler(this.operationsListView_DragOver);
            this.operationsListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationsListView_MouseDown);
            // 
            // itemName
            // 
            this.itemName.Width = 300;
            // 
            // Description
            // 
            this.Description.Width = 300;
            // 
            // PackageEditDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(632, 305);
            this.Controls.Add(this.categoryTreeView);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.categoryTabControl);
            this.Controls.Add(this.loadingPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackageEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit update package - {0} - {1}";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PackageEditDialog_FormClosing);
            this.Load += new System.EventHandler(this.PackageEditDialog_Load);
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.categoryTabControl.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.generalPanel.ResumeLayout(false);
            this.generalPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.developmentBuildNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.revisionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minorNumericUpDown)).EndInit();
            this.changelogTabPage.ResumeLayout(false);
            this.changelogPanel.ResumeLayout(false);
            this.changelogPanel.PerformLayout();
            this.changelogContentTabControl.ResumeLayout(false);
            this.englishChangelogTabPage.ResumeLayout(false);
            this.englishChangelogTabPage.PerformLayout();
            this.changelogToolStrip.ResumeLayout(false);
            this.changelogToolStrip.PerformLayout();
            this.availabilityTabPage.ResumeLayout(false);
            this.availabilityPanel.ResumeLayout(false);
            this.availabilityPanel.PerformLayout();
            this.unsupportedVersionsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMajorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedRevisionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMinorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedBuildNumericUpDown)).EndInit();
            this.operationsTabPage.ResumeLayout(false);
            this.operationsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage operationsTabPage;
        private System.Windows.Forms.Panel operationsPanel;
        private Controls.ExplorerListView operationsListView;
        private System.Windows.Forms.ColumnHeader itemName;
        private System.Windows.Forms.ColumnHeader Description;
        private System.Windows.Forms.ImageList categoryImageList;
        private System.Windows.Forms.Label someVersionsInfoLabel;
        private System.Windows.Forms.RadioButton allVersionsRadioButton;
        private System.Windows.Forms.Label allVersionsInfoLabel;
        private System.Windows.Forms.RadioButton someVersionsRadioButton;
        private System.Windows.Forms.TabPage availabilityTabPage;
        private System.Windows.Forms.Panel availabilityPanel;
        private System.Windows.Forms.ToolStripMenuItem serviceMarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unregisteredTrademarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem soundRecordingCopyrightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allRightsReservedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyrightToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem outsideQuotationMarkToolStripMenuItem;
        private System.Windows.Forms.ToolTip cancelToolTip;
        private System.Windows.Forms.Label cancelLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.ToolStripMenuItem apostropheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem classicQuotationMarkToolStripMenuItem;
        private Controls.ExplorerTreeView categoryTreeView;
        private System.Windows.Forms.Label includeIntoStatisticsInfoLabel;
        private System.Windows.Forms.CheckBox includeIntoStatisticsCheckBox;
        private Line line2;
        private System.Windows.Forms.Panel generalPanel;
        private System.Windows.Forms.NumericUpDown developmentBuildNumericUpDown;
        private System.Windows.Forms.Label necessaryUpdateInfoLabel;
        private System.Windows.Forms.CheckBox necessaryUpdateCheckBox;
        private Line line3;
        private Line line1;
        private System.Windows.Forms.Label architectureInfoLabel;
        private System.Windows.Forms.NumericUpDown buildNumericUpDown;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label environmentLabel;
        private System.Windows.Forms.NumericUpDown majorNumericUpDown;
        private System.Windows.Forms.ComboBox architectureComboBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.NumericUpDown revisionNumericUpDown;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.ComboBox developmentalStageComboBox;
        private System.Windows.Forms.Label devStageLabel;
        private System.Windows.Forms.NumericUpDown minorNumericUpDown;
        private System.Windows.Forms.TabPage generalTabPage;
        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private Controls.TablessTabControl categoryTabControl;
        private System.Windows.Forms.TabPage changelogTabPage;
        private System.Windows.Forms.Panel changelogPanel;
        private Controls.TablessTabControl changelogContentTabControl;
        private System.Windows.Forms.TabPage englishChangelogTabPage;
        private System.Windows.Forms.TextBox englishChangelogTextBox;
        private System.Windows.Forms.ToolStrip changelogToolStrip;
        private System.Windows.Forms.ToolStripComboBox changelogLanguageComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton changelogLoadButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton changelogClearButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripDropDownButton specialCharactersComboBox;
        private System.Windows.Forms.ToolStripMenuItem bulletToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem insideQuotationMarkToolStripMenuItem;
        private System.Windows.Forms.Panel unsupportedVersionsPanel;
        private System.Windows.Forms.NumericUpDown unsupportedMajorNumericUpDown;
        private System.Windows.Forms.Button addVersionButton;
        private System.Windows.Forms.Button removeVersionButton;
        private System.Windows.Forms.NumericUpDown unsupportedRevisionNumericUpDown;
        private System.Windows.Forms.ListBox unsupportedVersionsListBox;
        private System.Windows.Forms.NumericUpDown unsupportedMinorNumericUpDown;
        private System.Windows.Forms.NumericUpDown unsupportedBuildNumericUpDown;



    }
}