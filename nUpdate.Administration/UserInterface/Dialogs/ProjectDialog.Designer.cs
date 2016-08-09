using nUpdate.Administration.UserInterface.Controls;
using nUpdate.UI.WinForms.Controls;

namespace nUpdate.Administration.UserInterface.Dialogs
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.line2 = new nUpdate.UI.WinForms.Controls.Line();
            this.masterChannelHeader = new System.Windows.Forms.Label();
            this.checkMasterChannelLinkLabel = new System.Windows.Forms.LinkLabel();
            this.line1 = new nUpdate.UI.WinForms.Controls.Line();
            this.overviewHeader = new System.Windows.Forms.Label();
            this.newestPackageLabel = new System.Windows.Forms.Label();
            this.amountLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.updateUriLabel = new System.Windows.Forms.Label();
            this.newestPackageReleasedLabel = new System.Windows.Forms.Label();
            this.releasedPackagesAmountLabel = new System.Windows.Forms.Label();
            this.publicKeyLabel = new System.Windows.Forms.Label();
            this.projectIdLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.updateUriTextBox = new System.Windows.Forms.TextBox();
            this.publicKeyTextBox = new System.Windows.Forms.TextBox();
            this.projectIdTextBox = new System.Windows.Forms.TextBox();
            this.packageListView = new nUpdate.Administration.UserInterface.Controls.ExplorerListView();
            this.versionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.releaseDateHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.necessaryHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.searchTextBox = new nUpdate.Administration.UserInterface.Controls.CueTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addButton = new System.Windows.Forms.ToolStripSplitButton();
            this.addPackageToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.packageFromTemplateToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.addTemplateToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.uploadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.historyButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.packageStatusStrip = new System.Windows.Forms.StatusStrip();
            this.activeTaskLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.packageTaskProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.placeHolderLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.packagesCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.packagesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.cancelLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.cancelToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.overviewContentSwitch = new nUpdate.Administration.UserInterface.Controls.ContentSwitch();
            this.packagesContentSwitch = new nUpdate.Administration.UserInterface.Controls.ContentSwitch();
            this.tabControl = new nUpdate.Administration.UserInterface.Controls.TablessTabControl();
            this.overviewTabPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.line3 = new nUpdate.UI.WinForms.Controls.Line();
            this.tickPictureBox = new System.Windows.Forms.PictureBox();
            this.packageTabPage = new System.Windows.Forms.TabPage();
            this.statisticsContentSwitch = new nUpdate.Administration.UserInterface.Controls.ContentSwitch();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.packageStatusStrip.SuspendLayout();
            this.packagesContextMenuStrip.SuspendLayout();
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.overviewTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tickPictureBox)).BeginInit();
            this.packageTabPage.SuspendLayout();
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
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(52, 330);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(444, 41);
            this.label4.TabIndex = 77;
            this.label4.Text = "The MasterChannel contains all available update channels in this project to manag" +
    "e the package exchange and the comparison of different package versions in your " +
    "project.";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(32, 329);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(16, 16);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 76;
            this.pictureBox3.TabStop = false;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.White;
            this.line2.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            this.line2.Location = new System.Drawing.Point(126, 280);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(393, 10);
            this.line2.TabIndex = 75;
            this.line2.Text = "line2";
            // 
            // masterChannelHeader
            // 
            this.masterChannelHeader.AutoSize = true;
            this.masterChannelHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.masterChannelHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.masterChannelHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.masterChannelHeader.Location = new System.Drawing.Point(15, 274);
            this.masterChannelHeader.Name = "masterChannelHeader";
            this.masterChannelHeader.Size = new System.Drawing.Size(107, 20);
            this.masterChannelHeader.TabIndex = 74;
            this.masterChannelHeader.Text = "MasterChannel";
            // 
            // checkMasterChannelLinkLabel
            // 
            this.checkMasterChannelLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.checkMasterChannelLinkLabel.AutoSize = true;
            this.checkMasterChannelLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.checkMasterChannelLinkLabel.Location = new System.Drawing.Point(217, 310);
            this.checkMasterChannelLinkLabel.Name = "checkMasterChannelLinkLabel";
            this.checkMasterChannelLinkLabel.Size = new System.Drawing.Size(75, 13);
            this.checkMasterChannelLinkLabel.TabIndex = 26;
            this.checkMasterChannelLinkLabel.TabStop = true;
            this.checkMasterChannelLinkLabel.Text = "Check status ";
            this.checkMasterChannelLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.checkMasterChannelnLinkLabel_LinkClicked);
            // 
            // line1
            // 
            this.line1.BackColor = System.Drawing.Color.White;
            this.line1.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(85, 20);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(434, 10);
            this.line1.TabIndex = 11;
            this.line1.Text = "line1";
            // 
            // overviewHeader
            // 
            this.overviewHeader.AutoSize = true;
            this.overviewHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.overviewHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.overviewHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.overviewHeader.Location = new System.Drawing.Point(15, 13);
            this.overviewHeader.Name = "overviewHeader";
            this.overviewHeader.Size = new System.Drawing.Size(70, 20);
            this.overviewHeader.TabIndex = 8;
            this.overviewHeader.Text = "Overview";
            // 
            // newestPackageLabel
            // 
            this.newestPackageLabel.AutoSize = true;
            this.newestPackageLabel.Location = new System.Drawing.Point(217, 228);
            this.newestPackageLabel.Name = "newestPackageLabel";
            this.newestPackageLabel.Size = new System.Drawing.Size(26, 13);
            this.newestPackageLabel.TabIndex = 28;
            this.newestPackageLabel.Text = "N/A";
            // 
            // amountLabel
            // 
            this.amountLabel.AutoSize = true;
            this.amountLabel.Location = new System.Drawing.Point(217, 201);
            this.amountLabel.Name = "amountLabel";
            this.amountLabel.Size = new System.Drawing.Size(26, 13);
            this.amountLabel.TabIndex = 27;
            this.amountLabel.Text = "N/A";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(28, 52);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 13);
            this.nameLabel.TabIndex = 19;
            this.nameLabel.Text = "Name:";
            // 
            // updateUriLabel
            // 
            this.updateUriLabel.AutoSize = true;
            this.updateUriLabel.Location = new System.Drawing.Point(28, 82);
            this.updateUriLabel.Name = "updateUriLabel";
            this.updateUriLabel.Size = new System.Drawing.Size(96, 13);
            this.updateUriLabel.TabIndex = 18;
            this.updateUriLabel.Text = "Update directory:";
            // 
            // newestPackageReleasedLabel
            // 
            this.newestPackageReleasedLabel.AutoSize = true;
            this.newestPackageReleasedLabel.Location = new System.Drawing.Point(29, 228);
            this.newestPackageReleasedLabel.Name = "newestPackageReleasedLabel";
            this.newestPackageReleasedLabel.Size = new System.Drawing.Size(140, 13);
            this.newestPackageReleasedLabel.TabIndex = 7;
            this.newestPackageReleasedLabel.Text = "Newest package released:";
            // 
            // releasedPackagesAmountLabel
            // 
            this.releasedPackagesAmountLabel.AutoSize = true;
            this.releasedPackagesAmountLabel.Location = new System.Drawing.Point(29, 201);
            this.releasedPackagesAmountLabel.Name = "releasedPackagesAmountLabel";
            this.releasedPackagesAmountLabel.Size = new System.Drawing.Size(162, 13);
            this.releasedPackagesAmountLabel.TabIndex = 6;
            this.releasedPackagesAmountLabel.Text = "Amount of released packages:";
            // 
            // publicKeyLabel
            // 
            this.publicKeyLabel.AutoSize = true;
            this.publicKeyLabel.Location = new System.Drawing.Point(28, 111);
            this.publicKeyLabel.Name = "publicKeyLabel";
            this.publicKeyLabel.Size = new System.Drawing.Size(61, 13);
            this.publicKeyLabel.TabIndex = 14;
            this.publicKeyLabel.Text = "Public Key:";
            // 
            // projectIdLabel
            // 
            this.projectIdLabel.AutoSize = true;
            this.projectIdLabel.Location = new System.Drawing.Point(29, 140);
            this.projectIdLabel.Name = "projectIdLabel";
            this.projectIdLabel.Size = new System.Drawing.Size(37, 13);
            this.projectIdLabel.TabIndex = 16;
            this.projectIdLabel.Text = "GUID:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.nameTextBox.Location = new System.Drawing.Point(220, 49);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ReadOnly = true;
            this.nameTextBox.Size = new System.Drawing.Size(276, 22);
            this.nameTextBox.TabIndex = 20;
            this.nameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.readOnlyTextBox_KeyDown);
            // 
            // updateUriTextBox
            // 
            this.updateUriTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.updateUriTextBox.Location = new System.Drawing.Point(220, 79);
            this.updateUriTextBox.Name = "updateUriTextBox";
            this.updateUriTextBox.ReadOnly = true;
            this.updateUriTextBox.Size = new System.Drawing.Size(276, 22);
            this.updateUriTextBox.TabIndex = 21;
            this.updateUriTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.readOnlyTextBox_KeyDown);
            // 
            // publicKeyTextBox
            // 
            this.publicKeyTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.publicKeyTextBox.Location = new System.Drawing.Point(220, 108);
            this.publicKeyTextBox.Name = "publicKeyTextBox";
            this.publicKeyTextBox.ReadOnly = true;
            this.publicKeyTextBox.Size = new System.Drawing.Size(276, 22);
            this.publicKeyTextBox.TabIndex = 15;
            this.publicKeyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.readOnlyTextBox_KeyDown);
            // 
            // projectIdTextBox
            // 
            this.projectIdTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.projectIdTextBox.Location = new System.Drawing.Point(220, 137);
            this.projectIdTextBox.Name = "projectIdTextBox";
            this.projectIdTextBox.ReadOnly = true;
            this.projectIdTextBox.Size = new System.Drawing.Size(276, 22);
            this.projectIdTextBox.TabIndex = 17;
            this.projectIdTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.readOnlyTextBox_KeyDown);
            // 
            // packageListView
            // 
            this.packageListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packageListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packageListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.versionHeader,
            this.descriptionHeader,
            this.releaseDateHeader,
            this.necessaryHeader});
            this.packageListView.FullRowSelect = true;
            listViewGroup1.Header = "ListViewGroup";
            listViewGroup1.Name = "notReleasedGroup";
            listViewGroup2.Header = "ListViewGroup";
            listViewGroup2.Name = "releasedGroup";
            this.packageListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.packageListView.Location = new System.Drawing.Point(-1, 34);
            this.packageListView.Name = "packageListView";
            this.packageListView.Size = new System.Drawing.Size(525, 369);
            this.packageListView.TabIndex = 6;
            this.packageListView.UseCompatibleStateImageBehavior = false;
            this.packageListView.View = System.Windows.Forms.View.Details;
            this.packageListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.packageListView_ItemSelectionChanged);
            // 
            // versionHeader
            // 
            this.versionHeader.Text = "Package version";
            this.versionHeader.Width = 156;
            // 
            // descriptionHeader
            // 
            this.descriptionHeader.Text = "Description";
            this.descriptionHeader.Width = 164;
            // 
            // releaseDateHeader
            // 
            this.releaseDateHeader.Text = "Release Date";
            this.releaseDateHeader.Width = 98;
            // 
            // necessaryHeader
            // 
            this.necessaryHeader.Text = "Necessary";
            this.necessaryHeader.Width = 105;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.Cue = "Search...";
            this.searchTextBox.Location = new System.Drawing.Point(341, 6);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(179, 22);
            this.searchTextBox.TabIndex = 2;
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
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
            this.toolStripSeparator6});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(517, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addButton
            // 
            this.addButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPackageToolStripButton,
            this.packageFromTemplateToolStripButton,
            this.addTemplateToolStripButton});
            this.addButton.Image = ((System.Drawing.Image)(resources.GetObject("addButton.Image")));
            this.addButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(61, 25);
            this.addButton.Text = "Add";
            this.addButton.ButtonClick += new System.EventHandler(this.addButton_Click);
            // 
            // addPackageToolStripButton
            // 
            this.addPackageToolStripButton.Name = "addPackageToolStripButton";
            this.addPackageToolStripButton.Size = new System.Drawing.Size(222, 22);
            this.addPackageToolStripButton.Text = "Add package";
            this.addPackageToolStripButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // packageFromTemplateToolStripButton
            // 
            this.packageFromTemplateToolStripButton.Name = "packageFromTemplateToolStripButton";
            this.packageFromTemplateToolStripButton.Size = new System.Drawing.Size(222, 22);
            this.packageFromTemplateToolStripButton.Text = "Add package from template";
            this.packageFromTemplateToolStripButton.Click += new System.EventHandler(this.packageFromTemplateToolStripButton_Click);
            // 
            // addTemplateToolStripButton
            // 
            this.addTemplateToolStripButton.Name = "addTemplateToolStripButton";
            this.addTemplateToolStripButton.Size = new System.Drawing.Size(222, 22);
            this.addTemplateToolStripButton.Text = "Add template";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // editButton
            // 
            this.editButton.AutoToolTip = false;
            this.editButton.Enabled = false;
            this.editButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.editButton.Image = ((System.Drawing.Image)(resources.GetObject("editButton.Image")));
            this.editButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(50, 25);
            this.editButton.Text = "Edit ";
            this.editButton.ToolTipText = "Edit the configuration of the selected package.";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // deleteButton
            // 
            this.deleteButton.AutoToolTip = false;
            this.deleteButton.Enabled = false;
            this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(63, 25);
            this.deleteButton.Text = "Delete ";
            this.deleteButton.ToolTipText = "Delete the selected package from the server and/or locally.";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // uploadButton
            // 
            this.uploadButton.AutoToolTip = false;
            this.uploadButton.Enabled = false;
            this.uploadButton.Image = ((System.Drawing.Image)(resources.GetObject("uploadButton.Image")));
            this.uploadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(68, 25);
            this.uploadButton.Text = "Upload ";
            this.uploadButton.ToolTipText = "Upload the selected package onto the server and make it available for all clients" +
    ".";
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // historyButton
            // 
            this.historyButton.AutoToolTip = false;
            this.historyButton.Image = ((System.Drawing.Image)(resources.GetObject("historyButton.Image")));
            this.historyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.historyButton.Name = "historyButton";
            this.historyButton.Size = new System.Drawing.Size(65, 25);
            this.historyButton.Text = "History";
            this.historyButton.ToolTipText = "Load the history of the packages.";
            this.historyButton.Click += new System.EventHandler(this.historyButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 28);
            // 
            // packageStatusStrip
            // 
            this.packageStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activeTaskLabel,
            this.packageTaskProgressBar,
            this.placeHolderLabel,
            this.packagesCountLabel});
            this.packageStatusStrip.Location = new System.Drawing.Point(0, 454);
            this.packageStatusStrip.Name = "packageStatusStrip";
            this.packageStatusStrip.Size = new System.Drawing.Size(672, 22);
            this.packageStatusStrip.SizingGrip = false;
            this.packageStatusStrip.TabIndex = 4;
            // 
            // activeTaskLabel
            // 
            this.activeTaskLabel.BackColor = System.Drawing.Color.Transparent;
            this.activeTaskLabel.Name = "activeTaskLabel";
            this.activeTaskLabel.Size = new System.Drawing.Size(89, 17);
            this.activeTaskLabel.Text = "No active tasks.";
            // 
            // packageTaskProgressBar
            // 
            this.packageTaskProgressBar.Name = "packageTaskProgressBar";
            this.packageTaskProgressBar.Size = new System.Drawing.Size(100, 16);
            this.packageTaskProgressBar.Visible = false;
            // 
            // placeHolderLabel
            // 
            this.placeHolderLabel.BackColor = System.Drawing.Color.Transparent;
            this.placeHolderLabel.Name = "placeHolderLabel";
            this.placeHolderLabel.Size = new System.Drawing.Size(460, 17);
            this.placeHolderLabel.Spring = true;
            // 
            // packagesCountLabel
            // 
            this.packagesCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.packagesCountLabel.Name = "packagesCountLabel";
            this.packagesCountLabel.Size = new System.Drawing.Size(108, 17);
            this.packagesCountLabel.Text = "0 update packages.";
            // 
            // packagesContextMenuStrip
            // 
            this.packagesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.uploadToolStripMenuItem});
            this.packagesContextMenuStrip.Name = "packagesContextMenuStrip";
            this.packagesContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.packagesContextMenuStrip.Size = new System.Drawing.Size(113, 92);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addToolStripMenuItem.Image")));
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Enabled = false;
            this.editToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editToolStripMenuItem.Image")));
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // uploadToolStripMenuItem
            // 
            this.uploadToolStripMenuItem.Enabled = false;
            this.uploadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("uploadToolStripMenuItem.Image")));
            this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            this.uploadToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.uploadToolStripMenuItem.Text = "Upload";
            this.uploadToolStripMenuItem.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // loadingPanel
            // 
            this.loadingPanel.BackColor = System.Drawing.Color.White;
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.cancelLabel);
            this.loadingPanel.Controls.Add(this.pictureBox1);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Location = new System.Drawing.Point(148, 480);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(269, 51);
            this.loadingPanel.TabIndex = 66;
            this.loadingPanel.Visible = false;
            // 
            // cancelLabel
            // 
            this.cancelLabel.AutoSize = true;
            this.cancelLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.cancelLabel.Location = new System.Drawing.Point(252, 0);
            this.cancelLabel.Name = "cancelLabel";
            this.cancelLabel.Size = new System.Drawing.Size(14, 17);
            this.cancelLabel.TabIndex = 22;
            this.cancelLabel.Text = "x";
            this.cancelLabel.Visible = false;
            this.cancelLabel.Click += new System.EventHandler(this.cancelLabel_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(16, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoEllipsis = true;
            this.loadingLabel.Location = new System.Drawing.Point(34, 19);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(216, 15);
            this.loadingLabel.TabIndex = 11;
            this.loadingLabel.Text = "Waiting for thread...";
            // 
            // cancelToolTip
            // 
            this.cancelToolTip.IsBalloon = true;
            this.cancelToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.cancelToolTip.ToolTipTitle = "Cancel the upload.";
            // 
            // overviewContentSwitch
            // 
            this.overviewContentSwitch.Checked = true;
            this.overviewContentSwitch.Location = new System.Drawing.Point(17, 32);
            this.overviewContentSwitch.Name = "overviewContentSwitch";
            this.overviewContentSwitch.Size = new System.Drawing.Size(113, 29);
            this.overviewContentSwitch.TabIndex = 67;
            this.overviewContentSwitch.TabStop = true;
            this.overviewContentSwitch.Text = "Overview";
            this.overviewContentSwitch.UseVisualStyleBackColor = true;
            this.overviewContentSwitch.CheckedChanged += new System.EventHandler(this.overviewContentSwitch_CheckedChanged);
            // 
            // packagesContentSwitch
            // 
            this.packagesContentSwitch.Location = new System.Drawing.Point(17, 70);
            this.packagesContentSwitch.Name = "packagesContentSwitch";
            this.packagesContentSwitch.Size = new System.Drawing.Size(113, 29);
            this.packagesContentSwitch.TabIndex = 68;
            this.packagesContentSwitch.Text = "Packages";
            this.packagesContentSwitch.UseVisualStyleBackColor = true;
            this.packagesContentSwitch.CheckedChanged += new System.EventHandler(this.overviewContentSwitch_CheckedChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.overviewTabPage);
            this.tabControl.Controls.Add(this.packageTabPage);
            this.tabControl.Location = new System.Drawing.Point(129, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(533, 430);
            this.tabControl.TabIndex = 69;
            // 
            // overviewTabPage
            // 
            this.overviewTabPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.overviewTabPage.Controls.Add(this.label2);
            this.overviewTabPage.Controls.Add(this.line3);
            this.overviewTabPage.Controls.Add(this.newestPackageLabel);
            this.overviewTabPage.Controls.Add(this.pictureBox3);
            this.overviewTabPage.Controls.Add(this.newestPackageReleasedLabel);
            this.overviewTabPage.Controls.Add(this.amountLabel);
            this.overviewTabPage.Controls.Add(this.label4);
            this.overviewTabPage.Controls.Add(this.updateUriLabel);
            this.overviewTabPage.Controls.Add(this.releasedPackagesAmountLabel);
            this.overviewTabPage.Controls.Add(this.nameLabel);
            this.overviewTabPage.Controls.Add(this.projectIdTextBox);
            this.overviewTabPage.Controls.Add(this.projectIdLabel);
            this.overviewTabPage.Controls.Add(this.publicKeyLabel);
            this.overviewTabPage.Controls.Add(this.publicKeyTextBox);
            this.overviewTabPage.Controls.Add(this.overviewHeader);
            this.overviewTabPage.Controls.Add(this.updateUriTextBox);
            this.overviewTabPage.Controls.Add(this.line2);
            this.overviewTabPage.Controls.Add(this.masterChannelHeader);
            this.overviewTabPage.Controls.Add(this.nameTextBox);
            this.overviewTabPage.Controls.Add(this.line1);
            this.overviewTabPage.Controls.Add(this.checkMasterChannelLinkLabel);
            this.overviewTabPage.Controls.Add(this.tickPictureBox);
            this.overviewTabPage.Location = new System.Drawing.Point(4, 22);
            this.overviewTabPage.Name = "overviewTabPage";
            this.overviewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.overviewTabPage.Size = new System.Drawing.Size(525, 404);
            this.overviewTabPage.TabIndex = 0;
            this.overviewTabPage.Text = "overviewTabPage";
            this.overviewTabPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 310);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 13);
            this.label2.TabIndex = 79;
            this.label2.Text = "Status of the MasterChannel file:";
            // 
            // line3
            // 
            this.line3.BackColor = System.Drawing.Color.White;
            this.line3.LineAlignment = nUpdate.UI.WinForms.Controls.Line.Alignment.Horizontal;
            this.line3.Location = new System.Drawing.Point(23, 176);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(478, 10);
            this.line3.TabIndex = 78;
            this.line3.Text = "line3";
            // 
            // tickPictureBox
            // 
            this.tickPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("tickPictureBox.Image")));
            this.tickPictureBox.Location = new System.Drawing.Point(294, 309);
            this.tickPictureBox.Name = "tickPictureBox";
            this.tickPictureBox.Size = new System.Drawing.Size(16, 16);
            this.tickPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tickPictureBox.TabIndex = 30;
            this.tickPictureBox.TabStop = false;
            this.tickPictureBox.Visible = false;
            // 
            // packageTabPage
            // 
            this.packageTabPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packageTabPage.Controls.Add(this.packageListView);
            this.packageTabPage.Controls.Add(this.searchTextBox);
            this.packageTabPage.Controls.Add(this.toolStrip1);
            this.packageTabPage.Location = new System.Drawing.Point(4, 22);
            this.packageTabPage.Name = "packageTabPage";
            this.packageTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.packageTabPage.Size = new System.Drawing.Size(525, 404);
            this.packageTabPage.TabIndex = 1;
            this.packageTabPage.Text = "packageTabPage";
            this.packageTabPage.UseVisualStyleBackColor = true;
            // 
            // statisticsContentSwitch
            // 
            this.statisticsContentSwitch.Location = new System.Drawing.Point(17, 108);
            this.statisticsContentSwitch.Name = "statisticsContentSwitch";
            this.statisticsContentSwitch.Size = new System.Drawing.Size(113, 29);
            this.statisticsContentSwitch.TabIndex = 70;
            this.statisticsContentSwitch.Text = "Statistics";
            this.statisticsContentSwitch.UseVisualStyleBackColor = true;
            this.statisticsContentSwitch.CheckedChanged += new System.EventHandler(this.overviewContentSwitch_CheckedChanged);
            // 
            // ProjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(672, 476);
            this.Controls.Add(this.statisticsContentSwitch);
            this.Controls.Add(this.packagesContentSwitch);
            this.Controls.Add(this.overviewContentSwitch);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.packageStatusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProjectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{0} - {1}";
            this.Shown += new System.EventHandler(this.ProjectDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.packageStatusStrip.ResumeLayout(false);
            this.packageStatusStrip.PerformLayout();
            this.packagesContextMenuStrip.ResumeLayout(false);
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.overviewTabPage.ResumeLayout(false);
            this.overviewTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tickPictureBox)).EndInit();
            this.packageTabPage.ResumeLayout(false);
            this.packageTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton editButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private CueTextBox searchTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton uploadButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton historyButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label newestPackageReleasedLabel;
        private System.Windows.Forms.Label releasedPackagesAmountLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label updateUriLabel;
        private System.Windows.Forms.TextBox projectIdTextBox;
        private System.Windows.Forms.Label projectIdLabel;
        private System.Windows.Forms.TextBox publicKeyTextBox;
        private System.Windows.Forms.Label publicKeyLabel;
        private Line line1;
        private System.Windows.Forms.Label overviewHeader;
        private System.Windows.Forms.TextBox updateUriTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label newestPackageLabel;
        private System.Windows.Forms.Label amountLabel;
        private System.Windows.Forms.LinkLabel checkMasterChannelLinkLabel;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.Label cancelLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.ToolTip cancelToolTip;
        private System.Windows.Forms.ContextMenuStrip packagesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton addButton;
        private System.Windows.Forms.ToolStripMenuItem addPackageToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem packageFromTemplateToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem addTemplateToolStripButton;
        private Line line2;
        private System.Windows.Forms.Label masterChannelHeader;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.StatusStrip packageStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel activeTaskLabel;
        private System.Windows.Forms.ToolStripProgressBar packageTaskProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel placeHolderLabel;
        private System.Windows.Forms.ToolStripStatusLabel packagesCountLabel;
        private ExplorerListView packageListView;
        private System.Windows.Forms.ColumnHeader versionHeader;
        private System.Windows.Forms.ColumnHeader descriptionHeader;
        private System.Windows.Forms.ColumnHeader releaseDateHeader;
        private System.Windows.Forms.ColumnHeader necessaryHeader;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private ContentSwitch overviewContentSwitch;
        private ContentSwitch packagesContentSwitch;
        private TablessTabControl tabControl;
        private System.Windows.Forms.TabPage overviewTabPage;
        private System.Windows.Forms.TabPage packageTabPage;
        private ContentSwitch statisticsContentSwitch;
        private Line line3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox tickPictureBox;
    }
}