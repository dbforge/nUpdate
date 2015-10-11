namespace nUpdate.UI.Dialogs
{
    partial class MissingUpdateRequirementsDialog
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
            this.requirementsTextBox = new System.Windows.Forms.RichTextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.controlPanel1 = new nUpdate.UI.Controls.BottomPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // requirementsTextBox
            // 
            this.requirementsTextBox.BackColor = System.Drawing.Color.White;
            this.requirementsTextBox.BulletIndent = 10;
            this.requirementsTextBox.Location = new System.Drawing.Point(19, 56);
            this.requirementsTextBox.Name = "requirementsTextBox";
            this.requirementsTextBox.ReadOnly = true;
            this.requirementsTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.requirementsTextBox.Size = new System.Drawing.Size(351, 102);
            this.requirementsTextBox.TabIndex = 17;
            this.requirementsTextBox.Text = "";
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(16, 30);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(367, 23);
            this.infoLabel.TabIndex = 15;
            this.infoLabel.Text = "The following requirements are not given:";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // headerLabel
            // 
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.Location = new System.Drawing.Point(12, 9);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(371, 21);
            this.headerLabel.TabIndex = 14;
            this.headerLabel.Text = "There are no new updates available.";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.closeButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 182);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(395, 37);
            this.controlPanel1.TabIndex = 16;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(313, 7);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            // 
            // MissingUpdateRequirementsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(395, 219);
            this.Controls.Add(this.requirementsTextBox);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.controlPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MissingUpdateRequirementsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTitle";
            this.Load += new System.EventHandler(this.MissingUpdateRequirementsDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox requirementsTextBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label headerLabel;
        private Controls.BottomPanel controlPanel1;
        private System.Windows.Forms.Button closeButton;
    }
}