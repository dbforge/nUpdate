using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class MainDialog
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                _ftpPassword.Dispose();
                _proxyPassword.Dispose();
                _sqlPassword.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDialog));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.headerLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.sectionsListView = new nUpdate.Administration.UI.Controls.ExplorerListView();
            this.itemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.itemDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.line1 = new Line();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "application-blue.png");
            this.imageList1.Images.SetKeyName(1, "application-import.png");
            this.imageList1.Images.SetKeyName(2, "pinion-settings-icon.png");
            this.imageList1.Images.SetKeyName(3, "information.png");
            this.imageList1.Images.SetKeyName(4, "mail.png");
            this.imageList1.Images.SetKeyName(5, "server.png");
            this.imageList1.Images.SetKeyName(6, "application-sidebar.png");
            this.imageList1.Images.SetKeyName(7, "application-sidebar-list.png");
            this.imageList1.Images.SetKeyName(8, "user-business.png");
            this.imageList1.Images.SetKeyName(9, "bin.png");
            this.imageList1.Images.SetKeyName(10, "arrow-join.png");
            // 
            // headerLabel
            // 
            resources.ApplyResources(this.headerLabel, "headerLabel");
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.Name = "headerLabel";
            // 
            // infoLabel
            // 
            resources.ApplyResources(this.infoLabel, "infoLabel");
            this.infoLabel.Name = "infoLabel";
            // 
            // sectionsListView
            // 
            this.sectionsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sectionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.itemName,
            this.itemDescription});
            this.sectionsListView.FullRowSelect = true;
            this.sectionsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("sectionsListView.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("sectionsListView.Groups1"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("sectionsListView.Groups2")))});
            this.sectionsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.sectionsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items2"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items3"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items4"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items5"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items6"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items7"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("sectionsListView.Items8")))});
            this.sectionsListView.LargeImageList = this.imageList1;
            resources.ApplyResources(this.sectionsListView, "sectionsListView");
            this.sectionsListView.MultiSelect = false;
            this.sectionsListView.Name = "sectionsListView";
            this.sectionsListView.SmallImageList = this.imageList1;
            this.sectionsListView.TileSize = new System.Drawing.Size(220, 35);
            this.sectionsListView.UseCompatibleStateImageBehavior = false;
            this.sectionsListView.View = System.Windows.Forms.View.Tile;
            this.sectionsListView.Click += new System.EventHandler(this.sectionsListView_Click);
            // 
            // line1
            // 
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            resources.ApplyResources(this.line1, "line1");
            this.line1.Name = "line1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // MainDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.sectionsListView);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainDialog";
            this.Load += new System.EventHandler(this.MainDialog_Load);
            this.Shown += new System.EventHandler(this.MainDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label infoLabel;
        private Line line1;
        private UI.Controls.ExplorerListView sectionsListView;
        private System.Windows.Forms.ColumnHeader itemName;
        private System.Windows.Forms.ColumnHeader itemDescription;

    }
}

