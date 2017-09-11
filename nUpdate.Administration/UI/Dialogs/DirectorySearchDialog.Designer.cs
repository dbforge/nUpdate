using System.Windows.Forms;
using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class DirectorySearchDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectorySearchDialog));
            this.serverImageList = new System.Windows.Forms.ImageList(this.components);
            this.continueButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.controlPanel1 = new BottomPanel();
            this.backButton = new ExplorerNavigationButton.ExplorerNavigationButton();
            this.forwardButton = new ExplorerNavigationButton.ExplorerNavigationButton();
            this.serverDataTreeView = new nUpdate.Administration.UI.Controls.ExplorerTreeView();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.directoryLabel = new System.Windows.Forms.ToolStripLabel();
            this.directoryTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addDirectoryButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.removeDirectoryButton = new System.Windows.Forms.ToolStripButton();
            this.controlPanel1.SuspendLayout();
            this.loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverImageList
            // 
            this.serverImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("serverImageList.ImageStream")));
            this.serverImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.serverImageList.Images.SetKeyName(0, "server-network.png");
            this.serverImageList.Images.SetKeyName(1, "folder.png");
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(265, 8);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(346, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.continueButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 223);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(434, 38);
            this.controlPanel1.TabIndex = 19;
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Black;
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(3, 6);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(24, 24);
            this.backButton.TabIndex = 21;
            this.backButton.Text = "explorerNavigationButton1";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // forwardButton
            // 
            this.forwardButton.ArrowDirection = ExplorerNavigationButton.ArrowDirection.Right;
            this.forwardButton.BackColor = System.Drawing.Color.Black;
            this.forwardButton.Enabled = false;
            this.forwardButton.Location = new System.Drawing.Point(33, 6);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(24, 24);
            this.forwardButton.TabIndex = 22;
            this.forwardButton.Text = "explorerNavigationButton2";
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // serverDataTreeView
            // 
            this.serverDataTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverDataTreeView.HideSelection = false;
            this.serverDataTreeView.HotTracking = true;
            this.serverDataTreeView.ImageIndex = 0;
            this.serverDataTreeView.ImageList = this.serverImageList;
            this.serverDataTreeView.ItemHeight = 23;
            this.serverDataTreeView.Location = new System.Drawing.Point(12, 76);
            this.serverDataTreeView.Name = "serverDataTreeView";
            this.serverDataTreeView.SelectedImageIndex = 0;
            this.serverDataTreeView.ShowLines = false;
            this.serverDataTreeView.Size = new System.Drawing.Size(405, 133);
            this.serverDataTreeView.TabIndex = 23;
            this.serverDataTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.serverDataTreeView_AfterLabelEdit);
            this.serverDataTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.serverDataTreeView_AfterSelect);
            // 
            // loadingPanel
            // 
            this.loadingPanel.BackColor = System.Drawing.Color.White;
            this.loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadingPanel.Controls.Add(this.pictureBox1);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Location = new System.Drawing.Point(88, 89);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(269, 51);
            this.loadingPanel.TabIndex = 67;
            this.loadingPanel.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(14, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoEllipsis = true;
            this.loadingLabel.Location = new System.Drawing.Point(34, 19);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(217, 15);
            this.loadingLabel.TabIndex = 11;
            this.loadingLabel.Text = "Waiting for thread...";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.directoryLabel,
            this.directoryTextBox,
            this.toolStripSeparator1,
            this.addDirectoryButton,
            this.toolStripSeparator2,
            this.removeDirectoryButton});
            this.toolStrip1.Location = new System.Drawing.Point(12, 45);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(405, 25);
            this.toolStrip1.TabIndex = 68;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // directoryLabel
            // 
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(58, 22);
            this.directoryLabel.Text = "Directory:";
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.AutoSize = false;
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(100, 25);
            this.directoryTextBox.Text = "/";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // addDirectoryButton
            // 
            this.addDirectoryButton.Image = ((System.Drawing.Image)(resources.GetObject("addDirectoryButton.Image")));
            this.addDirectoryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addDirectoryButton.Name = "addDirectoryButton";
            this.addDirectoryButton.Size = new System.Drawing.Size(99, 22);
            this.addDirectoryButton.Text = "Add directory";
            this.addDirectoryButton.Click += new System.EventHandler(this.addDirectoryButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // removeDirectoryButton
            // 
            this.removeDirectoryButton.Image = ((System.Drawing.Image)(resources.GetObject("removeDirectoryButton.Image")));
            this.removeDirectoryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeDirectoryButton.Name = "removeDirectoryButton";
            this.removeDirectoryButton.Size = new System.Drawing.Size(110, 22);
            this.removeDirectoryButton.Text = "Delete directory";
            this.removeDirectoryButton.Click += new System.EventHandler(this.removeDirectoryButton_Click);
            // 
            // DirectorySearchDialog
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.forwardButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.serverDataTreeView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DirectorySearchDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set directory - {0} - {1}";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DirectorySearchForm_FormClosing);
            this.Shown += new System.EventHandler(this.DirectorySearchDialog_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DirectorySearchDialog_MouseDown);
            this.controlPanel1.ResumeLayout(false);
            this.loadingPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.ImageList serverImageList;
        private ExplorerNavigationButton.ExplorerNavigationButton backButton;
        private ExplorerNavigationButton.ExplorerNavigationButton forwardButton;
        private Controls.ExplorerTreeView serverDataTreeView;
        private Panel loadingPanel;
        private PictureBox pictureBox1;
        private Label loadingLabel;
        private ToolStrip toolStrip1;
        private ToolStripLabel directoryLabel;
        private ToolStripTextBox directoryTextBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton addDirectoryButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton removeDirectoryButton;
    }
}