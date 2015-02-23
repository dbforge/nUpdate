using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using nUpdate.Administration.Core;

namespace nUpdate.Administration.UI.Controls
{
    public partial class StatisticsChart : UserControl
    {
        public StatisticsChart()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The percentage of Windows Vista.
        /// </summary>
        public int WindowsVistaPercentage { get; set; }

        /// <summary>
        ///     The percentage of Windows 7.
        /// </summary>
        public int WindowsSevenPercentage { get; set; }

        /// <summary>
        ///     The percentage of Windows 8.
        /// </summary>
        public int WindowsEightPercentage { get; set; }

        /// <summary>
        ///     The percentage of Windows 8.1.
        /// </summary>
        public int WindowsEightPointOnePercentage { get; set; }

        /// <summary>
        ///     The percentage of Windows 10.
        /// </summary>
        public int WindowsTenPercentage { get; set; }

        /// <summary>
        ///     The version which relates to the current data.
        /// </summary>
        public UpdateVersion Version { get; set; }

        /// <summary>
        ///     The amount of total downloads for the current version.
        /// </summary>
        public int TotalDownloadCount { get; set; }

        private void StatisticsChart_Load(object sender, EventArgs e)
        {
            operatingSystemChart.Titles[0].Text = String.Format(operatingSystemChart.Titles[0].Text, Version.FullText);
            operatingSystemChart.Series[0]["PieLabelStyle"] = "Outside";

            double windowsVistaAmount = (TotalDownloadCount / 100d) * WindowsVistaPercentage;
            double windowsSevenAmount = (TotalDownloadCount / 100d) * WindowsSevenPercentage;
            double windowsEightAmount = (TotalDownloadCount / 100d) * WindowsEightPercentage;
            double windowsEightPointOneAmount = (TotalDownloadCount / 100d) * WindowsEightPointOnePercentage;
            double windowsTenAmount = (TotalDownloadCount / 100d) * WindowsTenPercentage;

            operatingSystemChart.Series[0].Points.Add(new DataPoint
            {
                Label = String.Format("Windows Vista ({0})", (int)windowsVistaAmount),
                YValues = new[] {(double) WindowsVistaPercentage}
            });
            operatingSystemChart.Series[0].Points.Add(new DataPoint
            {
                Label = String.Format("Windows 7 ({0})", (int)windowsSevenAmount),
                YValues = new[] {(double) WindowsSevenPercentage},
            });
            operatingSystemChart.Series[0].Points.Add(new DataPoint
            {
                Label = String.Format("Windows 8 ({0})", (int)windowsEightAmount),
                YValues = new[] {(double) WindowsEightPercentage}
            });
            operatingSystemChart.Series[0].Points.Add(new DataPoint
            {
                Label = String.Format("Windows 8.1 ({0})", (int)windowsEightPointOneAmount),
                YValues = new[] {(double) WindowsEightPointOnePercentage}
            });
            operatingSystemChart.Series[0].Points.Add(new DataPoint
            {
                Label = String.Format("Windows 10 ({0})", (int)windowsTenAmount),
                YValues = new[] {(double) WindowsTenPercentage}
            });

            foreach (var point in operatingSystemChart.Series[0].Points.Where(item => Math.Abs(item.YValues.First()) < 0.1))
            {
                point.CustomProperties = "PieLabelStyle = Disabled";
            }

            operatingSystemChart.Series[0].BorderWidth = 1;
            operatingSystemChart.Series[0].BorderColor = Color.FromArgb(26, 59, 105);

            operatingSystemChart.Legends.Add("Legend1");
            operatingSystemChart.Legends[0].Enabled = true;
            operatingSystemChart.Legends[0].Docking = Docking.Bottom;
            operatingSystemChart.Legends[0].Alignment = StringAlignment.Center;

            operatingSystemChart.Series[0].LegendText = "#PERCENT{P2}";
            operatingSystemChart.DataManipulator.Sort(PointSortOrder.Descending, operatingSystemChart.Series[0]);
        }

        private void closeLabel_Click(object sender, EventArgs e)
        {
            if (StatisticsChartClosed != null)
                StatisticsChartClosed(this, EventArgs.Empty);
        }

        public EventHandler<EventArgs> StatisticsChartClosed;

        private void closeLabel_MouseLeave(object sender, EventArgs e)
        {
           closeLabel.ForeColor = Color.IndianRed;
        }

        private void closeLabel_MouseEnter(object sender, EventArgs e)
        {
            closeLabel.ForeColor = Color.FromArgb(192, 0, 0);
        }
    }
}