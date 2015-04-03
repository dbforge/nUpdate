namespace nUpdate.Administration.UI.Controls
{
    partial class StatisticsChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea9 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title9 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.operatingSystemChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.closeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.operatingSystemChart)).BeginInit();
            this.SuspendLayout();
            // 
            // operatingSystemChart
            // 
            this.operatingSystemChart.BackColor = System.Drawing.SystemColors.Window;
            chartArea9.Name = "ChartArea1";
            this.operatingSystemChart.ChartAreas.Add(chartArea9);
            this.operatingSystemChart.Location = new System.Drawing.Point(-1, -1);
            this.operatingSystemChart.Name = "operatingSystemChart";
            this.operatingSystemChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            this.operatingSystemChart.RightToLeft = System.Windows.Forms.RightToLeft.No;
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series9.IsValueShownAsLabel = true;
            series9.Name = "OperatingSystems";
            series9.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            this.operatingSystemChart.Series.Add(series9);
            this.operatingSystemChart.Size = new System.Drawing.Size(568, 350);
            this.operatingSystemChart.TabIndex = 0;
            this.operatingSystemChart.Text = "Operating systems";
            title9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title9.Name = "OperatingSystems";
            title9.Text = "Operating systems - {0}";
            this.operatingSystemChart.Titles.Add(title9);
            // 
            // closeLabel
            // 
            this.closeLabel.AutoSize = true;
            this.closeLabel.BackColor = System.Drawing.SystemColors.Window;
            this.closeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeLabel.ForeColor = System.Drawing.Color.IndianRed;
            this.closeLabel.Location = new System.Drawing.Point(535, -2);
            this.closeLabel.Name = "closeLabel";
            this.closeLabel.Size = new System.Drawing.Size(23, 30);
            this.closeLabel.TabIndex = 1;
            this.closeLabel.Text = "x";
            this.closeLabel.Click += new System.EventHandler(this.closeLabel_Click);
            this.closeLabel.MouseEnter += new System.EventHandler(this.closeLabel_MouseEnter);
            this.closeLabel.MouseLeave += new System.EventHandler(this.closeLabel_MouseLeave);
            // 
            // StatisticsChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.closeLabel);
            this.Controls.Add(this.operatingSystemChart);
            this.Name = "StatisticsChart";
            this.Size = new System.Drawing.Size(566, 348);
            this.Load += new System.EventHandler(this.StatisticsChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.operatingSystemChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart operatingSystemChart;
        private System.Windows.Forms.Label closeLabel;
    }
}
