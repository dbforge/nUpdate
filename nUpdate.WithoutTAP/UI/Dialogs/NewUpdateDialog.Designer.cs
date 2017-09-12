using nUpdate.Internal.UI.Controls;

namespace nUpdate.UI.Dialogs
{
    partial class NewUpdateDialog
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
            this.changelogTextBox = new System.Windows.Forms.RichTextBox();
            this.headerLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.newestVersionLabel = new System.Windows.Forms.Label();
            this.currentVersionLabel = new System.Windows.Forms.Label();
            this.updateSizeLabel = new System.Windows.Forms.Label();
            this.changelogLabel = new System.Windows.Forms.Label();
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.controlPanel1 = new BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.installButton = new System.Windows.Forms.Button();
            this.accessLabel = new System.Windows.Forms.Label();
            this.line1 = new Line();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // changelogTextBox
            // 
            this.changelogTextBox.BackColor = System.Drawing.Color.White;
            this.changelogTextBox.BulletIndent = 10;
            this.changelogTextBox.Location = new System.Drawing.Point(12, 185);
            this.changelogTextBox.Name = "changelogTextBox";
            this.changelogTextBox.ReadOnly = true;
            this.changelogTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.changelogTextBox.Size = new System.Drawing.Size(351, 102);
            this.changelogTextBox.TabIndex = 0;
            this.changelogTextBox.Text = "";
            this.changelogTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.changelogTextBox_LinkClicked);
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.Location = new System.Drawing.Point(61, 12);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(189, 21);
            this.headerLabel.TabIndex = 2;
            this.headerLabel.Text = "{0} new updates available.";
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(63, 36);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(300, 34);
            this.infoLabel.TabIndex = 4;
            this.infoLabel.Text = "New updates can be downloaded for {0}.";
            // 
            // newestVersionLabel
            // 
            this.newestVersionLabel.AutoSize = true;
            this.newestVersionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newestVersionLabel.Location = new System.Drawing.Point(9, 82);
            this.newestVersionLabel.Name = "newestVersionLabel";
            this.newestVersionLabel.Size = new System.Drawing.Size(121, 13);
            this.newestVersionLabel.TabIndex = 12;
            this.newestVersionLabel.Text = "Available versions: {0}";
            // 
            // currentVersionLabel
            // 
            this.currentVersionLabel.AutoSize = true;
            this.currentVersionLabel.Location = new System.Drawing.Point(9, 100);
            this.currentVersionLabel.Name = "currentVersionLabel";
            this.currentVersionLabel.Size = new System.Drawing.Size(104, 13);
            this.currentVersionLabel.TabIndex = 13;
            this.currentVersionLabel.Text = "Current version: {0}";
            // 
            // updateSizeLabel
            // 
            this.updateSizeLabel.AutoSize = true;
            this.updateSizeLabel.Location = new System.Drawing.Point(9, 117);
            this.updateSizeLabel.Name = "updateSizeLabel";
            this.updateSizeLabel.Size = new System.Drawing.Size(118, 13);
            this.updateSizeLabel.TabIndex = 14;
            this.updateSizeLabel.Text = "Total package size: {0}";
            // 
            // changelogLabel
            // 
            this.changelogLabel.AutoSize = true;
            this.changelogLabel.Location = new System.Drawing.Point(9, 166);
            this.changelogLabel.Name = "changelogLabel";
            this.changelogLabel.Size = new System.Drawing.Size(67, 13);
            this.changelogLabel.TabIndex = 15;
            this.changelogLabel.Text = "Changelog:";
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(16, 17);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(41, 39);
            this.iconPictureBox.TabIndex = 11;
            this.iconPictureBox.TabStop = false;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.installButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 296);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(371, 39);
            this.controlPanel1.TabIndex = 16;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(278, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(85, 23);
            this.cancelButton.TabIndex = 17;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // installButton
            // 
            this.installButton.Location = new System.Drawing.Point(163, 8);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(111, 23);
            this.installButton.TabIndex = 0;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // accessLabel
            // 
            this.accessLabel.AutoSize = true;
            this.accessLabel.Location = new System.Drawing.Point(9, 134);
            this.accessLabel.Name = "accessLabel";
            this.accessLabel.Size = new System.Drawing.Size(54, 13);
            this.accessLabel.TabIndex = 17;
            this.accessLabel.Text = "Accesses:";
            // 
            // line1
            // 
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(7, 151);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(356, 10);
            this.line1.TabIndex = 18;
            this.line1.Text = "line1";
            // 
            // NewUpdateDialog
            // 
            this.AcceptButton = this.installButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(371, 335);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.accessLabel);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.changelogLabel);
            this.Controls.Add(this.updateSizeLabel);
            this.Controls.Add(this.currentVersionLabel);
            this.Controls.Add(this.newestVersionLabel);
            this.Controls.Add(this.iconPictureBox);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.changelogTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewUpdateDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTitle";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewUpdateDialog_FormClosing);
            this.Load += new System.EventHandler(this.NewUpdateDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox changelogTextBox;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.Label newestVersionLabel;
        private System.Windows.Forms.Label currentVersionLabel;
        private System.Windows.Forms.Label updateSizeLabel;
        private System.Windows.Forms.Label changelogLabel;
        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.Label accessLabel;
        private Line line1;
    }
}