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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Server");
            this.serverImageList = new System.Windows.Forms.ImageList(this.components);
            this.continueButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loadPictureBox = new System.Windows.Forms.PictureBox();
            this.controlPanel1 = new nUpdate.UI.Controls.ControlPanel();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.ftpDirectoryInfoLabel = new System.Windows.Forms.Label();
            this.serverDataTreeView = new System.Windows.Forms.TreeView();
            this.forwardButton = new ExplorerNavigationButton.ExplorerNavigationButton();
            this.backButton = new ExplorerNavigationButton.ExplorerNavigationButton();
            ((System.ComponentModel.ISupportInitialize)(this.loadPictureBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
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
            this.continueButton.Location = new System.Drawing.Point(391, 8);
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
            this.cancelButton.Location = new System.Drawing.Point(310, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // loadPictureBox
            // 
            this.loadPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("loadPictureBox.Image")));
            this.loadPictureBox.Location = new System.Drawing.Point(446, 50);
            this.loadPictureBox.Name = "loadPictureBox";
            this.loadPictureBox.Size = new System.Drawing.Size(20, 19);
            this.loadPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.loadPictureBox.TabIndex = 20;
            this.loadPictureBox.TabStop = false;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.continueButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 261);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(477, 38);
            this.controlPanel1.TabIndex = 19;
            // 
            // directoryLabel
            // 
            this.directoryLabel.AutoSize = true;
            this.directoryLabel.Location = new System.Drawing.Point(11, 79);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(56, 13);
            this.directoryLabel.TabIndex = 18;
            this.directoryLabel.Text = "Directory:";
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(75, 76);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.ReadOnly = true;
            this.directoryTextBox.Size = new System.Drawing.Size(391, 22);
            this.directoryTextBox.TabIndex = 17;
            this.directoryTextBox.Text = "/";
            // 
            // ftpDirectoryInfoLabel
            // 
            this.ftpDirectoryInfoLabel.AutoSize = true;
            this.ftpDirectoryInfoLabel.Location = new System.Drawing.Point(12, 51);
            this.ftpDirectoryInfoLabel.Name = "ftpDirectoryInfoLabel";
            this.ftpDirectoryInfoLabel.Size = new System.Drawing.Size(317, 13);
            this.ftpDirectoryInfoLabel.TabIndex = 16;
            this.ftpDirectoryInfoLabel.Text = "Select the directory that should be used for the update files.";
            // 
            // serverDataTreeView
            // 
            this.serverDataTreeView.ImageIndex = 0;
            this.serverDataTreeView.ImageList = this.serverImageList;
            this.serverDataTreeView.Location = new System.Drawing.Point(12, 107);
            this.serverDataTreeView.Name = "serverDataTreeView";
            treeNode1.Name = "serverHeader";
            treeNode1.Text = "Server";
            this.serverDataTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.serverDataTreeView.SelectedImageIndex = 0;
            this.serverDataTreeView.Size = new System.Drawing.Size(454, 143);
            this.serverDataTreeView.TabIndex = 0;
            this.serverDataTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.serverDataTreeView_AfterSelect);
            // 
            // forwardButton
            // 
            this.forwardButton.ArrowDirection = ExplorerNavigationButton.ArrowDirection.Right;
            this.forwardButton.BackColor = System.Drawing.Color.Black;
            this.forwardButton.Enabled = false;
            this.forwardButton.Location = new System.Drawing.Point(32, 6);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(24, 24);
            this.forwardButton.TabIndex = 22;
            this.forwardButton.Text = "explorerNavigationButton2";
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Black;
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(2, 6);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(24, 24);
            this.backButton.TabIndex = 21;
            this.backButton.Text = "explorerNavigationButton1";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // DirectorySearchForm
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(478, 299);
            this.Controls.Add(this.forwardButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.loadPictureBox);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.directoryLabel);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.ftpDirectoryInfoLabel);
            this.Controls.Add(this.serverDataTreeView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DirectorySearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTitle";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DirectorySearchForm_FormClosing);
            this.Load += new System.EventHandler(this.DirectorySearchForm_Load);
            this.Shown += new System.EventHandler(this.DirectorySearchForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.loadPictureBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView serverDataTreeView;
        private System.Windows.Forms.Label ftpDirectoryInfoLabel;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Label directoryLabel;
        private nUpdate.UI.Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.ImageList serverImageList;
        private System.Windows.Forms.PictureBox loadPictureBox;
        private ExplorerNavigationButton.ExplorerNavigationButton backButton;
        private ExplorerNavigationButton.ExplorerNavigationButton forwardButton;
    }
}