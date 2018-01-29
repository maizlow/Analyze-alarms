using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Analyze_alarms
{
    public partial class UC_NewLog : UserControl
    {
        private DataTable data;
        TimeSpan runTime = new TimeSpan();
        private int actualChartType = -1;
        //TODO: This data should be stored for future in DB
        //Summary data for summary tabpage
        private List<Classes.Summary> mySummary = new List<Classes.Summary>();
        private int nrOfSummaryEntrys;
        private Bitmap chartBitmaps;
        private LiveCharts.WinForms.CartesianChart mySavedCartesianChart;

        //First dimension = ClassNr, Second dimension = MessageNr
        private int[] ProdRunning_LB = { 0, 0 };
        private int[] ShiftActive_LB = { 0, 0 };

        public UC_NewLog(DataTable data)
        {
            InitializeComponent();

            this.data = data;
            InitDataTable(data);
            InitDateTimePickers();
            pictureBox1.Image = new Bitmap(System.Environment.CurrentDirectory + "\\ChartBackground2.png");
        }

        #region FUNCTIONS
        private void InitDateTimePickers()
        {
            dTP_From.Format = DateTimePickerFormat.Custom;
            dTP_From.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dTP_To.Format = DateTimePickerFormat.Custom;
            dTP_To.CustomFormat = "yyyy-MM-dd HH:mm:ss";
        }

        private void InitDataTable(DataTable data)
        {

            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.DataSource = data;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ColorDGVRow(int Index, Color color, DataGridView dgv)
        {
            dgv.Rows[Index].DefaultCellStyle.BackColor = color;
        }

        private void GetLogBitSettings()
        {
            foreach (LogSettings ls in MainForm.logSettings)
            {
                if (ls.isProdActiveLogBit)
                {
                    ProdRunning_LB = new int[] { ls.classNr, ls.messageNr };
                }

                if (ls.isShiftActiveLogBit)
                {
                    ShiftActive_LB = new int[] { ls.classNr, ls.messageNr };
                }

            }

        }

        private Classes.Summary CheckIfEntryExist(int MsgNr)
        {
            Classes.Summary sum = mySummary.Find(
            delegate (Classes.Summary summ)
            {
                return summ.MsgNumber == MsgNr;
            });

            return sum;
        }

        private void AnalyzeLogFile()
        {
            int StopCauseMsgNumber = 0; //Stop message number (To be stored in summaryList)
            string StopCauseMsgText = ""; //Stop message text (To be stored in summaryList)
            TimeSpan stopDuration = new TimeSpan(); //Total stop duration (To be stored in summaryList)

            DateTime startTime = DateTime.MinValue;
            DateTime stopTime = DateTime.MinValue;
            DateTime nextStart = DateTime.MinValue;
            int startIndex = 0; //Row index of last start
            int firstStopRow = 0;
            int lastStopRow = 0; //Row for last stop
            bool indirectStopFound = false;

            List<int> shiftStartsRows = new List<int>();
            List<int> shiftStopsRows = new List<int>();


            GetLogBitSettings();

            if (ProdRunning_LB != null)
            {
                //TODO: Handle multiple shifts. Should perhaps be done before creating UC to split logs into different TabPages eg. 180125 - Shift # 1
                #region TODO
                //if (ShiftActive_LB != null)
                //{

                //    //Find shifts
                //    foreach (DataRow dr in data.Rows)
                //    {
                //        if (Convert.ToInt16(dr["MsgClass"]) == ShiftActive_LB[0] && Convert.ToInt16(dr["MsgNumber"]) == ShiftActive_LB[1])
                //        {
                //            ColorDGVRow(data.Rows.IndexOf(dr), Color.LightPink);

                //            if (Convert.ToBoolean(dr["StateAfter"])) shiftStartsRows.Add(data.Rows.IndexOf(dr));
                //            else shiftStopsRows.Add(data.Rows.IndexOf(dr));
                //        }
                //    }
                //}
                #endregion

                foreach (DataRow dr in data.Rows)
                {
                    //Logbit production running: classnr 64 msgnr 128
                    if (Convert.ToInt16(dr["MsgClass"]) == ProdRunning_LB[0] && Convert.ToInt16(dr["MsgNumber"]) == ProdRunning_LB[1])
                    {
                        //Started production
                        if (Convert.ToBoolean(dr["StateAfter"]))
                        {
                            ColorDGVRow(data.Rows.IndexOf(dr), Color.LightGreen, dataGridView1);

                            //Get startTime
                            startTime = DateTime.Parse(dr["TimeString"].ToString());
                            startIndex = data.Rows.IndexOf(dr);
                            firstStopRow = startIndex + 1; //The first row of data after "Production started" must be treated as a potential stop
                                                           //Check that a stoptime is recorded before duration is recorded.

                        }
                        //Stopped production
                        else
                        {
                            ColorDGVRow(data.Rows.IndexOf(dr), Color.OrangeRed, dataGridView1);

                            //Get stopTime
                            stopTime = DateTime.Parse(dr["TimeString"].ToString());

                            //Startime has been recorded
                            if (startTime != DateTime.MinValue)
                            {
                                StopCauseMsgNumber = 0;
                                StopCauseMsgText = "";
                                //The last row before the Production stopped
                                lastStopRow = data.Rows.IndexOf(dr) - 1;

                                runTime += stopTime.Subtract(startTime);

                                //Find next start and calculate stoptime for present stop
                                int y = data.Rows.IndexOf(dr) + 1;
                                while (stopDuration.Milliseconds == 0 && y < data.Rows.Count)
                                {
                                    //2 = MsgClass , 3 = MsgNumber
                                    if (Convert.ToInt16(data.Rows[y].ItemArray[2].ToString()) == ProdRunning_LB[0] && Convert.ToInt16(data.Rows[y].ItemArray[3].ToString()) == ProdRunning_LB[1])
                                    {
                                        //Started production again. 1 = "StateAfter"
                                        if (Convert.ToBoolean(data.Rows[y].ItemArray[1]))
                                        {
                                            nextStart = DateTime.Parse(data.Rows[y].ItemArray[4].ToString());
                                            stopDuration = nextStart.Subtract(stopTime);
                                            break;
                                        }
                                    }
                                    y++;
                                }

                                //Check if lastStopRow is of Production running class
                                //Then it needs to be treated as an unknown stop and but the time on "Unknown"
                                if (Convert.ToInt16(data.Rows[lastStopRow].ItemArray[2]) == ProdRunning_LB[0])
                                {
                                    //TODO: Handle Unknown stop cause
                                    StopCauseMsgNumber = 9999;
                                    StopCauseMsgText = "Unknown cause";
                                }
                                else
                                {
                                    int stopCauseRow = 0;
                                    //Itirate between the potential stop rows
                                    for (int i = firstStopRow; i <= lastStopRow; i++)
                                    {
                                        if (Convert.ToBoolean(data.Rows[i].ItemArray[1]))
                                        {
                                            //Prep data
                                            int tempStopCauseClassNr = Convert.ToInt16(data.Rows[i].ItemArray[2]);
                                            int tempStopCauseMsgNr = Convert.ToInt16(data.Rows[i].ItemArray[3]);
                                            DateTime tempStopTimeStamp = Convert.ToDateTime(data.Rows[i].ItemArray[4]);
                                            string tempStopCauseMsgText = data.Rows[i].ItemArray[5].ToString();

                                            //Check vs. settings
                                            foreach (LogSettings ls in MainForm.logSettings)
                                            {
                                                //Direct stop class
                                                if (ls.classType == 2)
                                                {
                                                    if (ls.classNr == tempStopCauseClassNr)
                                                    {
                                                        StopCauseMsgNumber = tempStopCauseMsgNr;
                                                        StopCauseMsgText = tempStopCauseMsgText;
                                                        stopCauseRow = i;
                                                        break;
                                                    }
                                                }
                                                //Indirect stop class
                                                else if (ls.classType == 3)
                                                {
                                                    if (ls.classNr == tempStopCauseClassNr)
                                                    {
                                                        StopCauseMsgNumber = tempStopCauseMsgNr;
                                                        StopCauseMsgText = tempStopCauseMsgText;
                                                        stopCauseRow = i;
                                                        //If an indirect stop is found we need to search backwards for a potential subclass warning
                                                        indirectStopFound = true;
                                                    }
                                                }
                                            }

                                            if (indirectStopFound)
                                            {
                                                bool found = false;
                                                for (int c = firstStopRow; c > 0; c--)
                                                {
                                                    //Check vs. settings
                                                    foreach (LogSettings ls in MainForm.logSettings)
                                                    {
                                                        //Look for a class nr that match the indirect stops subclass member
                                                        if (ls.messageNr == StopCauseMsgNumber && ls.subClassMember == Convert.ToInt16(data.Rows[c].ItemArray[2]))
                                                        {
                                                            StopCauseMsgNumber = Convert.ToInt16(data.Rows[c].ItemArray[3]);
                                                            StopCauseMsgText = data.Rows[c].ItemArray[5].ToString();
                                                            stopCauseRow = c;
                                                            found = true;
                                                            break;
                                                        }
                                                    }
                                                    if (found) break;
                                                }
                                            }
                                            //Exit internal for-loop when found a match
                                            if (StopCauseMsgNumber > 0)
                                                break;
                                        }
                                    }


                                    ColorDGVRow(stopCauseRow, Color.Yellow, dataGridView1);
                                }

                                if (StopCauseMsgNumber > 0)
                                {
                                    //Record stopcause in summary
                                    Classes.Summary sum = CheckIfEntryExist(StopCauseMsgNumber);
                                    if (sum == null)
                                    {
                                        Classes.Summary newSum = new Classes.Summary();
                                        newSum.Amount++;
                                        newSum.MsgNumber = StopCauseMsgNumber;
                                        newSum.MsgText = StopCauseMsgText;
                                        newSum.stopDuration += stopDuration;
                                        mySummary.Add(newSum);
                                    }
                                    else
                                    {
                                        sum.Amount++;
                                        sum.MsgNumber = StopCauseMsgNumber;
                                        sum.MsgText = StopCauseMsgText;
                                        sum.stopDuration += stopDuration;
                                    }
                                }
                            }
                        }
                    }
                }
                //MessageBox.Show(mySummary.Count.ToString() + "   " + mySummary[0].stopDuration.ToString(@"HH\:mm\:ss"));
            }
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        private void CreateSummaryTab()
        {
            //Sort list in decsending order on stopDuration
            mySummary.Sort((a, b) => TimeSpan.Compare(b.stopDuration, a.stopDuration));
            //Bind the list to BindingList and then create a source, this ensure any editing in the DGV will
            //be reflected back to the list.
            var bindingList = new BindingList<Classes.Summary>(mySummary);
            var source = new BindingSource(bindingList, null);

            dataGridView2.DataSource = source;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.Columns[0].HeaderText = "Message nr.";
            dataGridView2.Columns[1].HeaderText = "Message text";
            dataGridView2.Columns[2].HeaderText = "Amount of stops";
            dataGridView2.Columns[3].HeaderText = "Total stop time";
            dataGridView2.AllowUserToResizeRows = false;

            int TotalAmountOfStops = 0;
            TimeSpan TotalStopTime = TimeSpan.Zero;
            TimeSpan TotalRunTime = TimeSpan.Zero;
            TimeSpan TotalTime = TimeSpan.Zero;

            nrOfSummaryEntrys = 0;

            foreach (Classes.Summary s in mySummary)
            {
                TotalAmountOfStops += s.Amount;
                TotalStopTime += s.stopDuration;
                nrOfSummaryEntrys++;
            }

            lbl_AmountOfStops.Text = TotalAmountOfStops.ToString();
            lbl_StopTime.Text = string.Format("{0:hh\\:mm\\:ss}", TotalStopTime);
            lbl_Runtime.Text = string.Format("{0:hh\\:mm\\:ss}", runTime);
            lbl_TotalTime.Text = string.Format("{0:hh\\:mm\\:ss}", TotalStopTime + runTime);
            //TODO: Implement shift functionality
            lbl_TotalActiveShiftTime.Text = "";

            label3.Visible = false;
            dataGridView2.Visible = true;
            panel1.Visible = true;
        }

        private LiveCharts.WinForms.CartesianChart CreateCartChart()
        {
            actualChartType = 0;
            var chart = new LiveCharts.WinForms.CartesianChart();
            chart.Name = "rowControl";

            var seriesStopDuration = new RowSeries
            {
                Title = "Stop duration:",
                Values = new ChartValues<int> { },
                ScalesXAt = 0,
                DataLabels = true,
                LabelPoint = point => point.X + " minutes",
                Foreground = System.Windows.Media.Brushes.Black,
                FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif"),
                FontWeight = System.Windows.FontWeights.Light,
                FontSize = 11,
            };
            var seriesStopAmount = new RowSeries
            {
                Title = "Stop amount:",
                Values = new ChartValues<int> { },
                ScalesXAt = 1,
                DataLabels = true,
                LabelPoint = point => point.X + " times",
                Foreground = System.Windows.Media.Brushes.Black,
                FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif"),
                FontWeight = System.Windows.FontWeights.Light,
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
                Foreground = System.Windows.Media.Brushes.Black,
                FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif"),
                FontWeight = System.Windows.FontWeights.Light,

            };

            var X_Axis = new Axis
            {
                Title = "Time",
                Foreground = System.Windows.Media.Brushes.Black,
                FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif"),
                FontWeight = System.Windows.FontWeights.Light,
            };

            var X_Axis2 = new Axis
            {
                Title = "Amount",
                Position = AxisPosition.RightTop,
                Foreground = System.Windows.Media.Brushes.Black,
                FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif"),
                FontWeight = System.Windows.FontWeights.Light,
            };

            chart.Series.Add(seriesStopDuration);
            chart.Series.Add(seriesStopAmount);
            
            foreach (Classes.Summary s in mySummary)
            {
                chart.Series[0].Values.Add(Convert.ToInt32(s.stopDuration.TotalMinutes));
                chart.Series[1].Values.Add(s.Amount);
                Y_Axis.Labels.Add(s.MsgText);                
            }

            chart.AxisX.Add(X_Axis);
            chart.AxisX.Add(X_Axis2);
            chart.AxisY.Add(Y_Axis);

            Image img = new Bitmap(System.Environment.CurrentDirectory + "\\ChartBackground2.png");
            chart.BackgroundImage = img;
            chart.BackgroundImageLayout = ImageLayout.Stretch;
            chart.Dock = DockStyle.Fill;
            chart.Text = "Stop analysis";

            return chart;

        }

        private LiveCharts.WinForms.PieChart CreatePieChart()
        {
            actualChartType = 1;
            var chart = new LiveCharts.WinForms.PieChart();
            chart.Name = "pieControl";

            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            foreach (Classes.Summary s in mySummary)
            {
                var series = new PieSeries
                {
                    Title = s.MsgText,
                    Values = new ChartValues<int> { },
                    DataLabels = false,
                    //LabelPoint = labelPoint,//point => point.Y + " min",
                    Foreground = System.Windows.Media.Brushes.Black,
                    FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif"),
                    FontWeight = System.Windows.FontWeights.Light,
                    FontSize = 11,
                };
                chart.Series.Add(series);
                chart.Series[chart.Series.Count - 1].Values.Add(Convert.ToInt32(s.stopDuration.TotalMinutes));
            }

            chart.LegendLocation = LegendLocation.Right;
            chart.DefaultLegend.FontFamily = new System.Windows.Media.FontFamily("Microsoft Sans Serif");
            chart.DefaultLegend.FontWeight = System.Windows.FontWeights.Light;
            chart.DefaultLegend.Margin = new System.Windows.Thickness(10, 1, 1, 1);
            chart.DefaultLegend.FontSize = 15;
            chart.DefaultLegend.Foreground = System.Windows.Media.Brushes.Black;
            Image img = new Bitmap(System.Environment.CurrentDirectory + "\\ChartBackground2.png");
            chart.BackgroundImage = img;
            chart.BackgroundImageLayout = ImageLayout.Stretch;
            chart.Dock = DockStyle.Fill;
            chart.Text = "Stop analysis";

            

            return chart;
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private Bitmap BitmapFromChart(Control myChart)
        {
            Bitmap bmp = new Bitmap(myChart.ClientSize.Width, myChart.ClientSize.Height);
           
            myChart.DrawToBitmap(bmp, myChart.ClientRectangle);

            int new_width = 1024;
            double aspect_ratio = (double)myChart.Height / (double)myChart.Width;
            double new_height = aspect_ratio * (double)new_width;
            
            bmp = ResizeImage((Image)bmp, new_width, (int)new_height);
            
            return bmp;
        }

        private void CreatePDF_Report()
        {
            
            var generator = new Classes.ReportGenerator("ALARM ANALYSIS " + DateTime.Today.ToShortDateString());

            Control[] temp = tp_Diagram.Controls.Find("rowControl", false);
            chartBitmaps = BitmapFromChart(temp[0]);
            var img = (Image)chartBitmaps;

            generator.charts = img;
            var openLogo = new OpenFileDialog();
            //string path;ALARM ANALYSIS " + DateTime.Today.ToShortDateString()
            //if (openLogo.ShowDialog() == DialogResult.OK)
            //{
            //    path = openLogo.FileName;
            //    generator.NewCustomerLogo(path);
            //}
            //else return;
            var filepath = generator.Generate(false);

            Process.Start(filepath);
        }

        /// <summary>
        /// Create a new diagram tab
        /// </summary>
        /// <param name="chartType">0 = Horizontal bar chart, 1 = Pie chart</param>
        private void CreateDiagramTab(int chartType)
        {
            pictureBox1.Visible = true;

            //TODO: Possible memory leak when swapping chart type.
            tp_Diagram.Controls.RemoveByKey("pieControl");
            tp_Diagram.Controls.RemoveByKey("rowControl");

            //Horizontal bar chart
            if (chartType == 0)
            {
                Control c = CreateCartChart();
                c.BringToFront();
                tp_Diagram.Controls.Add(c);
            }
            //Pie chart
            else if(chartType == 1)
            {
                Control c = CreatePieChart();
                c.BringToFront();
                tp_Diagram.Controls.Add(c);
            }

            var btn_Bar = new Button();
            btn_Bar.Click += new EventHandler(OnBarBtnClick);
            btn_Bar.Size = new Size(37, 34);
            var image = new Bitmap(System.Environment.CurrentDirectory + "\\RowChart.png");
            btn_Bar.Image = new Bitmap(image, 25, 25);
            btn_Bar.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Bar.Location = new Point(3, 3);

            var btn_Pie = new Button();
            btn_Pie.Click += new EventHandler(OnPieBtnClick);
            btn_Pie.Size = new Size(37, 34);
            var image2 = new Bitmap(System.Environment.CurrentDirectory + "\\PieChart.png");
            btn_Pie.Image = new Bitmap(image2, 25, 25);
            btn_Pie.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Pie.Location = new Point(btn_Bar.Location.X + btn_Bar.Width + 3, 3);

            tp_Diagram.Controls.Add(btn_Bar);
            tp_Diagram.Controls.Add(btn_Pie);

            btn_Bar.BringToFront();
            btn_Pie.BringToFront();
            label4.Visible = false;
            pictureBox1.Visible = false;

        }
        #endregion  

        #region EVENTS

        void OnBarBtnClick(object sender, EventArgs e)
        {
            if (actualChartType != 0) CreateDiagramTab(0);
        }

        void OnPieBtnClick(object sender, EventArgs e)
        {
            if (actualChartType != 1) CreateDiagramTab(1);
        }

        protected void btn_Filter_Click(object sender, EventArgs e)
        {
            DateTime from = dTP_From.Value;
            DateTime to = dTP_To.Value;
            DateTime row;

            for (int i = data.Rows.Count - 1; i >= 0; i--)
            {
                row = Convert.ToDateTime(data.Rows[i].ItemArray[4]);
                if (row != null)
                {
                    if (row < from || row > to)
                    {
                        data.Rows[i].Delete();
                    }
                }
            }
        }

        protected void dTP_From_ValueChanged(object sender, EventArgs e)
        {
            if (dTP_From.Value > dTP_To.Value)
            {
                dTP_From.Value = dTP_To.Value.AddHours(-1);
            }
        }

        protected void dTP_To_ValueChanged(object sender, EventArgs e)
        {
            if (dTP_From.Value > dTP_To.Value)
            {
                dTP_To.Value = dTP_From.Value.AddHours(1);
            }
        }

        public event EventHandler AnalyzeButtonClick;
        private void btn_Analyze_Click(object sender, EventArgs e)
        {
            //bubble the event up to the parent
            if (this.AnalyzeButtonClick != null)
                this.AnalyzeButtonClick(this, e);

            //Local handler code
            mySummary.Clear();
            runTime = TimeSpan.Zero;
            AnalyzeLogFile();
            CreateSummaryTab();
            CreateDiagramTab(0);
        }

        private void UC_NewLog_Load(object sender, EventArgs e)
        {

        }

        //Color every other line to increase visibility
        private void dataGridView2_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 1)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.TabIndex == 3)
                CreatePDF_Report();
        }
        #endregion

        private void tp_Report_Click(object sender, EventArgs e)
        {
            CreatePDF_Report();
        }
    }
}
