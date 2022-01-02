using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.Operations.Panels
{
    partial class RegistrySubKeyDeleteOperationPanel
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
            this.addButton = new System.Windows.Forms.Button();
            this.keyNameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.subKeysToDeleteListBox = new System.Windows.Forms.ListBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.mainKeyComboBox = new System.Windows.Forms.ComboBox();
            this.subKeyTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(379, 43);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 33;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // keyNameTextBox
            // 
            this.keyNameTextBox.Cue = "SubKey";
            this.keyNameTextBox.Location = new System.Drawing.Point(106, 43);
            this.keyNameTextBox.Name = "keyNameTextBox";
            this.keyNameTextBox.Size = new System.Drawing.Size(267, 22);
            this.keyNameTextBox.TabIndex = 32;
            this.keyNameTextBox.TextChanged += new System.EventHandler(this.InputChanged);
            this.keyNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyNameTextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 19);
            this.label1.TabIndex = 31;
            this.label1.Text = "Keyname:";
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(4, 13);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(84, 19);
            this.pathLabel.TabIndex = 29;
            this.pathLabel.Text = "Key:";
            // 
            // subKeysToDeleteListBox
            // 
            this.subKeysToDeleteListBox.FormattingEnabled = true;
            this.subKeysToDeleteListBox.Location = new System.Drawing.Point(106, 71);
            this.subKeysToDeleteListBox.Name = "subKeysToDeleteListBox";
            this.subKeysToDeleteListBox.Size = new System.Drawing.Size(267, 121);
            this.subKeysToDeleteListBox.TabIndex = 28;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(379, 71);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 38;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(245, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 13);
            this.label3.TabIndex = 41;
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
            this.mainKeyComboBox.Size = new System.Drawing.Size(133, 21);
            this.mainKeyComboBox.TabIndex = 40;
            // 
            // subKeyTextBox
            // 
            this.subKeyTextBox.Cue = "MyApp\\SubKey";
            this.subKeyTextBox.Location = new System.Drawing.Point(261, 13);
            this.subKeyTextBox.Name = "subKeyTextBox";
            this.subKeyTextBox.Size = new System.Drawing.Size(150, 22);
            this.subKeyTextBox.TabIndex = 39;
            this.subKeyTextBox.TextChanged += new System.EventHandler(this.InputChanged);
            // 
            // RegistrySubKeyDeleteOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mainKeyComboBox);
            this.Controls.Add(this.subKeyTextBox);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.keyNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.subKeysToDeleteListBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RegistrySubKeyDeleteOperationPanel";
            this.Size = new System.Drawing.Size(488, 204);
            this.Load += new System.EventHandler(this.RegistryEntryDeleteOperationPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private CueTextBox keyNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ListBox subKeysToDeleteListBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mainKeyComboBox;
        private CueTextBox subKeyTextBox;
    }
}
