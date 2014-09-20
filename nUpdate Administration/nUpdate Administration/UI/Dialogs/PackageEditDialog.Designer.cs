﻿namespace nUpdate.Administration.UI.Dialogs
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Registry", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Processes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Services", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rename file",
            "Renames a given file to the new name set."}, 10);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Delete file",
            "Deletes a given file."}, 9);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Create entry",
            "Creates a given entry in the registry."}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Delete entry",
            "Deletes a given entry in the registry."}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Set key value",
            "Sets the value of a key in the registry."}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "Start process",
            "Starts a given process."}, 8);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "Terminate process",
            "Terminates a given process if possible."}, 7);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "Start service.",
            "Starts a windows service. If it is already running it will be restarted. "}, 5);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "Stop service.",
            "Stops a running windows service."}, 6);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("General", 2, 2);
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Changelog", 3, 3);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Availability", 0, 0);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Operations", 4, 4);
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Program directory", 2, 2);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("AppData", 2, 2);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Temp directory", 2, 2);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Desktop", 2, 2);
            this.operationsPanel = new System.Windows.Forms.Panel();
            this.categoryImageList = new System.Windows.Forms.ImageList(this.components);
            this.availabilityPanel = new System.Windows.Forms.Panel();
            this.someVersionsInfoLabel = new System.Windows.Forms.Label();
            this.allVersionsRadioButton = new System.Windows.Forms.RadioButton();
            this.allVersionsInfoLabel = new System.Windows.Forms.Label();
            this.unsupportedVersionsPanel = new System.Windows.Forms.Panel();
            this.unsupportedMajorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.addVersionButton = new System.Windows.Forms.Button();
            this.removeVersionButton = new System.Windows.Forms.Button();
            this.unsupportedRevisionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.unsupportedVersionsList = new System.Windows.Forms.ListBox();
            this.unsupportedMinorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.unsupportedBuildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.someVersionsRadioButton = new System.Windows.Forms.RadioButton();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.changelogPanel = new System.Windows.Forms.Panel();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.changelogLoadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.changelogClearButton = new System.Windows.Forms.ToolStripButton();
            this.changelogTextBox = new System.Windows.Forms.TextBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesImageList = new System.Windows.Forms.ImageList(this.components);
            this.generalPanel = new System.Windows.Forms.Panel();
            this.developmentBuildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.mustUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.environmentInfoLabel = new System.Windows.Forms.Label();
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
            this.filesList = new System.Windows.Forms.ListView();
            this.filesPanel = new System.Windows.Forms.Panel();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.addFolderButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addFilesButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.removeEntryButton = new System.Windows.Forms.ToolStripButton();
            this.itemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.editPackageButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.operationsListView = new nUpdate.Administration.UI.Controls.ExtendedListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.line3 = new nUpdate.Administration.UI.Controls.Line();
            this.line1 = new nUpdate.Administration.UI.Controls.Line();
            this.categoryTreeView = new nUpdate.Administration.UI.Controls.ExplorerTreeView();
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.filesDataTreeView = new nUpdate.Administration.UI.Controls.ExplorerTreeView();
            this.operationsPanel.SuspendLayout();
            this.availabilityPanel.SuspendLayout();
            this.unsupportedVersionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMajorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedRevisionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMinorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedBuildNumericUpDown)).BeginInit();
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.changelogPanel.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            this.generalPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.developmentBuildNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.revisionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minorNumericUpDown)).BeginInit();
            this.filesPanel.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // operationsPanel
            // 
            this.operationsPanel.AutoScroll = true;
            this.operationsPanel.Controls.Add(this.operationsListView);
            this.operationsPanel.Location = new System.Drawing.Point(148, 22);
            this.operationsPanel.Name = "operationsPanel";
            this.operationsPanel.Size = new System.Drawing.Size(474, 235);
            this.operationsPanel.TabIndex = 44;
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
            this.categoryImageList.Images.SetKeyName(10, "document--pencil.png");
            this.categoryImageList.Images.SetKeyName(11, "document--arrow.png");
            // 
            // availabilityPanel
            // 
            this.availabilityPanel.Controls.Add(this.someVersionsInfoLabel);
            this.availabilityPanel.Controls.Add(this.allVersionsRadioButton);
            this.availabilityPanel.Controls.Add(this.allVersionsInfoLabel);
            this.availabilityPanel.Controls.Add(this.unsupportedVersionsPanel);
            this.availabilityPanel.Controls.Add(this.someVersionsRadioButton);
            this.availabilityPanel.Location = new System.Drawing.Point(148, 22);
            this.availabilityPanel.Name = "availabilityPanel";
            this.availabilityPanel.Size = new System.Drawing.Size(474, 235);
            this.availabilityPanel.TabIndex = 49;
            // 
            // someVersionsInfoLabel
            // 
            this.someVersionsInfoLabel.AutoSize = true;
            this.someVersionsInfoLabel.Location = new System.Drawing.Point(34, 67);
            this.someVersionsInfoLabel.Name = "someVersionsInfoLabel";
            this.someVersionsInfoLabel.Size = new System.Drawing.Size(292, 13);
            this.someVersionsInfoLabel.TabIndex = 40;
            this.someVersionsInfoLabel.Text = "This package is not available for the following versions.";
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
            // unsupportedVersionsPanel
            // 
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedMajorNumericUpDown);
            this.unsupportedVersionsPanel.Controls.Add(this.addVersionButton);
            this.unsupportedVersionsPanel.Controls.Add(this.removeVersionButton);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedRevisionNumericUpDown);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedVersionsList);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedMinorNumericUpDown);
            this.unsupportedVersionsPanel.Controls.Add(this.unsupportedBuildNumericUpDown);
            this.unsupportedVersionsPanel.Location = new System.Drawing.Point(4, 88);
            this.unsupportedVersionsPanel.Name = "unsupportedVersionsPanel";
            this.unsupportedVersionsPanel.Size = new System.Drawing.Size(335, 135);
            this.unsupportedVersionsPanel.TabIndex = 36;
            // 
            // unsupportedMajorNumericUpDown
            // 
            this.unsupportedMajorNumericUpDown.Location = new System.Drawing.Point(31, 108);
            this.unsupportedMajorNumericUpDown.Name = "unsupportedMajorNumericUpDown";
            this.unsupportedMajorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedMajorNumericUpDown.TabIndex = 26;
            // 
            // addVersionButton
            // 
            this.addVersionButton.Location = new System.Drawing.Point(246, 53);
            this.addVersionButton.Name = "addVersionButton";
            this.addVersionButton.Size = new System.Drawing.Size(82, 22);
            this.addVersionButton.TabIndex = 24;
            this.addVersionButton.Text = "Add";
            this.addVersionButton.UseVisualStyleBackColor = true;
            // 
            // removeVersionButton
            // 
            this.removeVersionButton.Location = new System.Drawing.Point(246, 81);
            this.removeVersionButton.Name = "removeVersionButton";
            this.removeVersionButton.Size = new System.Drawing.Size(82, 22);
            this.removeVersionButton.TabIndex = 25;
            this.removeVersionButton.Text = "Remove";
            this.removeVersionButton.UseVisualStyleBackColor = true;
            // 
            // unsupportedRevisionNumericUpDown
            // 
            this.unsupportedRevisionNumericUpDown.Location = new System.Drawing.Point(215, 108);
            this.unsupportedRevisionNumericUpDown.Name = "unsupportedRevisionNumericUpDown";
            this.unsupportedRevisionNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedRevisionNumericUpDown.TabIndex = 29;
            // 
            // unsupportedVersionsList
            // 
            this.unsupportedVersionsList.FormattingEnabled = true;
            this.unsupportedVersionsList.Location = new System.Drawing.Point(31, 8);
            this.unsupportedVersionsList.Name = "unsupportedVersionsList";
            this.unsupportedVersionsList.Size = new System.Drawing.Size(209, 95);
            this.unsupportedVersionsList.TabIndex = 23;
            // 
            // unsupportedMinorNumericUpDown
            // 
            this.unsupportedMinorNumericUpDown.Location = new System.Drawing.Point(93, 108);
            this.unsupportedMinorNumericUpDown.Name = "unsupportedMinorNumericUpDown";
            this.unsupportedMinorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedMinorNumericUpDown.TabIndex = 27;
            // 
            // unsupportedBuildNumericUpDown
            // 
            this.unsupportedBuildNumericUpDown.Location = new System.Drawing.Point(155, 108);
            this.unsupportedBuildNumericUpDown.Name = "unsupportedBuildNumericUpDown";
            this.unsupportedBuildNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.unsupportedBuildNumericUpDown.TabIndex = 28;
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
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 147;
            // 
            // loadingPanel
            // 
            this.loadingPanel.BackColor = System.Drawing.Color.White;
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.pictureBox1);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Location = new System.Drawing.Point(8, 282);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(227, 51);
            this.loadingPanel.TabIndex = 46;
            this.loadingPanel.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(9, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 21);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.Location = new System.Drawing.Point(34, 19);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(112, 13);
            this.loadingLabel.TabIndex = 11;
            this.loadingLabel.Text = "Waiting for thread...";
            // 
            // changelogPanel
            // 
            this.changelogPanel.Controls.Add(this.toolStrip5);
            this.changelogPanel.Controls.Add(this.changelogTextBox);
            this.changelogPanel.Location = new System.Drawing.Point(148, 22);
            this.changelogPanel.Name = "changelogPanel";
            this.changelogPanel.Size = new System.Drawing.Size(474, 235);
            this.changelogPanel.TabIndex = 48;
            // 
            // toolStrip5
            // 
            this.toolStrip5.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changelogLoadButton,
            this.toolStripSeparator5,
            this.changelogClearButton});
            this.toolStrip5.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip5.Size = new System.Drawing.Size(474, 23);
            this.toolStrip5.TabIndex = 3;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // changelogLoadButton
            // 
            this.changelogLoadButton.Image = ((System.Drawing.Image)(resources.GetObject("changelogLoadButton.Image")));
            this.changelogLoadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.changelogLoadButton.Name = "changelogLoadButton";
            this.changelogLoadButton.Size = new System.Drawing.Size(101, 20);
            this.changelogLoadButton.Text = "Load from file";
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
            // 
            // changelogTextBox
            // 
            this.changelogTextBox.AcceptsReturn = true;
            this.changelogTextBox.Location = new System.Drawing.Point(4, 26);
            this.changelogTextBox.Multiline = true;
            this.changelogTextBox.Name = "changelogTextBox";
            this.changelogTextBox.Size = new System.Drawing.Size(467, 207);
            this.changelogTextBox.TabIndex = 0;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 316;
            // 
            // filesImageList
            // 
            this.filesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("filesImageList.ImageStream")));
            this.filesImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.filesImageList.Images.SetKeyName(0, "zip.png");
            this.filesImageList.Images.SetKeyName(1, "file.png");
            this.filesImageList.Images.SetKeyName(2, "folder.png");
            // 
            // generalPanel
            // 
            this.generalPanel.AutoScroll = true;
            this.generalPanel.AutoScrollMargin = new System.Drawing.Size(0, 7);
            this.generalPanel.AutoScrollMinSize = new System.Drawing.Size(1, 20);
            this.generalPanel.Controls.Add(this.developmentBuildNumericUpDown);
            this.generalPanel.Controls.Add(this.label1);
            this.generalPanel.Controls.Add(this.mustUpdateCheckBox);
            this.generalPanel.Controls.Add(this.line3);
            this.generalPanel.Controls.Add(this.line1);
            this.generalPanel.Controls.Add(this.environmentInfoLabel);
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
            this.generalPanel.Location = new System.Drawing.Point(148, 22);
            this.generalPanel.Name = "generalPanel";
            this.generalPanel.Size = new System.Drawing.Size(474, 235);
            this.generalPanel.TabIndex = 47;
            // 
            // developmentBuildNumericUpDown
            // 
            this.developmentBuildNumericUpDown.Enabled = false;
            this.developmentBuildNumericUpDown.Location = new System.Drawing.Point(363, 8);
            this.developmentBuildNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.developmentBuildNumericUpDown.Name = "developmentBuildNumericUpDown";
            this.developmentBuildNumericUpDown.Size = new System.Drawing.Size(43, 22);
            this.developmentBuildNumericUpDown.TabIndex = 25;
            this.developmentBuildNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(10, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(439, 39);
            this.label1.TabIndex = 24;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // mustUpdateCheckBox
            // 
            this.mustUpdateCheckBox.AutoSize = true;
            this.mustUpdateCheckBox.Location = new System.Drawing.Point(12, 186);
            this.mustUpdateCheckBox.Name = "mustUpdateCheckBox";
            this.mustUpdateCheckBox.Size = new System.Drawing.Size(115, 17);
            this.mustUpdateCheckBox.TabIndex = 23;
            this.mustUpdateCheckBox.Text = "Must be installed";
            this.mustUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // environmentInfoLabel
            // 
            this.environmentInfoLabel.AutoSize = true;
            this.environmentInfoLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.environmentInfoLabel.Location = new System.Drawing.Point(9, 136);
            this.environmentInfoLabel.Name = "environmentInfoLabel";
            this.environmentInfoLabel.Size = new System.Drawing.Size(438, 26);
            this.environmentInfoLabel.TabIndex = 20;
            this.environmentInfoLabel.Text = "Sets if the update package should only run on special architectures. To set any t" +
    "ype \r\nof architecture, choose \"AnyCPU\" as entry.";
            // 
            // buildNumericUpDown
            // 
            this.buildNumericUpDown.Location = new System.Drawing.Point(331, 35);
            this.buildNumericUpDown.Name = "buildNumericUpDown";
            this.buildNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.buildNumericUpDown.TabIndex = 5;
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
            this.environmentLabel.Location = new System.Drawing.Point(8, 112);
            this.environmentLabel.Name = "environmentLabel";
            this.environmentLabel.Size = new System.Drawing.Size(116, 13);
            this.environmentLabel.TabIndex = 18;
            this.environmentLabel.Text = "Architecture settings:";
            // 
            // majorNumericUpDown
            // 
            this.majorNumericUpDown.Location = new System.Drawing.Point(207, 35);
            this.majorNumericUpDown.Name = "majorNumericUpDown";
            this.majorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.majorNumericUpDown.TabIndex = 3;
            // 
            // architectureComboBox
            // 
            this.architectureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.architectureComboBox.FormattingEnabled = true;
            this.architectureComboBox.Items.AddRange(new object[] {
            "x86 (32-bit)",
            "x64 (64-bit)",
            "AnyCPU (independent)"});
            this.architectureComboBox.Location = new System.Drawing.Point(207, 108);
            this.architectureComboBox.Name = "architectureComboBox";
            this.architectureComboBox.Size = new System.Drawing.Size(150, 21);
            this.architectureComboBox.TabIndex = 17;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(207, 66);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(240, 22);
            this.descriptionTextBox.TabIndex = 11;
            // 
            // revisionNumericUpDown
            // 
            this.revisionNumericUpDown.Location = new System.Drawing.Point(391, 35);
            this.revisionNumericUpDown.Name = "revisionNumericUpDown";
            this.revisionNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.revisionNumericUpDown.TabIndex = 6;
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
            this.developmentalStageComboBox.Items.AddRange(new object[] {
            "Alpha",
            "Beta",
            "Release"});
            this.developmentalStageComboBox.Location = new System.Drawing.Point(207, 8);
            this.developmentalStageComboBox.Name = "developmentalStageComboBox";
            this.developmentalStageComboBox.Size = new System.Drawing.Size(150, 21);
            this.developmentalStageComboBox.TabIndex = 0;
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
            this.minorNumericUpDown.Name = "minorNumericUpDown";
            this.minorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.minorNumericUpDown.TabIndex = 4;
            // 
            // filesList
            // 
            this.filesList.BackColor = System.Drawing.SystemColors.Window;
            this.filesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.filesList.FullRowSelect = true;
            this.filesList.GridLines = true;
            this.filesList.Location = new System.Drawing.Point(4, 30);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(467, 198);
            this.filesList.SmallImageList = this.filesImageList;
            this.filesList.TabIndex = 3;
            this.filesList.UseCompatibleStateImageBehavior = false;
            this.filesList.View = System.Windows.Forms.View.Details;
            // 
            // filesPanel
            // 
            this.filesPanel.Controls.Add(this.filesDataTreeView);
            this.filesPanel.Controls.Add(this.toolStrip4);
            this.filesPanel.Controls.Add(this.filesList);
            this.filesPanel.Location = new System.Drawing.Point(148, 22);
            this.filesPanel.Name = "filesPanel";
            this.filesPanel.Size = new System.Drawing.Size(474, 235);
            this.filesPanel.TabIndex = 50;
            // 
            // toolStrip4
            // 
            this.toolStrip4.AutoSize = false;
            this.toolStrip4.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip4.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip4.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFolderButton,
            this.toolStripSeparator1,
            this.addFilesButton,
            this.toolStripSeparator4,
            this.removeEntryButton});
            this.toolStrip4.Location = new System.Drawing.Point(3, 2);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip4.Size = new System.Drawing.Size(468, 25);
            this.toolStrip4.TabIndex = 4;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // addFolderButton
            // 
            this.addFolderButton.Image = ((System.Drawing.Image)(resources.GetObject("addFolderButton.Image")));
            this.addFolderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addFolderButton.Name = "addFolderButton";
            this.addFolderButton.Size = new System.Drawing.Size(83, 22);
            this.addFolderButton.Text = "Add folder";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // addFilesButton
            // 
            this.addFilesButton.Image = ((System.Drawing.Image)(resources.GetObject("addFilesButton.Image")));
            this.addFilesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addFilesButton.Name = "addFilesButton";
            this.addFilesButton.Size = new System.Drawing.Size(73, 22);
            this.addFilesButton.Text = "Add files";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // removeEntryButton
            // 
            this.removeEntryButton.Image = ((System.Drawing.Image)(resources.GetObject("removeEntryButton.Image")));
            this.removeEntryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeEntryButton.Name = "removeEntryButton";
            this.removeEntryButton.Size = new System.Drawing.Size(100, 22);
            this.removeEntryButton.Text = "Remove entry";
            // 
            // itemName
            // 
            this.itemName.Width = 300;
            // 
            // Description
            // 
            this.Description.Width = 300;
            // 
            // editPackageButton
            // 
            this.editPackageButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.editPackageButton.Location = new System.Drawing.Point(497, 8);
            this.editPackageButton.Name = "editPackageButton";
            this.editPackageButton.Size = new System.Drawing.Size(121, 23);
            this.editPackageButton.TabIndex = 2;
            this.editPackageButton.Text = "Edit package";
            this.editPackageButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(416, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // operationsListView
            // 
            this.operationsListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.operationsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.operationsListView.FullRowSelect = true;
            listViewGroup1.Header = "Files";
            listViewGroup1.Name = "filesGroup";
            listViewGroup2.Header = "Registry";
            listViewGroup2.Name = "registryGroup";
            listViewGroup3.Header = "Processes";
            listViewGroup3.Name = "processGroup";
            listViewGroup4.Header = "Services";
            listViewGroup4.Name = "serviceGroup";
            this.operationsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4});
            this.operationsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewItem1.Group = listViewGroup1;
            listViewItem1.Tag = "RenameFile";
            listViewItem2.Group = listViewGroup1;
            listViewItem2.Tag = "DeleteFile";
            listViewItem3.Group = listViewGroup2;
            listViewItem3.Tag = "CreateRegistryEntry";
            listViewItem4.Group = listViewGroup2;
            listViewItem4.Tag = "DeleteRegistryEntry";
            listViewItem5.Group = listViewGroup2;
            listViewItem5.Tag = "SetRegistryKeyValue";
            listViewItem6.Group = listViewGroup3;
            listViewItem6.Tag = "StartProcess";
            listViewItem7.Group = listViewGroup3;
            listViewItem7.Tag = "TerminateProcess";
            listViewItem8.Group = listViewGroup4;
            listViewItem8.Tag = "StartService";
            listViewItem9.Group = listViewGroup4;
            listViewItem9.Tag = "StopService";
            this.operationsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.operationsListView.LargeImageList = this.categoryImageList;
            this.operationsListView.Location = new System.Drawing.Point(0, 2);
            this.operationsListView.MultiSelect = false;
            this.operationsListView.Name = "operationsListView";
            this.operationsListView.Size = new System.Drawing.Size(474, 231);
            this.operationsListView.SmallImageList = this.categoryImageList;
            this.operationsListView.TabIndex = 2;
            this.operationsListView.TileSize = new System.Drawing.Size(450, 50);
            this.operationsListView.UseCompatibleStateImageBehavior = false;
            this.operationsListView.View = System.Windows.Forms.View.Tile;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 300;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 300;
            // 
            // line3
            // 
            this.line3.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line3.Location = new System.Drawing.Point(10, 169);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(437, 10);
            this.line3.TabIndex = 22;
            this.line3.Text = "line3";
            // 
            // line1
            // 
            this.line1.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(7, 91);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(437, 14);
            this.line1.TabIndex = 21;
            this.line1.Text = "line1";
            // 
            // categoryTreeView
            // 
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
            treeNode5.ImageIndex = 2;
            treeNode5.Name = "generalNode";
            treeNode5.SelectedImageIndex = 2;
            treeNode5.Text = "General";
            treeNode6.ImageIndex = 3;
            treeNode6.Name = "changelogNode";
            treeNode6.SelectedImageIndex = 3;
            treeNode6.Text = "Changelog";
            treeNode7.ImageIndex = 0;
            treeNode7.Name = "availabilityNode";
            treeNode7.SelectedImageIndex = 0;
            treeNode7.Text = "Availability";
            treeNode8.ImageIndex = 4;
            treeNode8.Name = "operationsNode";
            treeNode8.SelectedImageIndex = 4;
            treeNode8.Text = "Operations";
            this.categoryTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8});
            this.categoryTreeView.SelectedImageIndex = 0;
            this.categoryTreeView.ShowLines = false;
            this.categoryTreeView.Size = new System.Drawing.Size(129, 231);
            this.categoryTreeView.TabIndex = 45;
            this.categoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.categoryTreeView_AfterSelect);
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.editPackageButton);
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 263);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(632, 40);
            this.controlPanel1.TabIndex = 51;
            // 
            // filesDataTreeView
            // 
            this.filesDataTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filesDataTreeView.HotTracking = true;
            this.filesDataTreeView.ImageIndex = 0;
            this.filesDataTreeView.ImageList = this.filesImageList;
            this.filesDataTreeView.ItemHeight = 23;
            this.filesDataTreeView.Location = new System.Drawing.Point(3, 30);
            this.filesDataTreeView.Name = "filesDataTreeView";
            treeNode1.ImageIndex = 2;
            treeNode1.Name = "Knoten0";
            treeNode1.SelectedImageIndex = 2;
            treeNode1.Text = "Program directory";
            treeNode2.ImageIndex = 2;
            treeNode2.Name = "Knoten1";
            treeNode2.SelectedImageIndex = 2;
            treeNode2.Text = "AppData";
            treeNode3.ImageIndex = 2;
            treeNode3.Name = "Knoten2";
            treeNode3.SelectedImageIndex = 2;
            treeNode3.Text = "Temp directory";
            treeNode4.ImageIndex = 2;
            treeNode4.Name = "Knoten3";
            treeNode4.SelectedImageIndex = 2;
            treeNode4.Text = "Desktop";
            this.filesDataTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.filesDataTreeView.SelectedImageIndex = 0;
            this.filesDataTreeView.ShowLines = false;
            this.filesDataTreeView.Size = new System.Drawing.Size(469, 198);
            this.filesDataTreeView.TabIndex = 5;
            // 
            // PackageEditDialog
            // 
            this.AcceptButton = this.editPackageButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(632, 303);
            this.Controls.Add(this.generalPanel);
            this.Controls.Add(this.operationsPanel);
            this.Controls.Add(this.categoryTreeView);
            this.Controls.Add(this.availabilityPanel);
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.changelogPanel);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.filesPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackageEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit update package - {0} - {1}";
            this.Load += new System.EventHandler(this.PackageEditDialog_Load);
            this.operationsPanel.ResumeLayout(false);
            this.availabilityPanel.ResumeLayout(false);
            this.availabilityPanel.PerformLayout();
            this.unsupportedVersionsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMajorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedRevisionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedMinorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unsupportedBuildNumericUpDown)).EndInit();
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.changelogPanel.ResumeLayout(false);
            this.changelogPanel.PerformLayout();
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.generalPanel.ResumeLayout(false);
            this.generalPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.developmentBuildNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.revisionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minorNumericUpDown)).EndInit();
            this.filesPanel.ResumeLayout(false);
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel operationsPanel;
        private System.Windows.Forms.ColumnHeader itemName;
        private System.Windows.Forms.ColumnHeader Description;
        private System.Windows.Forms.ImageList categoryImageList;
        private Controls.ExplorerTreeView categoryTreeView;
        private System.Windows.Forms.Panel availabilityPanel;
        private System.Windows.Forms.Label someVersionsInfoLabel;
        private System.Windows.Forms.RadioButton allVersionsRadioButton;
        private System.Windows.Forms.Label allVersionsInfoLabel;
        private System.Windows.Forms.Panel unsupportedVersionsPanel;
        private System.Windows.Forms.NumericUpDown unsupportedMajorNumericUpDown;
        private System.Windows.Forms.Button addVersionButton;
        private System.Windows.Forms.Button removeVersionButton;
        private System.Windows.Forms.NumericUpDown unsupportedRevisionNumericUpDown;
        private System.Windows.Forms.ListBox unsupportedVersionsList;
        private System.Windows.Forms.NumericUpDown unsupportedMinorNumericUpDown;
        private System.Windows.Forms.NumericUpDown unsupportedBuildNumericUpDown;
        private System.Windows.Forms.RadioButton someVersionsRadioButton;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.Panel changelogPanel;
        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton changelogLoadButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton changelogClearButton;
        private System.Windows.Forms.TextBox changelogTextBox;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ImageList filesImageList;
        private System.Windows.Forms.Panel generalPanel;
        private System.Windows.Forms.NumericUpDown developmentBuildNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox mustUpdateCheckBox;
        private Controls.Line line3;
        private Controls.Line line1;
        private System.Windows.Forms.Label environmentInfoLabel;
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
        private System.Windows.Forms.ListView filesList;
        private System.Windows.Forms.Panel filesPanel;
        private Controls.ExplorerTreeView filesDataTreeView;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton addFolderButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton addFilesButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton removeEntryButton;
        private Controls.ExtendedListView operationsListView;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button editPackageButton;
        private System.Windows.Forms.Button cancelButton;


    }
}