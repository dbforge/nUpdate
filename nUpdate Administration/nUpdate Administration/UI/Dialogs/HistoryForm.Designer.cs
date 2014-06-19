namespace nUpdate.Administration.UI.Dialogs
{
    partial class HistoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryForm));
            this.noHistoryLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showDetailsButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearLogButton = new System.Windows.Forms.ToolStripButton();
            this.historyList = new nUpdate.Administration.UI.Controls.ActionList();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // noHistoryLabel
            // 
            this.noHistoryLabel.AutoSize = true;
            this.noHistoryLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noHistoryLabel.Location = new System.Drawing.Point(165, 134);
            this.noHistoryLabel.Name = "noHistoryLabel";
            this.noHistoryLabel.Size = new System.Drawing.Size(111, 13);
            this.noHistoryLabel.TabIndex = 2;
            this.noHistoryLabel.Text = "No history available.";
            this.noHistoryLabel.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.toolStripSeparator1,
            this.showDetailsButton,
            this.toolStripSeparator2,
            this.clearLogButton});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(446, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openButton
            // 
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(83, 22);
            this.openButton.Text = "Open path";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // showDetailsButton
            // 
            this.showDetailsButton.Image = ((System.Drawing.Image)(resources.GetObject("showDetailsButton.Image")));
            this.showDetailsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showDetailsButton.Name = "showDetailsButton";
            this.showDetailsButton.Size = new System.Drawing.Size(93, 22);
            this.showDetailsButton.Text = "Show details";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // clearLogButton
            // 
            this.clearLogButton.Image = ((System.Drawing.Image)(resources.GetObject("clearLogButton.Image")));
            this.clearLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(93, 22);
            this.clearLogButton.Text = "Clear history";
            this.clearLogButton.Click += new System.EventHandler(this.clearLog_Click);
            // 
            // historyList
            // 
            this.historyList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.historyList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.historyList.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historyList.FormattingEnabled = true;
            this.historyList.IntegralHeight = false;
            this.historyList.ItemHeight = 72;
            this.historyList.Location = new System.Drawing.Point(0, 27);
            this.historyList.Name = "historyList";
            this.historyList.Size = new System.Drawing.Size(446, 259);
            this.historyList.TabIndex = 0;
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(446, 287);
            this.Controls.Add(this.noHistoryLabel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.historyList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "History - {0} - nUpdate Administration 1.1.0.0";
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Administration.UI.Controls.ActionList historyList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.Label noHistoryLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton showDetailsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton clearLogButton;

    }
}