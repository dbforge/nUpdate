namespace nUpdate.Administration.UI.Dialogs
{
    partial class PackageEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageEditForm));
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.canceButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.ftpHeaderLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buildNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.majorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.revisionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.minorNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.stageComboBox = new System.Windows.Forms.ComboBox();
            this.devStageLabel = new System.Windows.Forms.Label();
            this.environmentLabel = new System.Windows.Forms.Label();
            this.environmentComboBox = new System.Windows.Forms.ComboBox();
            this.controlPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buildNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.revisionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minorNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.canceButton);
            this.controlPanel1.Controls.Add(this.saveButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 195);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(397, 40);
            this.controlPanel1.TabIndex = 0;
            // 
            // canceButton
            // 
            this.canceButton.Location = new System.Drawing.Point(233, 8);
            this.canceButton.Name = "canceButton";
            this.canceButton.Size = new System.Drawing.Size(75, 23);
            this.canceButton.TabIndex = 1;
            this.canceButton.Text = "Cancel";
            this.canceButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(310, 8);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // ftpHeaderLabel
            // 
            this.ftpHeaderLabel.AutoSize = true;
            this.ftpHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.ftpHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.ftpHeaderLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpHeaderLabel.Location = new System.Drawing.Point(12, 9);
            this.ftpHeaderLabel.Name = "ftpHeaderLabel";
            this.ftpHeaderLabel.Size = new System.Drawing.Size(95, 20);
            this.ftpHeaderLabel.TabIndex = 40;
            this.ftpHeaderLabel.Text = "Edit package";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Version:";
            // 
            // buildNumericUpDown
            // 
            this.buildNumericUpDown.Location = new System.Drawing.Point(250, 44);
            this.buildNumericUpDown.Name = "buildNumericUpDown";
            this.buildNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.buildNumericUpDown.TabIndex = 44;
            // 
            // majorNumericUpDown
            // 
            this.majorNumericUpDown.Location = new System.Drawing.Point(126, 44);
            this.majorNumericUpDown.Name = "majorNumericUpDown";
            this.majorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.majorNumericUpDown.TabIndex = 42;
            // 
            // revisionNumericUpDown
            // 
            this.revisionNumericUpDown.Location = new System.Drawing.Point(310, 44);
            this.revisionNumericUpDown.Name = "revisionNumericUpDown";
            this.revisionNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.revisionNumericUpDown.TabIndex = 45;
            // 
            // minorNumericUpDown
            // 
            this.minorNumericUpDown.Location = new System.Drawing.Point(188, 44);
            this.minorNumericUpDown.Name = "minorNumericUpDown";
            this.minorNumericUpDown.Size = new System.Drawing.Size(56, 22);
            this.minorNumericUpDown.TabIndex = 43;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Description:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(126, 75);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(240, 22);
            this.textBox1.TabIndex = 47;
            // 
            // stageComboBox
            // 
            this.stageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stageComboBox.FormattingEnabled = true;
            this.stageComboBox.Items.AddRange(new object[] {
            "Alpha",
            "Beta",
            "Release"});
            this.stageComboBox.Location = new System.Drawing.Point(188, 108);
            this.stageComboBox.Name = "stageComboBox";
            this.stageComboBox.Size = new System.Drawing.Size(178, 21);
            this.stageComboBox.TabIndex = 49;
            // 
            // devStageLabel
            // 
            this.devStageLabel.AutoSize = true;
            this.devStageLabel.Location = new System.Drawing.Point(12, 111);
            this.devStageLabel.Name = "devStageLabel";
            this.devStageLabel.Size = new System.Drawing.Size(118, 13);
            this.devStageLabel.TabIndex = 50;
            this.devStageLabel.Text = "Developmental stage:";
            // 
            // environmentLabel
            // 
            this.environmentLabel.AutoSize = true;
            this.environmentLabel.Location = new System.Drawing.Point(13, 144);
            this.environmentLabel.Name = "environmentLabel";
            this.environmentLabel.Size = new System.Drawing.Size(119, 13);
            this.environmentLabel.TabIndex = 52;
            this.environmentLabel.Text = "Environment settings:";
            // 
            // environmentComboBox
            // 
            this.environmentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.environmentComboBox.FormattingEnabled = true;
            this.environmentComboBox.Items.AddRange(new object[] {
            "x86 (32-bit)",
            "x64 (64-bit)",
            "AnyCPU (independent)"});
            this.environmentComboBox.Location = new System.Drawing.Point(188, 140);
            this.environmentComboBox.Name = "environmentComboBox";
            this.environmentComboBox.Size = new System.Drawing.Size(178, 21);
            this.environmentComboBox.TabIndex = 51;
            // 
            // PackageEditForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(397, 235);
            this.Controls.Add(this.environmentLabel);
            this.Controls.Add(this.environmentComboBox);
            this.Controls.Add(this.stageComboBox);
            this.Controls.Add(this.devStageLabel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buildNumericUpDown);
            this.Controls.Add(this.majorNumericUpDown);
            this.Controls.Add(this.revisionNumericUpDown);
            this.Controls.Add(this.minorNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ftpHeaderLabel);
            this.Controls.Add(this.controlPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackageEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit update package - {0} - {1}";
            this.controlPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.buildNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.revisionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minorNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Label ftpHeaderLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown buildNumericUpDown;
        private System.Windows.Forms.NumericUpDown majorNumericUpDown;
        private System.Windows.Forms.NumericUpDown revisionNumericUpDown;
        private System.Windows.Forms.NumericUpDown minorNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox stageComboBox;
        private System.Windows.Forms.Label devStageLabel;
        private System.Windows.Forms.Label environmentLabel;
        private System.Windows.Forms.ComboBox environmentComboBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button canceButton;
    }
}