namespace nUpdate.Administration.Operations.Panels
{
    partial class RegistryDeleteValueOperationPanel
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
            this.valueLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mainKeyComboBox = new System.Windows.Forms.ComboBox();
            this.subKeyTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.valueNameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.nameValuePairsToDeleteListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(4, 47);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(39, 13);
            this.valueLabel.TabIndex = 51;
            this.valueLabel.Text = "Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 13);
            this.label3.TabIndex = 49;
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
            this.mainKeyComboBox.TabIndex = 48;
            // 
            // subKeyTextBox
            // 
            this.subKeyTextBox.Cue = "SubKey1\\Subkey2";
            this.subKeyTextBox.Location = new System.Drawing.Point(249, 13);
            this.subKeyTextBox.Name = "subKeyTextBox";
            this.subKeyTextBox.Size = new System.Drawing.Size(150, 22);
            this.subKeyTextBox.TabIndex = 45;
            this.subKeyTextBox.TextChanged += new System.EventHandler(this.InputChanged);
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(4, 16);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(48, 19);
            this.pathLabel.TabIndex = 44;
            this.pathLabel.Text = "Path:";
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(379, 70);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 58;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(379, 42);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 57;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // valueNameTextBox
            // 
            this.valueNameTextBox.Cue = "Test";
            this.valueNameTextBox.Location = new System.Drawing.Point(106, 42);
            this.valueNameTextBox.Name = "valueNameTextBox";
            this.valueNameTextBox.Size = new System.Drawing.Size(267, 22);
            this.valueNameTextBox.TabIndex = 56;
            this.valueNameTextBox.TextChanged += new System.EventHandler(this.InputChanged);
            this.valueNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.valueNameTextBox_KeyDown);
            // 
            // nameValuePairsToDeleteListBox
            // 
            this.nameValuePairsToDeleteListBox.FormattingEnabled = true;
            this.nameValuePairsToDeleteListBox.Location = new System.Drawing.Point(106, 70);
            this.nameValuePairsToDeleteListBox.Name = "nameValuePairsToDeleteListBox";
            this.nameValuePairsToDeleteListBox.Size = new System.Drawing.Size(267, 121);
            this.nameValuePairsToDeleteListBox.TabIndex = 55;
            // 
            // RegistryDeleteValueOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.valueNameTextBox);
            this.Controls.Add(this.nameValuePairsToDeleteListBox);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mainKeyComboBox);
            this.Controls.Add(this.subKeyTextBox);
            this.Controls.Add(this.pathLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RegistryDeleteValueOperationPanel";
            this.Size = new System.Drawing.Size(488, 203);
            this.Load += new System.EventHandler(this.RegistryEntryDeleteValueOperationPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mainKeyComboBox;
        private UI.Controls.CueTextBox subKeyTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private UI.Controls.CueTextBox valueNameTextBox;
        private System.Windows.Forms.ListBox nameValuePairsToDeleteListBox;
    }
}
