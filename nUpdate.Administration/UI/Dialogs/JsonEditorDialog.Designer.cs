using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class JsonEditorDialog
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
                _argumentsStyle.Dispose();
                _keyWordStyle.Dispose();
                _commentStyle.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JsonEditorDialog));
            this.codeTextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.controlPanel1 = new BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveLanguageButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // codeTextBox
            // 
            this.codeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeTextBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.codeTextBox.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.codeTextBox.BackBrush = null;
            this.codeTextBox.CharHeight = 14;
            this.codeTextBox.CharWidth = 8;
            this.codeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.codeTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.codeTextBox.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.codeTextBox.IsReplaceMode = false;
            this.codeTextBox.LeftBracket = '{';
            this.codeTextBox.LeftBracket2 = '[';
            this.codeTextBox.Location = new System.Drawing.Point(0, 1);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Paddings = new System.Windows.Forms.Padding(0);
            this.codeTextBox.RightBracket = '}';
            this.codeTextBox.RightBracket2 = ']';
            this.codeTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.codeTextBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("codeTextBox.ServiceColors")));
            this.codeTextBox.Size = new System.Drawing.Size(518, 255);
            this.codeTextBox.TabIndex = 1;
            this.codeTextBox.Zoom = 100;
            this.codeTextBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.codeTextBox_TextChanged);
            // 
            // controlPanel1
            // 
            this.controlPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.saveLanguageButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 256);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(518, 40);
            this.controlPanel1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(429, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // saveLanguageButton
            // 
            this.saveLanguageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveLanguageButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.saveLanguageButton.Location = new System.Drawing.Point(318, 8);
            this.saveLanguageButton.Name = "saveLanguageButton";
            this.saveLanguageButton.Size = new System.Drawing.Size(105, 23);
            this.saveLanguageButton.TabIndex = 0;
            this.saveLanguageButton.Text = "Save language";
            this.saveLanguageButton.UseVisualStyleBackColor = true;
            this.saveLanguageButton.Click += new System.EventHandler(this.saveLanguageButton_Click);
            // 
            // JsonEditorDialog
            // 
            this.AcceptButton = this.saveLanguageButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(518, 296);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.codeTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(534, 334);
            this.Name = "JsonEditorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit language file - {0}";
            this.Load += new System.EventHandler(this.JSONEditorDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox codeTextBox;
        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button saveLanguageButton;
        private System.Windows.Forms.Button cancelButton;

    }
}