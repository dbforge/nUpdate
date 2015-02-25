namespace nUpdate.Administration.UI.Dialogs
{
    partial class HistoryDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryDialog));
            this.noHistoryLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.clearLogButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToFileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.orderComboBox = new System.Windows.Forms.ToolStripComboBox();
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
            this.clearLogButton,
            this.toolStripSeparator1,
            this.saveToFileButton,
            this.toolStripSeparator2,
            this.orderComboBox});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(446, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // saveToFileButton
            // 
            this.saveToFileButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToFileButton.Image")));
            this.saveToFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToFileButton.Name = "saveToFileButton";
            this.saveToFileButton.Size = new System.Drawing.Size(84, 22);
            this.saveToFileButton.Text = "Save to file";
            this.saveToFileButton.Click += new System.EventHandler(this.saveToFileButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // orderComboBox
            // 
            this.orderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.orderComboBox.Items.AddRange(new object[] {
            "Order descending",
            "Order ascending"});
            this.orderComboBox.Name = "orderComboBox";
            this.orderComboBox.Size = new System.Drawing.Size(121, 25);
            this.orderComboBox.SelectedIndexChanged += new System.EventHandler(this.orderComboBox_SelectedIndexChanged);
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
            // HistoryDialog
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
            this.Name = "HistoryDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "History - {0} - {1}";
            this.Load += new System.EventHandler(this.HistoryDialog_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Administration.UI.Controls.ActionList historyList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Label noHistoryLabel;
        private System.Windows.Forms.ToolStripButton clearLogButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton saveToFileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox orderComboBox;

    }
}