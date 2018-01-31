using System;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using Analyze_alarms.Classes;
using System.Collections.Generic;

namespace Analyze_alarms.Forms
{
    public partial class PaintCharts : Form
    {
        private Charts charts = new Charts(new TimeSpan(1));
        private List<Summary> summaryCopy;
        public static bool showned = false;
        public Control rowChart, pieChart;
        private UC_NewLog parent;
        
        public PaintCharts(UC_NewLog parent, List<Summary> mySummary)
        {
            InitializeComponent();
            this.parent = parent;
            this.summaryCopy = mySummary;

            this.Size = new System.Drawing.Size(1280, 850);

            rowChart = charts.CreateRowChart(summaryCopy);
            pieChart = charts.CreatePieChart(summaryCopy);

            this.Controls.Add(rowChart);
            this.Controls.Add(pieChart);
                       
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            parent.rowChart = rowChart;
            parent.pieChart = pieChart;

            parent.PaintFormCompleted = true;
        }

        private void PaintCharts_Shown(object sender, EventArgs e)
        {
            this.Visible = false;           

            timer1.Interval = 500;
            timer1.Start();
        }
    }
}
