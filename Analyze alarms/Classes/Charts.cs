using System;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Forms;
using System.Windows.Media;

namespace Analyze_alarms.Classes
{
    public class Charts
    {
        private FontFamily fontFamily = new FontFamily("Microsoft Sans Serif");
        private Brush foreGround = Brushes.Black;
        private System.Windows.FontWeight fontWeight = System.Windows.FontWeights.Light;
        private TimeSpan animationSpeed = new TimeSpan(3000000);
        

        public Charts(TimeSpan animationSpeed)
        {
            this.animationSpeed = animationSpeed;
        }

        public Charts()
        {

        }

        public LiveCharts.WinForms.PieChart CreatePieChart(List<Summary> mySummary)
        {
            var chart = new LiveCharts.WinForms.PieChart();
            chart.Name = "pieControl";

            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            foreach (Summary s in mySummary)
            {
                var series = new PieSeries
                {
                    Title = s.MsgText,
                    Values = new ChartValues<int> { },
                    DataLabels = false,
                    Foreground = foreGround,
                    FontFamily = fontFamily,
                    FontWeight = fontWeight,
                    FontSize = 11,
                };
                chart.Series.Add(series);
                chart.Series[chart.Series.Count - 1].Values.Add(Convert.ToInt32(s.stopDuration.TotalMinutes));
            }

            chart.LegendLocation = LegendLocation.Right;
            chart.DefaultLegend.FontFamily = fontFamily;
            chart.DefaultLegend.FontWeight = fontWeight;
            chart.DefaultLegend.Margin = new System.Windows.Thickness(10, 1, 1, 1);
            chart.DefaultLegend.FontSize = 15;
            chart.DefaultLegend.Foreground = foreGround;
            System.Drawing.Image img = new System.Drawing.Bitmap(System.Environment.CurrentDirectory + "\\ChartBackground2.png");
            chart.BackgroundImage = img;
            chart.BackgroundImageLayout = ImageLayout.Stretch;
            chart.Dock = DockStyle.Fill;
            chart.Text = "Stop analysis";
            chart.AnimationsSpeed = animationSpeed;

            return chart;
        }


        public LiveCharts.WinForms.CartesianChart CreateRowChart(List<Summary> mySummary)
        {
            var chart = new LiveCharts.WinForms.CartesianChart();
            chart.Name = "rowControl";

            var seriesStopDuration = new RowSeries
            {
                Title = "Stop duration:",
                Values = new ChartValues<int> { },
                ScalesXAt = 0,
                DataLabels = true,
                LabelPoint = point => point.X + " minutes",
                Foreground = foreGround,
                FontFamily = fontFamily,
                FontWeight = fontWeight,
                FontSize = 11,
            };
            var seriesStopAmount = new RowSeries
            {
                Title = "Stop amount:",
                Values = new ChartValues<int> { },
                ScalesXAt = 1,
                DataLabels = true,
                LabelPoint = point => point.X + " times",
                Foreground = foreGround,
                FontFamily = fontFamily,
                FontWeight = fontWeight,
                FontSize = 11

            };

            var Y_Axis = new Axis
            {
                Title = "Stopcauses",
                Labels = new List<string>(),
                LabelsRotation = 15,
                Separator = new Separator
                {
                    Step = 1,
                    IsEnabled = false
                },
                Foreground = foreGround,
                FontFamily = fontFamily,
                FontWeight = fontWeight,

            };

            var X_Axis = new Axis
            {
                Title = "Time",
                Foreground = foreGround,
                FontFamily = fontFamily,
                FontWeight = fontWeight,
            };

            var X_Axis2 = new Axis
            {
                Title = "Amount",
                Position = AxisPosition.RightTop,
                Foreground = foreGround,
                FontFamily = fontFamily,
                FontWeight = fontWeight,
            };

            chart.Series.Add(seriesStopDuration);
            chart.Series.Add(seriesStopAmount);

            foreach (Summary s in mySummary)
            {
                chart.Series[0].Values.Add(Convert.ToInt32(s.stopDuration.TotalMinutes));
                chart.Series[1].Values.Add(s.Amount);
                Y_Axis.Labels.Add(s.MsgText);
            }

            chart.AxisX.Add(X_Axis);
            chart.AxisX.Add(X_Axis2);
            chart.AxisY.Add(Y_Axis);

            System.Drawing.Image img = new System.Drawing.Bitmap(System.Environment.CurrentDirectory + "\\ChartBackground2.png");
            chart.BackgroundImage = img;
            chart.BackgroundImageLayout = ImageLayout.Stretch;
            chart.Dock = DockStyle.Fill;
            chart.Text = "Stop analysis";
            chart.AnimationsSpeed = animationSpeed;

            return chart;

        }
    }
}
