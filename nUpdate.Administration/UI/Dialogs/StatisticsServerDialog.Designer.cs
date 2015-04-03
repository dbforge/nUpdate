namespace nUpdate.Administration.UI.Dialogs
{
    partial class StatisticsServerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsServerDialog));
            this.serverList = new nUpdate.Administration.UI.Controls.ServerList();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addServerButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editServerButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteServerButton = new System.Windows.Forms.ToolStripButton();
            this.noServersLabel = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverList
            // 
            this.serverList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.serverList.FormattingEnabled = true;
            this.serverList.IntegralHeight = false;
            this.serverList.ItemHeight = 60;
            this.serverList.Location = new System.Drawing.Point(0, 26);
            this.serverList.Name = "serverList";
            this.serverList.Size = new System.Drawing.Size(479, 244);
            this.serverList.TabIndex = 8;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Visualpharm-Hardware-Server.ico");
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addServerButton,
            this.toolStripSeparator1,
            this.editServerButton,
            this.toolStripSeparator2,
            this.deleteServerButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(479, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addServerButton
            // 
            this.addServerButton.Image = ((System.Drawing.Image)(resources.GetObject("addServerButton.Image")));
            this.addServerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addServerButton.Name = "addServerButton";
            this.addServerButton.Size = new System.Drawing.Size(92, 22);
            this.addServerButton.Text = "Add a server";
            this.addServerButton.Click += new System.EventHandler(this.addServerButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // editServerButton
            // 
            this.editServerButton.Image = ((System.Drawing.Image)(resources.GetObject("editServerButton.Image")));
            this.editServerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editServerButton.Name = "editServerButton";
            this.editServerButton.Size = new System.Drawing.Size(81, 22);
            this.editServerButton.Text = "Edit server";
            this.editServerButton.Click += new System.EventHandler(this.editServerButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // deleteServerButton
            // 
            this.deleteServerButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteServerButton.Image")));
            this.deleteServerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteServerButton.Name = "deleteServerButton";
            this.deleteServerButton.Size = new System.Drawing.Size(94, 22);
            this.deleteServerButton.Text = "Delete server";
            this.deleteServerButton.Click += new System.EventHandler(this.deleteServerButton_Click);
            // 
            // noServersLabel
            // 
            this.noServersLabel.AutoSize = true;
            this.noServersLabel.Location = new System.Drawing.Point(159, 128);
            this.noServersLabel.Name = "noServersLabel";
            this.noServersLabel.Size = new System.Drawing.Size(153, 13);
            this.noServersLabel.TabIndex = 10;
            this.noServersLabel.Text = "No statistic servers available.";
            this.noServersLabel.Visible = false;
            // 
            // StatisticsServerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 270);
            this.Controls.Add(this.noServersLabel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.serverList);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "StatisticsServerDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Statistic servers - {0}";
            this.Load += new System.EventHandler(this.StatisticsServerDialog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.StatisticsServerDialog_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ServerList serverList;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addServerButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton editServerButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton deleteServerButton;
        private System.Windows.Forms.Label noServersLabel;
    }
}