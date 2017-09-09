namespace nUpdate.UserInterfaceTest
{
    partial class MainDialog
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
            this.updateButton = new System.Windows.Forms.Button();
            this.hiddenSearchCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(77, 82);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(141, 72);
            this.updateButton.TabIndex = 0;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // hiddenSearchCheckBox
            // 
            this.hiddenSearchCheckBox.AutoSize = true;
            this.hiddenSearchCheckBox.Location = new System.Drawing.Point(77, 49);
            this.hiddenSearchCheckBox.Name = "hiddenSearchCheckBox";
            this.hiddenSearchCheckBox.Size = new System.Drawing.Size(147, 27);
            this.hiddenSearchCheckBox.TabIndex = 1;
            this.hiddenSearchCheckBox.Text = "Hidden Search";
            this.hiddenSearchCheckBox.UseVisualStyleBackColor = true;
            this.hiddenSearchCheckBox.CheckedChanged += new System.EventHandler(this.hiddenSearchCheckBox_CheckedChanged);
            // 
            // MainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.hiddenSearchCheckBox);
            this.Controls.Add(this.updateButton);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "MainDialog";
            this.Text = "MainDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.CheckBox hiddenSearchCheckBox;
    }
}

