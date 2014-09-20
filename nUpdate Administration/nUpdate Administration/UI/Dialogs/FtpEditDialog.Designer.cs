using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class FtpEditDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FtpEditDialog));
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.searchOnServerButton = new System.Windows.Forms.Button();
            this.line1 = new nUpdate.Administration.UI.Controls.Line();
            this.protocolComboBox = new System.Windows.Forms.ComboBox();
            this.protocolLabel = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.modeLabel = new System.Windows.Forms.Label();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new nUpdate.Administration.UI.Controls.WatermarkTextBox();
            this.userTextBox = new nUpdate.Administration.UI.Controls.WatermarkTextBox();
            this.hostTextBox = new nUpdate.Administration.UI.Controls.WatermarkTextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.userLabel = new System.Windows.Forms.Label();
            this.adressLabel = new System.Windows.Forms.Label();
            this.ftpHeaderLabel = new System.Windows.Forms.Label();
            this.controlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(386, 8);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(308, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.cancelButton);
            this.controlPanel1.Controls.Add(this.saveButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 192);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(474, 40);
            this.controlPanel1.TabIndex = 56;
            // 
            // searchOnServerButton
            // 
            this.searchOnServerButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.searchOnServerButton.Location = new System.Drawing.Point(317, 152);
            this.searchOnServerButton.Name = "searchOnServerButton";
            this.searchOnServerButton.Size = new System.Drawing.Size(119, 23);
            this.searchOnServerButton.TabIndex = 55;
            this.searchOnServerButton.Text = "Search on server";
            this.searchOnServerButton.UseVisualStyleBackColor = true;
            this.searchOnServerButton.Click += new System.EventHandler(this.searchOnServerButton_Click);
            // 
            // line1
            // 
            this.line1.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(14, 136);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(448, 10);
            this.line1.TabIndex = 54;
            this.line1.Text = "line1";
            // 
            // protocolComboBox
            // 
            this.protocolComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.protocolComboBox.FormattingEnabled = true;
            this.protocolComboBox.Items.AddRange(new object[] {
            "FTP",
            "FTPS (SSL 2.0 explicit)",
            "FTPS (SSL 2.0 implicit)",
            "FTPS (SSL 3.0 explicit)",
            "FTPS (SSL 3.0 implicit)",
            "FTPS (TLS explicit)",
            "FTPS (TLS implicit)"});
            this.protocolComboBox.Location = new System.Drawing.Point(305, 108);
            this.protocolComboBox.Name = "protocolComboBox";
            this.protocolComboBox.Size = new System.Drawing.Size(132, 21);
            this.protocolComboBox.TabIndex = 53;
            // 
            // protocolLabel
            // 
            this.protocolLabel.AutoSize = true;
            this.protocolLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.protocolLabel.Location = new System.Drawing.Point(239, 111);
            this.protocolLabel.Name = "protocolLabel";
            this.protocolLabel.Size = new System.Drawing.Size(53, 13);
            this.protocolLabel.TabIndex = 52;
            this.protocolLabel.Text = "Protocol:";
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "Passive (adviced)",
            "Active"});
            this.modeComboBox.Location = new System.Drawing.Point(89, 108);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(142, 21);
            this.modeComboBox.TabIndex = 51;
            // 
            // modeLabel
            // 
            this.modeLabel.AutoSize = true;
            this.modeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.modeLabel.Location = new System.Drawing.Point(18, 111);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(40, 13);
            this.modeLabel.TabIndex = 50;
            this.modeLabel.Text = "Mode:";
            // 
            // directoryLabel
            // 
            this.directoryLabel.AutoSize = true;
            this.directoryLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.directoryLabel.Location = new System.Drawing.Point(18, 156);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(56, 13);
            this.directoryLabel.TabIndex = 49;
            this.directoryLabel.Text = "Directory:";
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(88, 153);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(222, 22);
            this.directoryTextBox.TabIndex = 48;
            this.directoryTextBox.TabStop = false;
            this.directoryTextBox.Text = "/";
            // 
            // portTextBox
            // 
            this.portTextBox.Cue = "The port";
            this.portTextBox.Location = new System.Drawing.Point(305, 51);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(72, 22);
            this.portTextBox.TabIndex = 47;
            this.portTextBox.TabStop = false;
            this.portTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.portTextBox_KeyPress);
            // 
            // userTextBox
            // 
            this.userTextBox.Cue = "The username";
            this.userTextBox.Location = new System.Drawing.Point(89, 79);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(142, 22);
            this.userTextBox.TabIndex = 46;
            this.userTextBox.TabStop = false;
            // 
            // hostTextBox
            // 
            this.hostTextBox.Cue = "server.host.com";
            this.hostTextBox.Location = new System.Drawing.Point(89, 51);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(142, 22);
            this.hostTextBox.TabIndex = 45;
            this.hostTextBox.TabStop = false;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(304, 79);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(133, 22);
            this.passwordTextBox.TabIndex = 44;
            this.passwordTextBox.TabStop = false;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.portLabel.Location = new System.Drawing.Point(239, 54);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(31, 13);
            this.portLabel.TabIndex = 43;
            this.portLabel.Text = "Port:";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.passwordLabel.Location = new System.Drawing.Point(239, 82);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(59, 13);
            this.passwordLabel.TabIndex = 42;
            this.passwordLabel.Text = "Password:";
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.userLabel.Location = new System.Drawing.Point(17, 82);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(33, 13);
            this.userLabel.TabIndex = 41;
            this.userLabel.Text = "User:";
            // 
            // adressLabel
            // 
            this.adressLabel.AutoSize = true;
            this.adressLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.adressLabel.Location = new System.Drawing.Point(18, 54);
            this.adressLabel.Name = "adressLabel";
            this.adressLabel.Size = new System.Drawing.Size(34, 13);
            this.adressLabel.TabIndex = 40;
            this.adressLabel.Text = "Host:";
            // 
            // ftpHeaderLabel
            // 
            this.ftpHeaderLabel.AutoSize = true;
            this.ftpHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.ftpHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.ftpHeaderLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ftpHeaderLabel.Location = new System.Drawing.Point(12, 9);
            this.ftpHeaderLabel.Name = "ftpHeaderLabel";
            this.ftpHeaderLabel.Size = new System.Drawing.Size(70, 20);
            this.ftpHeaderLabel.TabIndex = 39;
            this.ftpHeaderLabel.Text = "FTP-Data";
            // 
            // FtpEditDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(474, 232);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.searchOnServerButton);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.protocolComboBox);
            this.Controls.Add(this.protocolLabel);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.modeLabel);
            this.Controls.Add(this.directoryLabel);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.userTextBox);
            this.Controls.Add(this.hostTextBox);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.userLabel);
            this.Controls.Add(this.adressLabel);
            this.Controls.Add(this.ftpHeaderLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FtpEditDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit FTP-data - ";
            this.Load += new System.EventHandler(this.FtpProjectEditDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button searchOnServerButton;
        private Controls.Line line1;
        private System.Windows.Forms.ComboBox protocolComboBox;
        private System.Windows.Forms.Label protocolLabel;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.TextBox directoryTextBox;
        private WatermarkTextBox portTextBox;
        private WatermarkTextBox userTextBox;
        private WatermarkTextBox hostTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.Label adressLabel;
        private System.Windows.Forms.Label ftpHeaderLabel;
        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
    }
}