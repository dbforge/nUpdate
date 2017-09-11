using nUpdate.Internal.UI.Controls;

namespace nUpdate.Administration.UI.Dialogs
{
    partial class ProjectRemovalDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectRemovalDialog));
            this.projectsTreeView = new nUpdate.Administration.UI.Controls.ExplorerTreeView();
            this.line1 = new Line();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.noProjectsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // projectsTreeView
            // 
            this.projectsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.projectsTreeView.CheckBoxes = true;
            this.projectsTreeView.HotTracking = true;
            this.projectsTreeView.Location = new System.Drawing.Point(0, 12);
            this.projectsTreeView.Name = "projectsTreeView";
            this.projectsTreeView.ShowLines = false;
            this.projectsTreeView.Size = new System.Drawing.Size(389, 216);
            this.projectsTreeView.TabIndex = 0;
            this.projectsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.projectsTreeView_AfterCheck);
            // 
            // line1
            // 
            this.line1.LineAlignment = Line.Alignment.Horizontal;
            this.line1.Location = new System.Drawing.Point(-6, 225);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(401, 10);
            this.line1.TabIndex = 1;
            this.line1.Text = "line1";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(28, 238);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(354, 41);
            this.label2.TabIndex = 33;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(11, 237);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 32;
            this.pictureBox1.TabStop = false;
            // 
            // noProjectsLabel
            // 
            this.noProjectsLabel.AutoSize = true;
            this.noProjectsLabel.Location = new System.Drawing.Point(135, 107);
            this.noProjectsLabel.Name = "noProjectsLabel";
            this.noProjectsLabel.Size = new System.Drawing.Size(117, 13);
            this.noProjectsLabel.TabIndex = 34;
            this.noProjectsLabel.Text = "No projects available.";
            this.noProjectsLabel.Visible = false;
            // 
            // ProjectRemovalDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(389, 288);
            this.Controls.Add(this.noProjectsLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.projectsTreeView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProjectRemovalDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project removal... - {0}";
            this.Load += new System.EventHandler(this.ProjectRemovalDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ExplorerTreeView projectsTreeView;
        private Line line1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label noProjectsLabel;



    }
}