namespace nUpdate.Administration.Operations.Panels
{
    partial class ScriptExecuteOperationPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptExecuteOperationPanel));
            this.codeTextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.sourceCodeTabPage = new System.Windows.Forms.TabPage();
            this.errorsTabPage = new System.Windows.Forms.TabPage();
            this.errorListView = new System.Windows.Forms.ListView();
            this.numberHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lineHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.sourceCodeTabPage.SuspendLayout();
            this.errorsTabPage.SuspendLayout();
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
            this.codeTextBox.AutoScrollMinSize = new System.Drawing.Size(907, 252);
            this.codeTextBox.BackBrush = null;
            this.codeTextBox.CharHeight = 14;
            this.codeTextBox.CharWidth = 8;
            this.codeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.codeTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.codeTextBox.IsReplaceMode = false;
            this.codeTextBox.LeftBracket = '{';
            this.codeTextBox.LeftBracket2 = '[';
            this.codeTextBox.Location = new System.Drawing.Point(0, 0);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Paddings = new System.Windows.Forms.Padding(0);
            this.codeTextBox.RightBracket = '}';
            this.codeTextBox.RightBracket2 = ']';
            this.codeTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.codeTextBox.Size = new System.Drawing.Size(468, 197);
            this.codeTextBox.TabIndex = 2;
            this.codeTextBox.Text = resources.GetString("codeTextBox.Text");
            this.codeTextBox.Zoom = 100;
            this.codeTextBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.codeTextBox_TextChanged);
            this.codeTextBox.Leave += new System.EventHandler(this.codeTextBox_Leave);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.sourceCodeTabPage);
            this.tabControl1.Controls.Add(this.errorsTabPage);
            this.tabControl1.Location = new System.Drawing.Point(2, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(476, 223);
            this.tabControl1.TabIndex = 3;
            // 
            // sourceCodeTabPage
            // 
            this.sourceCodeTabPage.Controls.Add(this.codeTextBox);
            this.sourceCodeTabPage.Location = new System.Drawing.Point(4, 22);
            this.sourceCodeTabPage.Name = "sourceCodeTabPage";
            this.sourceCodeTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.sourceCodeTabPage.Size = new System.Drawing.Size(468, 197);
            this.sourceCodeTabPage.TabIndex = 0;
            this.sourceCodeTabPage.Text = "Source code";
            this.sourceCodeTabPage.UseVisualStyleBackColor = true;
            // 
            // errorsTabPage
            // 
            this.errorsTabPage.Controls.Add(this.errorListView);
            this.errorsTabPage.Location = new System.Drawing.Point(4, 22);
            this.errorsTabPage.Name = "errorsTabPage";
            this.errorsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.errorsTabPage.Size = new System.Drawing.Size(468, 197);
            this.errorsTabPage.TabIndex = 1;
            this.errorsTabPage.Text = "Errors (0)";
            this.errorsTabPage.UseVisualStyleBackColor = true;
            // 
            // errorListView
            // 
            this.errorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.numberHeader,
            this.descriptionHeader,
            this.lineHeader,
            this.columnHeader});
            this.errorListView.Location = new System.Drawing.Point(0, 0);
            this.errorListView.Name = "errorListView";
            this.errorListView.Size = new System.Drawing.Size(468, 197);
            this.errorListView.TabIndex = 0;
            this.errorListView.UseCompatibleStateImageBehavior = false;
            this.errorListView.View = System.Windows.Forms.View.Details;
            // 
            // numberHeader
            // 
            this.numberHeader.Text = "Number";
            this.numberHeader.Width = 78;
            // 
            // descriptionHeader
            // 
            this.descriptionHeader.Text = "Description";
            this.descriptionHeader.Width = 255;
            // 
            // lineHeader
            // 
            this.lineHeader.Text = "Line";
            this.lineHeader.Width = 63;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "Column";
            this.columnHeader.Width = 68;
            // 
            // ScriptExecuteOperationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tabControl1);
            this.Name = "ScriptExecuteOperationPanel";
            this.Size = new System.Drawing.Size(488, 225);
            ((System.ComponentModel.ISupportInitialize)(this.codeTextBox)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.sourceCodeTabPage.ResumeLayout(false);
            this.errorsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox codeTextBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage sourceCodeTabPage;
        private System.Windows.Forms.TabPage errorsTabPage;
        private System.Windows.Forms.ListView errorListView;
        private System.Windows.Forms.ColumnHeader descriptionHeader;
        private System.Windows.Forms.ColumnHeader lineHeader;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ColumnHeader numberHeader;
    }
}
