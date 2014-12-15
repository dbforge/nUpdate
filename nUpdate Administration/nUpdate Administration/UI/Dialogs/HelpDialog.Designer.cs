namespace nUpdate.Administration.UI.Dialogs
{
    partial class HelpDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpDialog));
            this.informationLabel = new System.Windows.Forms.Label();
            this.helpEntryActionList = new nUpdate.Administration.UI.Controls.ActionList();
            this.SuspendLayout();
            // 
            // informationLabel
            // 
            this.informationLabel.AutoSize = true;
            this.informationLabel.Location = new System.Drawing.Point(226, 147);
            this.informationLabel.Name = "informationLabel";
            this.informationLabel.Size = new System.Drawing.Size(96, 13);
            this.informationLabel.TabIndex = 1;
            this.informationLabel.Text = "Loading entries...";
            // 
            // helpEntryActionList
            // 
            this.helpEntryActionList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.helpEntryActionList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.helpEntryActionList.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpEntryActionList.FormattingEnabled = true;
            this.helpEntryActionList.IntegralHeight = false;
            this.helpEntryActionList.ItemHeight = 72;
            this.helpEntryActionList.Location = new System.Drawing.Point(-2, 0);
            this.helpEntryActionList.Name = "helpEntryActionList";
            this.helpEntryActionList.Size = new System.Drawing.Size(547, 345);
            this.helpEntryActionList.TabIndex = 2;
            // 
            // HelpDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 345);
            this.Controls.Add(this.informationLabel);
            this.Controls.Add(this.helpEntryActionList);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "HelpDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Help - nUpdate Administration 0.1.0.0 Alpha 1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HelpDialog_FormClosing);
            this.Load += new System.EventHandler(this.HelpDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label informationLabel;
        private Controls.ActionList helpEntryActionList;
    }
}