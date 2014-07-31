namespace nUpdate.Administration.UI.Dialogs
{
    partial class StatisticsServerAddDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsServerAddDialog));
            this.continueButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.controlPanel1 = new nUpdate.Administration.UI.Controls.ControlPanel();
            this.tablessTabControl1 = new nUpdate.Administration.UI.Controls.TablessTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.watermarkTextBox3 = new nUpdate.Administration.WatermarkTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.watermarkTextBox4 = new nUpdate.Administration.WatermarkTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.watermarkTextBox1 = new nUpdate.Administration.WatermarkTextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.watermarkTextBox2 = new nUpdate.Administration.WatermarkTextBox();
            this.line2 = new nUpdate.Administration.UI.Controls.Line();
            this.controlPanel1.SuspendLayout();
            this.tablessTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(386, 8);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
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
            this.controlPanel1.Controls.Add(this.continueButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 314);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(471, 40);
            this.controlPanel1.TabIndex = 74;
            // 
            // tablessTabControl1
            // 
            this.tablessTabControl1.Controls.Add(this.tabPage1);
            this.tablessTabControl1.Controls.Add(this.tabPage2);
            this.tablessTabControl1.Location = new System.Drawing.Point(0, 35);
            this.tablessTabControl1.Name = "tablessTabControl1";
            this.tablessTabControl1.SelectedIndex = 0;
            this.tablessTabControl1.Size = new System.Drawing.Size(474, 284);
            this.tablessTabControl1.TabIndex = 82;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.watermarkTextBox3);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.pictureBox2);
            this.tabPage1.Controls.Add(this.watermarkTextBox4);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(466, 258);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // watermarkTextBox3
            // 
            this.watermarkTextBox3.Cue = "stats";
            this.watermarkTextBox3.Location = new System.Drawing.Point(99, 109);
            this.watermarkTextBox3.Name = "watermarkTextBox3";
            this.watermarkTextBox3.Size = new System.Drawing.Size(343, 22);
            this.watermarkTextBox3.TabIndex = 86;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(12, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 85;
            this.label7.Text = "FTP-directory:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(13, 49);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 84;
            this.pictureBox2.TabStop = false;
            // 
            // watermarkTextBox4
            // 
            this.watermarkTextBox4.Cue = "http://www.myserver.com/stats/";
            this.watermarkTextBox4.Location = new System.Drawing.Point(98, 135);
            this.watermarkTextBox4.Name = "watermarkTextBox4";
            this.watermarkTextBox4.Size = new System.Drawing.Size(343, 22);
            this.watermarkTextBox4.TabIndex = 83;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(12, 138);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 13);
            this.label9.TabIndex = 82;
            this.label9.Text = "HTTP(s)-Url:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(32, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(418, 39);
            this.label10.TabIndex = 81;
            this.label10.Text = "Note that a statistics server requires PHP 5 and MySQL 4 minimum. If you made \r\ns" +
    "ure that your server is running with these versions then follow the assistant to" +
    " \r\ncreate a new statistics server.\r\n";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(6, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 20);
            this.label11.TabIndex = 80;
            this.label11.Text = "General";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.textBox2);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.watermarkTextBox1);
            this.tabPage2.Controls.Add(this.pictureBox3);
            this.tabPage2.Controls.Add(this.watermarkTextBox2);
            this.tabPage2.Controls.Add(this.line2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(466, 258);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(98, 187);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(343, 22);
            this.textBox1.TabIndex = 94;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(6, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 20);
            this.label4.TabIndex = 81;
            this.label4.Text = "Add a statistics server";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(98, 159);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(343, 22);
            this.textBox2.TabIndex = 93;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(11, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 82;
            this.label5.Text = "Host/Server:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(10, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 92;
            this.label6.Text = "Password:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(11, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 83;
            this.label8.Text = "Database:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(11, 162);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 91;
            this.label12.Text = "Username:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(116, 105);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(306, 26);
            this.label14.TabIndex = 90;
            this.label14.Text = "If the database is already existing the old one will be deleted to\r\nguarantee no " +
    "problems during the setup of the necessary tables.";
            // 
            // watermarkTextBox1
            // 
            this.watermarkTextBox1.Cue = "The host/server of your MySQL-database to use";
            this.watermarkTextBox1.Location = new System.Drawing.Point(98, 48);
            this.watermarkTextBox1.Name = "watermarkTextBox1";
            this.watermarkTextBox1.Size = new System.Drawing.Size(343, 22);
            this.watermarkTextBox1.TabIndex = 85;
            this.watermarkTextBox1.TabStop = false;
            this.watermarkTextBox1.Text = "localhost";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(98, 104);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(16, 16);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 89;
            this.pictureBox3.TabStop = false;
            // 
            // watermarkTextBox2
            // 
            this.watermarkTextBox2.Cue = "The name of the database of your MySQL-Server to use";
            this.watermarkTextBox2.Location = new System.Drawing.Point(98, 76);
            this.watermarkTextBox2.Name = "watermarkTextBox2";
            this.watermarkTextBox2.Size = new System.Drawing.Size(343, 22);
            this.watermarkTextBox2.TabIndex = 86;
            this.watermarkTextBox2.TabStop = false;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.SystemColors.Window;
            this.line2.LineAlignment = nUpdate.Administration.UI.Controls.Line.Alignment.Horizontal;
            this.line2.Location = new System.Drawing.Point(4, 141);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(448, 10);
            this.line2.TabIndex = 88;
            this.line2.Text = "line2";
            // 
            // StatisticsServerAddDialog
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(471, 354);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.tablessTabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "StatisticsServerAddDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Statistics server - {0}";
            this.Load += new System.EventHandler(this.StatisticsServerAddDialog_Load);
            this.controlPanel1.ResumeLayout(false);
            this.tablessTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ControlPanel controlPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
        private Controls.TablessTabControl tablessTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private WatermarkTextBox watermarkTextBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox2;
        private WatermarkTextBox watermarkTextBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private WatermarkTextBox watermarkTextBox1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private WatermarkTextBox watermarkTextBox2;
        private Controls.Line line2;
    }
}