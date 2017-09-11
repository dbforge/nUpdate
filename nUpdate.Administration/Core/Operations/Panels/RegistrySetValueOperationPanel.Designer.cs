using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.Core.Operations.Panels
{
    partial class RegistrySetValueOperationPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistrySetValueOperationPanel));
            this.label3 = new System.Windows.Forms.Label();
            this.mainKeyComboBox = new System.Windows.Forms.ComboBox();
            this.valueTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.dataTypeLabel = new System.Windows.Forms.Label();
            this.subKeyTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.valueKindComboBox = new System.Windows.Forms.ComboBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.nameValuePairListView = new nUpdate.Administration.UI.Controls.ExplorerListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.nameTextBox = new nUpdate.Administration.UI.Controls.CueTextBox();
            this.line1 = new Line();
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
            this.valueTextBox.Cue = "MyValue";
            this.valueTextBox.Location = new System.Drawing.Point(279, 60);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(162, 22);
            this.valueTextBox.TabIndex = 33;
            // 
            // dataTypeLabel
            // 
            this.dataTypeLabel.Location = new System.Drawing.Point(4, 93);
            this.dataTypeLabel.Name = "dataTypeLabel";
            this.dataTypeLabel.Size = new System.Drawing.Size(97, 19);
            this.dataTypeLabel.TabIndex = 32;
            this.dataTypeLabel.Text = "Value kind:";
            // 
            // subKeyTextBox
            // 
            this.subKeyTextBox.Cue = "SubKey1\\SubKey2";
            this.subKeyTextBox.Location = new System.Drawing.Point(249, 13);
            this.subKeyTextBox.Name = "subKeyTextBox";
            this.subKeyTextBox.Size = new System.Drawing.Size(192, 22);
            this.subKeyTextBox.TabIndex = 31;
            this.subKeyTextBox.TextChanged += new System.EventHandler(this.subKeyTextBox_TextChanged);
            // 
            // pathLabel
            // 
            this.pathLabel.Location = new System.Drawing.Point(4, 16);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(48, 19);
            this.pathLabel.TabIndex = 30;
            this.pathLabel.Text = "Path:";
            // 
            // valueKindComboBox
            // 
            this.valueKindComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.valueKindComboBox.FormattingEnabled = true;
            this.valueKindComboBox.Location = new System.Drawing.Point(106, 90);
            this.valueKindComboBox.Name = "valueKindComboBox";
            this.valueKindComboBox.Size = new System.Drawing.Size(121, 21);
            this.valueKindComboBox.TabIndex = 39;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(233, 63);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(39, 13);
            this.valueLabel.TabIndex = 40;
            this.valueLabel.Text = "Value:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(398, 30);
            this.label2.TabIndex = 42;
            this.label2.Text = "If you choose \"Binary\" or \"MultipleString\" as a value kind, then separate the arr" +
    "ay \r\nelements by a \",\" without any spaces. Example: \"10,43,44\"";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 255);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 41;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 286);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(398, 30);
            this.label1.TabIndex = 43;
            this.label1.Text = "If the name-value-pair does not already exist, it will be created.";
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(366, 156);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 61;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(366, 128);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 60;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // nameValuePairListView
            // 
            this.nameValuePairListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.nameValuePairListView.FullRowSelect = true;
            this.nameValuePairListView.GridLines = true;
            this.nameValuePairListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.nameValuePairListView.Location = new System.Drawing.Point(106, 128);
            this.nameValuePairListView.Name = "nameValuePairListView";
            this.nameValuePairListView.Size = new System.Drawing.Size(254, 110);
            this.nameValuePairListView.TabIndex = 62;
            this.nameValuePairListView.UseCompatibleStateImageBehavior = false;
            this.nameValuePairListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 83;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 78;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Value kind";
            this.columnHeader3.Width = 89;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 64;
            this.label4.Text = "Name:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Cue = "Test";
            this.nameTextBox.Location = new System.Drawing.Point(106, 60);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(121, 22);
            this.nameTextBox.TabIndex = 63;
            // 
            // line1
            // 
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(2, 41);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(455, 13);
            this.line1.TabIndex = 65;
            this.line1.Text = "line1";
            // 
            // RegistrySetValueOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.line1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameValuePairListView);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.valueKindComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mainKeyComboBox);
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.dataTypeLabel);
            this.Controls.Add(this.subKeyTextBox);
            this.Controls.Add(this.pathLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RegistrySetValueOperationPanel";
            this.Size = new System.Drawing.Size(474, 230);
            this.Load += new System.EventHandler(this.RegistryEntrySetValueOperationPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mainKeyComboBox;
        private UI.Controls.CueTextBox valueTextBox;
        private System.Windows.Forms.Label dataTypeLabel;
        private UI.Controls.CueTextBox subKeyTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ComboBox valueKindComboBox;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private UI.Controls.ExplorerListView nameValuePairListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label4;
        private UI.Controls.CueTextBox nameTextBox;
        private Line line1;
    }
}
