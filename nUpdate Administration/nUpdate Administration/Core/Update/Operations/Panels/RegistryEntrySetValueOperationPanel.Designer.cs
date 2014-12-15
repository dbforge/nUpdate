namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    partial class RegistryEntrySetValueOperationPanel
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
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistryEntrySetValueOperationPanel));
            this.label3 = new System.Windows.Forms.Label();
            this.mainKeyComboBox = new System.Windows.Forms.ComboBox();
            this.valueTextBox = new nUpdate.Administration.UI.Controls.WatermarkTextBox();
            this.dataTypeLabel = new System.Windows.Forms.Label();
            this.appKeyTextBox = new nUpdate.Administration.UI.Controls.WatermarkTextBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.dataTypeComboBox = new System.Windows.Forms.ComboBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "\\";
            // 
            // mainKeyComboBox
            // 
            this.mainKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mainKeyComboBox.FormattingEnabled = true;
            this.mainKeyComboBox.Items.AddRange(new object[] {
            "HKEY_CLASSES_ROOT",
            "HKEY_CURRENT_USER",
            "HKEY_LOCAL_MACHINE"});
            this.mainKeyComboBox.Location = new System.Drawing.Point(106, 13);
            this.mainKeyComboBox.Name = "mainKeyComboBox";
            this.mainKeyComboBox.Size = new System.Drawing.Size(121, 21);
            this.mainKeyComboBox.TabIndex = 35;
            // 
            // valueTextBox
            // 
            this.valueTextBox.Cue = "Test";
            this.valueTextBox.Location = new System.Drawing.Point(106, 73);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(267, 22);
            this.valueTextBox.TabIndex = 33;
            // 
            // dataTypeLabel
            // 
            this.dataTypeLabel.Location = new System.Drawing.Point(4, 46);
            this.dataTypeLabel.Name = "dataTypeLabel";
            this.dataTypeLabel.Size = new System.Drawing.Size(97, 19);
            this.dataTypeLabel.TabIndex = 32;
            this.dataTypeLabel.Text = "Data type:";
            // 
            // appKeyTextBox
            // 
            this.appKeyTextBox.Cue = "MyApp";
            this.appKeyTextBox.Location = new System.Drawing.Point(249, 13);
            this.appKeyTextBox.Name = "appKeyTextBox";
            this.appKeyTextBox.Size = new System.Drawing.Size(150, 22);
            this.appKeyTextBox.TabIndex = 31;
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(4, 16);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(48, 19);
            this.pathLabel.TabIndex = 30;
            this.pathLabel.Text = "Path:";
            // 
            // dataTypeComboBox
            // 
            this.dataTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataTypeComboBox.FormattingEnabled = true;
            this.dataTypeComboBox.Location = new System.Drawing.Point(106, 43);
            this.dataTypeComboBox.Name = "dataTypeComboBox";
            this.dataTypeComboBox.Size = new System.Drawing.Size(267, 21);
            this.dataTypeComboBox.TabIndex = 39;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(4, 76);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(39, 13);
            this.valueLabel.TabIndex = 40;
            this.valueLabel.Text = "Value:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(398, 30);
            this.label2.TabIndex = 42;
            this.label2.Text = "If you choose \"Binary\" or \"MultipleString\" as data type for the value, then separ" +
    "ate\r\nthe array elements by a \",\" without any spaces. Example: \"10,43,44\"";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 41;
            this.pictureBox1.TabStop = false;
            // 
            // RegistryEntrySetValueOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.dataTypeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mainKeyComboBox);
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.dataTypeLabel);
            this.Controls.Add(this.appKeyTextBox);
            this.Controls.Add(this.pathLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RegistryEntrySetValueOperationPanel";
            this.Size = new System.Drawing.Size(488, 204);
            this.Load += new System.EventHandler(this.RegistryEntrySetValueOperationPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mainKeyComboBox;
        private UI.Controls.WatermarkTextBox valueTextBox;
        private System.Windows.Forms.Label dataTypeLabel;
        private UI.Controls.WatermarkTextBox appKeyTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ComboBox dataTypeComboBox;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
