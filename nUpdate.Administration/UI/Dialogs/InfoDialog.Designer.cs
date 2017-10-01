using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class InfoDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoDialog));
            this.closeButton = new System.Windows.Forms.Button();
            this.donatePictureBox = new System.Windows.Forms.PictureBox();
            this.line1 = new Line();
            this.label13 = new System.Windows.Forms.Label();
            this.controlPanel1 = new BottomPanel();
            this.websiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label12 = new System.Windows.Forms.Label();
            this.ll_github = new System.Windows.Forms.LinkLabel();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.artentusLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.timSchieweLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.iconPackLinkLabel = new System.Windows.Forms.LinkLabel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.headerLabel = new System.Windows.Forms.Label();
            this.line2 = new Line();
            this.label1 = new System.Windows.Forms.Label();
            this.dotNetZipLinkLabel = new System.Windows.Forms.LinkLabel();
            this.jsonNetLinkLabel = new System.Windows.Forms.LinkLabel();
            this.fastColoredTextBoxLinkLabel = new System.Windows.Forms.LinkLabel();
            this.bikoLibraryLinklabel = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.iconLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.nafetsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.donatePictureBox)).BeginInit();
            this.controlPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(347, 7);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // donatePictureBox
            // 
            this.donatePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.donatePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("donatePictureBox.Image")));
            this.donatePictureBox.Location = new System.Drawing.Point(140, 289);
            this.donatePictureBox.Name = "donatePictureBox";
            this.donatePictureBox.Size = new System.Drawing.Size(147, 47);
            this.donatePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.donatePictureBox.TabIndex = 23;
            this.donatePictureBox.TabStop = false;
            this.donatePictureBox.Click += new System.EventHandler(this.donatePictureBox_Click);
            // 
            // line1
            // 
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(17, 157);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(401, 14);
            this.line1.TabIndex = 21;
            this.line1.Text = "line1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(134, 39);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(75, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "v3.2.1";
            // 
            // controlPanel1
            // 
            this.controlPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.controlPanel1.Controls.Add(this.closeButton);
            this.controlPanel1.Location = new System.Drawing.Point(0, 347);
            this.controlPanel1.Name = "controlPanel1";
            this.controlPanel1.Size = new System.Drawing.Size(434, 40);
            this.controlPanel1.TabIndex = 18;
            // 
            // websiteLinkLabel
            // 
            this.websiteLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.websiteLinkLabel.AutoSize = true;
            this.websiteLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.websiteLinkLabel.Location = new System.Drawing.Point(266, 254);
            this.websiteLinkLabel.Name = "websiteLinkLabel";
            this.websiteLinkLabel.Size = new System.Drawing.Size(71, 13);
            this.websiteLinkLabel.TabIndex = 17;
            this.websiteLinkLabel.TabStop = true;
            this.websiteLinkLabel.Text = "nupdate.net";
            this.websiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.websiteLinkLabel_LinkClicked);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(174, 254);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "or have a look at";
            // 
            // ll_github
            // 
            this.ll_github.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.ll_github.AutoSize = true;
            this.ll_github.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.ll_github.Location = new System.Drawing.Point(136, 254);
            this.ll_github.Name = "ll_github";
            this.ll_github.Size = new System.Drawing.Size(43, 13);
            this.ll_github.TabIndex = 15;
            this.ll_github.TabStop = true;
            this.ll_github.Text = "Github";
            this.ll_github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ll_github_LinkClicked);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 254);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(123, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Follow this project on ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(67, 221);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(306, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "for tips and the great \"ExplorerNavigationButton\"-control.";
            // 
            // artentusLinkLabel
            // 
            this.artentusLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.artentusLinkLabel.AutoSize = true;
            this.artentusLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.artentusLinkLabel.Location = new System.Drawing.Point(19, 220);
            this.artentusLinkLabel.Name = "artentusLinkLabel";
            this.artentusLinkLabel.Size = new System.Drawing.Size(51, 13);
            this.artentusLinkLabel.TabIndex = 12;
            this.artentusLinkLabel.TabStop = true;
            this.artentusLinkLabel.Text = "Artentus";
            this.artentusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.artentusLinkLabel_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(173, 172);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(239, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "as a contributor to the RsaSignature-class, to";
            // 
            // timSchieweLinkLabel
            // 
            this.timSchieweLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.timSchieweLinkLabel.AutoSize = true;
            this.timSchieweLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.timSchieweLinkLabel.Location = new System.Drawing.Point(108, 172);
            this.timSchieweLinkLabel.Name = "timSchieweLinkLabel";
            this.timSchieweLinkLabel.Size = new System.Drawing.Size(69, 13);
            this.timSchieweLinkLabel.TabIndex = 9;
            this.timSchieweLinkLabel.TabStop = true;
            this.timSchieweLinkLabel.Text = "Tim Schiewe";
            this.timSchieweLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.timSchieweLinkLabel_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Special thanks to ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(54, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Fugue Icon Package by";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Icons: ";
            // 
            // iconPackLinkLabel
            // 
            this.iconPackLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.iconPackLinkLabel.AutoSize = true;
            this.iconPackLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.iconPackLinkLabel.Location = new System.Drawing.Point(178, 98);
            this.iconPackLinkLabel.Name = "iconPackLinkLabel";
            this.iconPackLinkLabel.Size = new System.Drawing.Size(109, 13);
            this.iconPackLinkLabel.TabIndex = 5;
            this.iconPackLinkLabel.TabStop = true;
            this.iconPackLinkLabel.Text = "Yusuke Kamiyamane";
            this.iconPackLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.iconPackLinkLabel_LinkClicked);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.Location = new System.Drawing.Point(92, 39);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(45, 13);
            this.versionLabel.TabIndex = 4;
            this.versionLabel.Text = "Version";
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Location = new System.Drawing.Point(92, 57);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(195, 13);
            this.copyrightLabel.TabIndex = 3;
            this.copyrightLabel.Text = "Copyright © by Dominic Beger 2013-";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(21, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.headerLabel.Location = new System.Drawing.Point(91, 9);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(168, 20);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "nUpdate Administration";
            // 
            // line2
            // 
            this.line2.LineAlignment = Line.Alignment.Horizontal;
            this.line2.Location = new System.Drawing.Point(21, 273);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(397, 10);
            this.line2.TabIndex = 24;
            this.line2.Text = "line2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Used libraries:";
            // 
            // dotNetZipLinkLabel
            // 
            this.dotNetZipLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.dotNetZipLinkLabel.AutoSize = true;
            this.dotNetZipLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.dotNetZipLinkLabel.Location = new System.Drawing.Point(99, 141);
            this.dotNetZipLinkLabel.Name = "dotNetZipLinkLabel";
            this.dotNetZipLinkLabel.Size = new System.Drawing.Size(60, 13);
            this.dotNetZipLinkLabel.TabIndex = 26;
            this.dotNetZipLinkLabel.TabStop = true;
            this.dotNetZipLinkLabel.Text = "DotNetZip";
            this.dotNetZipLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dotNetZipLinkLabel_LinkClicked);
            // 
            // jsonNetLinkLabel
            // 
            this.jsonNetLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.jsonNetLinkLabel.AutoSize = true;
            this.jsonNetLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.jsonNetLinkLabel.Location = new System.Drawing.Point(165, 141);
            this.jsonNetLinkLabel.Name = "jsonNetLinkLabel";
            this.jsonNetLinkLabel.Size = new System.Drawing.Size(56, 13);
            this.jsonNetLinkLabel.TabIndex = 27;
            this.jsonNetLinkLabel.TabStop = true;
            this.jsonNetLinkLabel.Text = "JSON.NET";
            this.jsonNetLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.jsonNetLinkLabel_LinkClicked);
            // 
            // fastColoredTextBoxLinkLabel
            // 
            this.fastColoredTextBoxLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.fastColoredTextBoxLinkLabel.AutoSize = true;
            this.fastColoredTextBoxLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.fastColoredTextBoxLinkLabel.Location = new System.Drawing.Point(227, 141);
            this.fastColoredTextBoxLinkLabel.Name = "fastColoredTextBoxLinkLabel";
            this.fastColoredTextBoxLinkLabel.Size = new System.Drawing.Size(107, 13);
            this.fastColoredTextBoxLinkLabel.TabIndex = 28;
            this.fastColoredTextBoxLinkLabel.TabStop = true;
            this.fastColoredTextBoxLinkLabel.Text = "FastColoredTextBox";
            this.fastColoredTextBoxLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.fastColoredTextBoxLinkLabel_LinkClicked);
            // 
            // bikoLibraryLinklabel
            // 
            this.bikoLibraryLinklabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.bikoLibraryLinklabel.AutoSize = true;
            this.bikoLibraryLinklabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.bikoLibraryLinklabel.Location = new System.Drawing.Point(341, 141);
            this.bikoLibraryLinklabel.Name = "bikoLibraryLinklabel";
            this.bikoLibraryLinklabel.Size = new System.Drawing.Size(67, 13);
            this.bikoLibraryLinklabel.TabIndex = 29;
            this.bikoLibraryLinklabel.TabStop = true;
            this.bikoLibraryLinklabel.Text = "Biko Library";
            this.bikoLibraryLinklabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.bikoLibraryLinkLabel_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "nUpdate Icon made by";
            // 
            // iconLinkLabel
            // 
            this.iconLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.iconLinkLabel.AutoSize = true;
            this.iconLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.iconLinkLabel.Location = new System.Drawing.Point(177, 117);
            this.iconLinkLabel.Name = "iconLinkLabel";
            this.iconLinkLabel.Size = new System.Drawing.Size(92, 13);
            this.iconLinkLabel.TabIndex = 30;
            this.iconLinkLabel.TabStop = true;
            this.iconLinkLabel.Text = "Stefan Baumann";
            this.iconLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.iconLinkLabel_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(306, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "for helping with Regular Expressions, Constantin Tillmann";
            // 
            // nafetsLinkLabel
            // 
            this.nafetsLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.nafetsLinkLabel.AutoSize = true;
            this.nafetsLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.nafetsLinkLabel.Location = new System.Drawing.Point(18, 188);
            this.nafetsLinkLabel.Name = "nafetsLinkLabel";
            this.nafetsLinkLabel.Size = new System.Drawing.Size(92, 13);
            this.nafetsLinkLabel.TabIndex = 33;
            this.nafetsLinkLabel.TabStop = true;
            this.nafetsLinkLabel.Text = "Stefan Baumann";
            this.nafetsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.nafetsLinkLabel_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(382, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "for testing the support of large packages and providing information and";
            // 
            // InfoDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(434, 387);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nafetsLinkLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.iconLinkLabel);
            this.Controls.Add(this.bikoLibraryLinklabel);
            this.Controls.Add(this.fastColoredTextBoxLinkLabel);
            this.Controls.Add(this.jsonNetLinkLabel);
            this.Controls.Add(this.dotNetZipLinkLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.line2);
            this.Controls.Add(this.donatePictureBox);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.controlPanel1);
            this.Controls.Add(this.websiteLinkLabel);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.ll_github);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.artentusLinkLabel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.timSchieweLinkLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.iconPackLinkLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.headerLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Information - {0}";
            this.Load += new System.EventHandler(this.InfoForm_Load);
            this.Shown += new System.EventHandler(this.InfoForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.donatePictureBox)).EndInit();
            this.controlPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.LinkLabel iconPackLinkLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel timSchieweLinkLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel artentusLinkLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel ll_github;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.LinkLabel websiteLinkLabel;
        private BottomPanel controlPanel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label13;
        private Line line1;
        private System.Windows.Forms.PictureBox donatePictureBox;
        private Line line2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel dotNetZipLinkLabel;
        private System.Windows.Forms.LinkLabel jsonNetLinkLabel;
        private System.Windows.Forms.LinkLabel fastColoredTextBoxLinkLabel;
        private System.Windows.Forms.LinkLabel bikoLibraryLinklabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel iconLinkLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel nafetsLinkLabel;
        private System.Windows.Forms.Label label4;
    }
}