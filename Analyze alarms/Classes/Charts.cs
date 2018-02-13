using System;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using LiveCharts.Configurations;
using System.Windows.Forms;
using System.Windows.Media;
using System.Reflection;

namespace Analyze_alarms.Classes
{
    public class DateModel
    {
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }
    }

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
                chart.Series[chart.Series.Count - 1].Values.Add(Convert.ToInt32(s.StopDuration.TotalMinutes));
            }

            chart.LegendLocation = LegendLocation.Right;
            chart.DefaultLegend.FontFamily = fontFamily;
            chart.DefaultLegend.FontWeight = fontWeight;
            chart.DefaultLegend.Margin = new System.Windows.Thickness(10, 1, 1, 1);
            chart.DefaultLegend.FontSize = 15;
            chart.DefaultLegend.Foreground = foreGround;
            System.Drawing.Image img = new System.Drawing.Bitmap(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ChartBackground2.png");
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
                chart.Series[0].Values.Add(Convert.ToInt32(s.StopDuration.TotalMinutes));
                chart.Series[1].Values.Add(s.Amount);
                Y_Axis.Labels.Add(s.MsgText);
            }

            chart.AxisX.Add(X_Axis);
            chart.AxisX.Add(X_Axis2);
            chart.AxisY.Add(Y_Axis);

            System.Drawing.Image img = new System.Drawing.Bitmap(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ChartBackground2.png");
            chart.BackgroundImage = img;
            chart.BackgroundImageLayout = ImageLayout.Stretch;
            chart.Dock = DockStyle.Fill;
            chart.Text = "Stop analysis";
            chart.AnimationsSpeed = animationSpeed;

            return chart;

        }


        public class DateModel
        {
            public System.DateTime DateTime { get; set; }
            public double Value { get; set; }
        }

        //TODO: Dettime scatter chart to show frequency at certain times of the day
        public LiveCharts.WinForms.CartesianChart CreateScatterChart(List<AlarmInterval> alarmIntervals)
        {
            var chart = new LiveCharts.WinForms.CartesianChart();
            chart.Name = "scatterControl";

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(dayModel => dayModel.Value);

            //ScatterSeries series = new ScatterSeries(dayConfig);
            var r = new Random();
            var mySeries = new SeriesCollection(dayConfig);


            ChartValues<DateModel> val = new ChartValues<DateModel>();
            ScatterSeries s = new ScatterSeries(dayConfig);

            foreach (AlarmInterval a in alarmIntervals)
            {
                s = new ScatterSeries(dayConfig)
                {
                    Title = a.AlarmText,
                    MaxPointShapeDiameter = 20,
                    PointGeometry = DefaultGeometries.Diamond,
                    Fill = Brushes.Blue
                };

                mySeries.Add(s);

                val.Add(new DateModel
                {
                    DateTime = a.TimeStamp,
                    Value = a.Duration.TotalSeconds + 20
                });

                mySeries[mySeries.Count - 1].Values = val;
            }

            chart.Series = mySeries;

            chart.AxisX.Add(new Axis
            {
                LabelFormatter = value => new System.DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("t"),
                Foreground = foreGround
            });

            chart.AxisY.Add(new Axis
            {
                Title = "Duration [min]",
                MinValue = 0,
                Foreground = foreGround
            });

            //    //series.Values = q;

            //    //    TimeSpan logLength = finish.Subtract(start);

            //    //    chart.AxisX.Add(new Axis
            //    //    {
            //    //        Foreground = foreGround,
            //    //        LabelFormatter = new Func<double, string>(va => va * start.ToOADate()).ToString("HH:mm");
            //    //});

            System.Drawing.Image img = new System.Drawing.Bitmap(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\ChartBackground2.png");
            chart.BackgroundImage = img;
            chart.BackgroundImageLayout = ImageLayout.Stretch;
            chart.Dock = DockStyle.Fill;
            chart.Text = "Stop analysis";
            chart.DisableAnimations = true;
            chart.ForeColor = System.Drawing.Color.Black;
            
            return chart;
        }
    }
}
