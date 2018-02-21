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
using System.Threading;
using Analyze_alarms.Classes;

namespace Analyze_alarms
{
    public partial class UC_NewLog : UserControl
    {
        private MainForm parent;
        private DataTable data;
        TimeSpan runTime = new TimeSpan();
        private string folderPath;

        public List<Summary> mySummary = new List<Classes.Summary>();
        public ReportFormData myReportFormData;// = new Classes.ReportFormData();
        public List<DataTableRowClass> myDataTableRowsList = new List<Classes.DataTableRowClass>();
        public List<AnalyzedRows> analyzedRows = new List<AnalyzedRows>();
        public List<AlarmInterval> alarmIntervals = new List<AlarmInterval>();

        ReportTab reportData;
        private int nrOfSummaryEntrys;
        private TabPage tp_Report;
        private Charts myCharts = new Classes.Charts();
        public Control rowChart, pieChart;
        ReportGenerator generator;
        private bool paintFormCompleted;
        public bool PaintFormCompleted
        {
            set
            {
                paintFormCompleted = value;
                paintChartFormDone(ref generator, ref generator.reportTab);
            }

        }
        private bool Analyzed;

        //First dimension = ClassNr, Second dimension = MessageNr
        private int[] ProdRunning_LB = { 0, 0 };
        private int[] ShiftActive_LB = { 0, 0 };
        

        //Constructor
        public UC_NewLog(DataTable data, MainForm parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.data = data;
            Init(data);
            ConvertDataTableToClass(data);
        }


        public UC_NewLog(string controlName, MainForm parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.Name = controlName;
        }

        #region FUNCTIONS

        public void Init(DataTable data)
        {   
            folderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            InitDataTable(data);
            InitDateTimePickers();
            this.Dock = DockStyle.Fill;
            pictureBox1.Image = new Bitmap(folderPath + "\\ChartBackground2.png");
        }

        public void PopulateDataFromDB(List<Summary> summaryList, ReportFormData reportFormData, List<DataTableRowClass> dataRowList, List<AnalyzedRows> analyzedRows)
        {
            this.mySummary = summaryList;
            this.myReportFormData = reportFormData;
            this.myDataTableRowsList = dataRowList;
            this.analyzedRows = analyzedRows;

            this.data = ConvertDataClassToDataTable(myDataTableRowsList);
            Init(data);

            foreach (AnalyzedRows a in analyzedRows)
            {
                ColorDGVRow(a.rowNr, Color.FromArgb(a.color), dataGridView1);
            }

            CreateSummaryTab();
            CreateDiagramTab();
            CreateReportTab(true);
        }

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
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;            
        }

        private void ConvertDataTableToClass(DataTable data)
        {
            DataTableRowClass temp = new DataTableRowClass();
            for (int i = 0; i < data.Rows.Count - 1; i++)
            {
                temp = new DataTableRowClass(double.Parse(data.Rows[i].ItemArray[1].ToString()),
                                                       short.Parse(data.Rows[i].ItemArray[2].ToString()),
                                                       short.Parse(data.Rows[i].ItemArray[3].ToString()),
                                                       short.Parse(data.Rows[i].ItemArray[4].ToString()),
                                                       DateTime.Parse(data.Rows[i].ItemArray[5].ToString()),
                                                       data.Rows[i].ItemArray[6].ToString());
                myDataTableRowsList.Add(temp);
            }

        }

        private DataTable ConvertDataClassToDataTable(List<DataTableRowClass> dtrc)
        {
            if (dtrc != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Id"); //0
                dt.Columns[0].DataType = typeof(Int32);
                dt.Columns.Add("Time_ms"); //1
                dt.Columns[1].DataType = typeof(double);
                dt.Columns.Add("StateAfter"); //2
                dt.Columns[2].DataType = typeof(short);
                dt.Columns.Add("MsgClass"); //3
                dt.Columns[3].DataType = typeof(short);
                dt.Columns.Add("MsgNumber"); //4
                dt.Columns[4].DataType = typeof(short);
                dt.Columns.Add("TimeString"); //5
                dt.Columns[5].DataType = typeof(DateTime);
                dt.Columns.Add("MsgText"); //6
                dt.Columns[6].DataType = typeof(string);

                foreach (DataTableRowClass dtr in dtrc)
                {
                    dt.Rows.Add(dtr.Id, dtr.Time_Ms, dtr.StateAfter, dtr.MsgClass, dtr.MsgNumber, dtr.TimeString, dtr.MsgText);
                }

                return dt;
            }
            return null;
        }

        private void ColorDGVRow(int Index, Color color, DataGridView dgv)
        {
            if (Index < dgv.Rows.Count) dgv.Rows[Index].DefaultCellStyle.BackColor = color;            
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

        private Summary CheckIfEntryExist(int MsgNr)
        {
            Classes.Summary sum = mySummary.Find(
            delegate (Summary summ)
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
                            analyzedRows.Add(new AnalyzedRows(data.Rows.IndexOf(dr), Color.LightGreen));

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
                            analyzedRows.Add(new AnalyzedRows(data.Rows.IndexOf(dr), Color.OrangeRed));

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
                                    if (Convert.ToInt16(data.Rows[y].ItemArray[3].ToString()) == ProdRunning_LB[0] && Convert.ToInt16(data.Rows[y].ItemArray[4].ToString()) == ProdRunning_LB[1])
                                    {
                                        //Started production again. 1 = "StateAfter"
                                        if (Convert.ToBoolean(data.Rows[y].ItemArray[2]))
                                        {
                                            nextStart = DateTime.Parse(data.Rows[y].ItemArray[5].ToString());
                                            stopDuration = nextStart.Subtract(stopTime);
                                            break;
                                        }
                                    }
                                    y++;
                                }

                                //Always assume Unknown
                                StopCauseMsgNumber = 9999;
                                StopCauseMsgText = "Unknown cause";

                                int stopCauseRow = 0;
                                //Check if lastStopRow is of Production running class
                                //Then it needs to be treated as an unknown stop and put the time on "Unknown"
                                if (Convert.ToInt16(data.Rows[lastStopRow].ItemArray[3]) == ProdRunning_LB[0])
                                {
                                    //Check vs. settings
                                    foreach (LogSettings ls in MainForm.logSettings)
                                    {
                                        //Direct stop class
                                        if (ls.classType == 2)
                                        {
                                            if (ls.classNr == Convert.ToInt16(data.Rows[lastStopRow + 2].ItemArray[3]))
                                            {
                                                StopCauseMsgNumber = Convert.ToInt16(data.Rows[lastStopRow + 2].ItemArray[4]); ;
                                                StopCauseMsgText = data.Rows[lastStopRow + 2].ItemArray[6].ToString();
                                                stopCauseRow = lastStopRow + 2;
                                                break;
                                            }
                                        }
                                    }


                                }
                                else
                                {
                                    //Itirate between the potential stop rows
                                    for (int i = firstStopRow; i <= lastStopRow; i++)
                                    {
                                        if (Convert.ToBoolean(data.Rows[i].ItemArray[2]))
                                        {
                                            //Prep data
                                            int tempStopCauseClassNr = Convert.ToInt16(data.Rows[i].ItemArray[3]);
                                            int tempStopCauseMsgNr = Convert.ToInt16(data.Rows[i].ItemArray[4]);
                                            DateTime tempStopTimeStamp = Convert.ToDateTime(data.Rows[i].ItemArray[5]);
                                            string tempStopCauseMsgText = data.Rows[i].ItemArray[6].ToString();

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
                                                indirectStopFound = false;
                                                bool found = false;
                                                for (int c = lastStopRow; c > 0; c--)
                                                {
                                                    //Check vs. settings
                                                    foreach (LogSettings ls in MainForm.logSettings)
                                                    {
                                                        //Look for a class nr that match the indirect stops subclass member
                                                        if (ls.messageNr == StopCauseMsgNumber && ls.subClassMember == Convert.ToInt16(data.Rows[c].ItemArray[3]))
                                                        {
                                                            if (Convert.ToBoolean(data.Rows[c].ItemArray[2]) == true)
                                                            {
                                                                StopCauseMsgNumber = Convert.ToInt16(data.Rows[c].ItemArray[4]);
                                                                StopCauseMsgText = data.Rows[c].ItemArray[6].ToString();
                                                                stopCauseRow = c;
                                                                found = true;
                                                                break;
                                                            }
                                                            //If finding a subclass that match but if it is allread state 0 set alarm to Indirect class
                                                            else
                                                            {
                                                                found = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (found) break;
                                                }
                                            }
                                            //Exit internal for-loop when found a match
                                            if (StopCauseMsgNumber > 0 && StopCauseMsgNumber != 9999 || (StopCauseMsgNumber == 9999 && i == lastStopRow))
                                                break;
                                        }
                                    }
                                }

                                if (StopCauseMsgNumber > 0 && StopCauseMsgNumber != 9999)
                                { 
                                    ColorDGVRow(stopCauseRow, Color.Yellow, dataGridView1);
                                    analyzedRows.Add(new AnalyzedRows(stopCauseRow, Color.Yellow));
                                }

                                if (StopCauseMsgNumber > 0)
                                {
                                    //Debug
                                    //if (StopCauseMsgNumber == 9999)
                                    //    Debug.WriteLine(lastStopRow.ToString());
                                    //Record stopcause in summary
                                    Summary sum = CheckIfEntryExist(StopCauseMsgNumber);
                                    if (sum == null)
                                    {
                                        Summary newSum = new Summary();
                                        newSum.Amount++;
                                        newSum.MsgNumber = StopCauseMsgNumber;
                                        newSum.MsgText = StopCauseMsgText;
                                        newSum.StopDuration += stopDuration;
                                        newSum.RunTime += runTime;
                                        mySummary.Add(newSum);
                                        runTime = TimeSpan.Zero;
                                    }
                                    else
                                    {
                                        sum.Amount++;
                                        sum.MsgNumber = StopCauseMsgNumber;
                                        sum.MsgText = StopCauseMsgText;
                                        sum.StopDuration += stopDuration;
                                        sum.RunTime += runTime;
                                        runTime = TimeSpan.Zero;
                                    }
                                    if (alarmIntervals != null)
                                    {
                                        alarmIntervals.Add(new AlarmInterval(stopTime, stopDuration, StopCauseMsgText));
                                    }
                                    else
                                    {
                                        alarmIntervals = new List<AlarmInterval>();
                                        alarmIntervals.Add(new AlarmInterval(stopTime, stopDuration, StopCauseMsgText));
                                    }
                                }
                            }
                        }
                    }
                }
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
            mySummary.Sort((a, b) => TimeSpan.Compare(b.StopDuration, a.StopDuration));
            //Bind the list to BindingList and then create a source, this ensure any editing in the DGV will
            //be reflected back to the list.
            var bindingList = new BindingList<Summary>(mySummary);
            var source = new BindingSource(bindingList, null);
            
            dataGridView2.DataSource = source;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].HeaderText = "Message nr.";
            dataGridView2.Columns[2].HeaderText = "Message text";
            dataGridView2.Columns[3].HeaderText = "Amount of stops";
            dataGridView2.Columns[4].HeaderText = "Total stop time";
            dataGridView2.Columns[5].Visible = false;
            dataGridView2.AllowUserToResizeRows = false;

            int TotalAmountOfStops = 0;
            TimeSpan TotalStopTime = TimeSpan.Zero;
            TimeSpan TotalRunTime = TimeSpan.Zero;
            TimeSpan TotalTime = TimeSpan.Zero;

            nrOfSummaryEntrys = 0;

            foreach (Classes.Summary s in mySummary)
            {
                TotalAmountOfStops += s.Amount;
                TotalStopTime += s.StopDuration;
                TotalRunTime += s.RunTime;
                nrOfSummaryEntrys++;
            }

            lbl_AmountOfStops.Text = TotalAmountOfStops.ToString();
            lbl_StopTime.Text = string.Format("{0:hh\\:mm\\:ss}", TotalStopTime);
            lbl_Runtime.Text = string.Format("{0:hh\\:mm\\:ss}", TotalRunTime);
            lbl_TotalTime.Text = string.Format("{0:hh\\:mm\\:ss}", TotalStopTime + TotalRunTime);
            //TODO: Implement shift functionality
            lbl_TotalActiveShiftTime.Text = "";

            label3.Visible = false;
            dataGridView2.Visible = true;
            panel1.Visible = true;
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

        /// <summary>
        /// Create a new diagram tab
        /// </summary>
        /// <param name="chartType">0 = Horizontal bar chart, 1 = Pie chart</param>
        private void CreateDiagramTab()
        {
            pictureBox1.Visible = true;

            //Horizontal bar chart
            if (!tp_Diagram.Controls.ContainsKey("rowControl"))
            {
                Control c = myCharts.CreateRowChart(mySummary);
                tp_Diagram.Controls.Add(c);
            }
            else
            {
                tp_Diagram.Controls.RemoveByKey("rowControl");
                Control c = myCharts.CreateRowChart(mySummary);
                tp_Diagram.Controls.Add(c);
            }
            //Pie chart
            if (!tp_Diagram.Controls.ContainsKey("pieControl"))
            {
                Control c = myCharts.CreatePieChart(mySummary);
                tp_Diagram.Controls.Add(c);
            }
            else
            {
                tp_Diagram.Controls.RemoveByKey("pieControl");
                Control c = myCharts.CreatePieChart(mySummary);
                tp_Diagram.Controls.Add(c);
            }

            //Scatter chart
            //if (!tp_Diagram.Controls.ContainsKey("scatterControl"))
            //{
            //    Control c = myCharts.CreateScatterChart(alarmIntervals);
            //    tp_Diagram.Controls.Add(c);
            //}

            var btn_Row = new Button();
            btn_Row.Name = "btn_Row";
            btn_Row.Click += new EventHandler(OnBarBtnClick);
            btn_Row.Size = new Size(37, 34);
            var image = new Bitmap(folderPath + "\\RowChart.png");
            btn_Row.Image = new Bitmap(image, 25, 25);
            btn_Row.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Row.Location = new Point(3, 3);

            var btn_Pie = new Button();
            btn_Pie.Name = "btn_Pie";
            btn_Pie.Click += new EventHandler(OnPieBtnClick);
            btn_Pie.Size = new Size(37, 34);
            var image2 = new Bitmap(folderPath + "\\PieChart.png");
            btn_Pie.Image = new Bitmap(image2, 25, 25);
            btn_Pie.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Pie.Location = new Point(btn_Row.Location.X + btn_Row.Width + 3, 3);

            tp_Diagram.Controls.Add(btn_Row);
            tp_Diagram.Controls.Add(btn_Pie);

            tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("rowControl")].BringToFront();
            //TODO: tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("scatterControl")].BringToFront();
            //VERY SLOW CODE
            btn_Row.BringToFront();
            btn_Pie.BringToFront();
            label4.Visible = false;
            pictureBox1.Visible = false;

        }

        private void CreateReportTab(bool fromDB = false)
        {
            
            if (myReportFormData != null)
            {
                reportData = new ReportTab(this, myReportFormData);                
            }
            else
            {
                myReportFormData = new ReportFormData();
                myReportFormData.Init();
                reportData = new ReportTab(this, myReportFormData);
            }     
            
            if (!fromDB) tp_Report = reportData.CreateTabPage();
            else tp_Report = reportData.CreateTabPage(fromDB);
            
            tabControl1.TabPages.Add(tp_Report);
        }

        public void StartCreatePDFReport(Classes.ReportTab rd)
        {
            generator = new ReportGenerator(this, rd);

            Cursor = Cursors.WaitCursor;

            rd.AddAttachmentsToReport(ref generator);

            if (myReportFormData.customLogoPath != null) generator.NewCustomerLogo(myReportFormData.customLogoPath);

            if (myReportFormData.chk_PieChart_Checked || myReportFormData.chk_RowChart_Checked)
            {
                rd.paintChartsForm = new Forms.PaintCharts(this, mySummary);
                rd.paintChartsForm.Show();
                //After the paintForm is showned it will draw charts and then set UC_NewLog property PaintFormCompleted,
                //this will in turn call function paintChartFormDone();
            }
            else
            {
                GenerateReport(ref rd);
            }
        }

        private void paintChartFormDone(ref Classes.ReportGenerator generator, ref Classes.ReportTab rd)
        {

            if (rowChart != null)
                generator.rowChart = (Image)BitmapFromChart(rowChart);

            if (pieChart != null)
                generator.pieChart = (Image)BitmapFromChart(pieChart);

            rd.paintChartsForm.Close();
            rd.paintChartsForm.Dispose();

            GenerateReport(ref rd);
        }

        private void GenerateReport(ref Classes.ReportTab rd)
        {
            if (rd.saveReportFilePath != null && myReportFormData != null)
            {
                var filepath = generator.Generate(rd.saveReportFilePath);
                Process.Start(filepath);                
            }
            Cursor = Cursors.Default;
        }

        #endregion

        #region EVENTS

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                tp_Report.Dispose();
                reportData.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnBarBtnClick(object sender, EventArgs e)
        {
            if (tp_Diagram.Controls.ContainsKey("rowControl") && tp_Diagram.Controls.ContainsKey("pieControl"))
            {
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("rowControl")].BringToFront();
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("pieControl")].SendToBack();
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("btn_Row")].BringToFront();
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("btn_Pie")].BringToFront();
            }
        }

        //TODO: DEGUG ONLY
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //var grid = sender as DataGridView;
            //var rowIdx = (e.RowIndex).ToString();

            //var centerFormat = new StringFormat()
            //{
            //    // right alignment might actually make more sense for numbers
            //    Alignment = StringAlignment.Center,
            //    LineAlignment = StringAlignment.Center
            //};

            //var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            //e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void OnPieBtnClick(object sender, EventArgs e)
        {
            if (tp_Diagram.Controls.ContainsKey("rowControl") && tp_Diagram.Controls.ContainsKey("pieControl"))
            {
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("pieControl")].BringToFront();
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("rowControl")].SendToBack();
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("btn_Row")].BringToFront();
                tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("btn_Pie")].BringToFront();
            }
        }

        protected void btn_Filter_Click(object sender, EventArgs e)
        {
            DateTime from = dTP_From.Value;
            DateTime to = dTP_To.Value;
            DateTime row;

            for (int i = data.Rows.Count - 1; i >= 0; i--)
            {
                row = Convert.ToDateTime(data.Rows[i].ItemArray[5]);
                if (row != null)
                {
                    if (row < from || row > to)
                    {
                        data.Rows[i].Delete();
                    }
                }
            }

            for(int y = 0; y < data.Rows.Count - 1; y++)
            {
                ColorDGVRow(y, Form.DefaultBackColor, dataGridView1);
            }

            if (parent.openedProjectData != null) parent.openedProjectData.isSaved = false;
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

        private void btn_Analyze_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            
            mySummary.Clear();
            runTime = TimeSpan.Zero;
            AnalyzeLogFile();
            CreateSummaryTab();
            CreateDiagramTab();
            Analyzed = true;
            Cursor = Cursors.Default;

            tabControl1.SelectedIndex = 2;
            if (parent.openedProjectData != null) parent.openedProjectData.isSaved = false;
        }

        private void UC_NewLog_Load(object sender, EventArgs e)
        {

        }

        //Color every other line to increase visibility
        private void dataGridView2_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 1)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.AntiqueWhite;
            }
        }

        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (parent.openedProjectData != null) parent.openedProjectData.isSaved = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2 && tabControl1.TabPages.Count < 4 && Analyzed)
                CreateReportTab();

            if (tabControl1.SelectedIndex == 3)
            {
                if (parent.WindowState == FormWindowState.Maximized) parent.WindowState = FormWindowState.Normal;
                parent.Size = new Size(641, 531);
                parent.MaximumSize = parent.Size;
            }
            else
            {
                parent.MaximumSize = new Size(9999, 9999);
            }
        }
        #endregion


    }
}
