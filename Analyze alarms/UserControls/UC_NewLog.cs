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

namespace Analyze_alarms
{
    public partial class UC_NewLog : UserControl
    {
        MainForm parent;
        private DataTable data;
        TimeSpan runTime = new TimeSpan();
        //TODO: This data should be stored for future in DB
        //Summary data for summary tabpage
        public List<Classes.Summary> mySummary = new List<Classes.Summary>();
        private int nrOfSummaryEntrys;
        private TabPage tp_Report;
        private SaveFileDialog saveDialog;
        private Classes.Charts myCharts = new Classes.Charts();
        public Control rowChart, pieChart;
        Form paintForm;
        Classes.ReportGenerator generator;
        private bool tb_Header_Edited, tb_ReportFrom_Edited, tb_ReportBy_Edited, tb_FreeText_Edited;
        public string tb_Header_Text = "No text.", tb_ReportFrom_Text = "No text.", tb_ReportBy_Text = "No text.", tb_FreeText_Text = "";
        public DateTime dtp_ReportDate = DateTime.Now;
        public bool chk_RowChart_Checked, chk_PieChart_Checked, chk_Summary_Checked;
        public string saveReportFilePath;
        private bool paintFormCompleted;
        public bool PaintFormCompleted
        {
            set
            {
                paintFormCompleted = value;
                paintChartFormDone(ref generator);
            }

        }

        //First dimension = ClassNr, Second dimension = MessageNr
        private int[] ProdRunning_LB = { 0, 0 };
        private int[] ShiftActive_LB = { 0, 0 };

        //Constructor
        public UC_NewLog(MainForm parent, DataTable data)
        {
            InitializeComponent();

            this.Parent = parent;
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
                c.BringToFront();
                tp_Diagram.Controls.Add(c);
            }
            //Pie chart
            if (!tp_Diagram.Controls.ContainsKey("pieControl"))
            {
                Control c = myCharts.CreatePieChart(mySummary);
                c.BringToFront();
                tp_Diagram.Controls.Add(c);
            }

            var btn_Row = new Button();
            btn_Row.Name = "btn_Row";
            btn_Row.Click += new EventHandler(OnBarBtnClick);
            btn_Row.Size = new Size(37, 34);
            var image = new Bitmap(System.Environment.CurrentDirectory + "\\RowChart.png");
            btn_Row.Image = new Bitmap(image, 25, 25);
            btn_Row.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Row.Location = new Point(3, 3);

            var btn_Pie = new Button();
            btn_Pie.Name = "btn_Pie";
            btn_Pie.Click += new EventHandler(OnPieBtnClick);
            btn_Pie.Size = new Size(37, 34);
            var image2 = new Bitmap(System.Environment.CurrentDirectory + "\\PieChart.png");
            btn_Pie.Image = new Bitmap(image2, 25, 25);
            btn_Pie.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn_Pie.Location = new Point(btn_Row.Location.X + btn_Row.Width + 3, 3);

            tp_Diagram.Controls.Add(btn_Row);
            tp_Diagram.Controls.Add(btn_Pie);

            tp_Diagram.Controls[tp_Diagram.Controls.IndexOfKey("rowControl")].BringToFront();
            btn_Row.BringToFront();
            btn_Pie.BringToFront();
            label4.Visible = false;
            pictureBox1.Visible = false;

        }

        private void CreateReportTab()
        {
            Size controlSize = new Size(244, 20);
            Font fontStyleBold = new Font(Label.DefaultFont, FontStyle.Bold);

            //Tab page
            tp_Report = new TabPage();
            tp_Report.Text = "Report";
            tabControl1.TabPages.Add(tp_Report);

            //Header label
            var lbl_Header = new Label();
            lbl_Header.Text = "Report header";
            lbl_Header.Font = fontStyleBold;
            lbl_Header.AutoSize = false;
            lbl_Header.Size = controlSize;
            lbl_Header.Location = new Point(10, 10);
            tp_Report.Controls.Add(lbl_Header);

            //Header textbox
            var tb_ReportHeader = new TextBox();
            tb_ReportHeader.Enter += new EventHandler(tb_ReportHeader_Enter);
            tb_ReportHeader.Leave += new EventHandler(tb_ReportHeader_Leave);
            tb_ReportHeader.Text = "Report header...";
            tb_ReportHeader.MaxLength = 35;
            tb_ReportHeader.Size = controlSize;
            tb_ReportHeader.Location = new Point(lbl_Header.Location.X, lbl_Header.Location.Y + lbl_Header.Height);
            tp_Report.Controls.Add(tb_ReportHeader);

            //Date label
            var lbl_Date = new Label();
            lbl_Date.Text = "Report date";
            lbl_Date.Font = fontStyleBold;
            lbl_Date.AutoSize = false;
            lbl_Date.Size = controlSize;
            lbl_Date.Location = new Point(tb_ReportHeader.Location.X, tb_ReportHeader.Location.Y + tb_ReportHeader.Height + 15);
            tp_Report.Controls.Add(lbl_Date);

            //Date picker
            var dtp_Report = new DateTimePicker();
            dtp_Report.ValueChanged += new EventHandler(dtp_Report_ValueChanged);
            dtp_Report.Format = DateTimePickerFormat.Short;
            dtp_Report.Value = DateTime.Now;
            dtp_Report.Size = controlSize;
            dtp_Report.Location = new Point(lbl_Date.Location.X, lbl_Date.Location.Y + lbl_Date.Height);
            tp_Report.Controls.Add(dtp_Report);

            //Report from plant/line label
            var lbl_ReportFrom = new Label();
            lbl_ReportFrom.Text = "Report from";
            lbl_ReportFrom.Font = fontStyleBold;
            lbl_ReportFrom.AutoSize = false;
            lbl_ReportFrom.Size = controlSize;
            lbl_ReportFrom.Location = new Point(dtp_Report.Location.X, dtp_Report.Location.Y + dtp_Report.Height + 15);
            tp_Report.Controls.Add(lbl_ReportFrom);

            //Report from plant/line textbox
            var tb_ReportFrom = new TextBox();
            tb_ReportFrom.Enter += new EventHandler(tb_ReportFrom_Enter);
            tb_ReportFrom.Leave += new EventHandler(tb_ReportFrom_Leave);
            tb_ReportFrom.Text = "Report from...";
            tb_ReportFrom.MaxLength = 35;
            tb_ReportFrom.Size = controlSize;
            tb_ReportFrom.Location = new Point(lbl_ReportFrom.Location.X, lbl_ReportFrom.Location.Y + lbl_ReportFrom.Height);
            tp_Report.Controls.Add(tb_ReportFrom);

            //ReportBy label
            var lbl_ReportBy = new Label();
            lbl_ReportBy.Text = "Report done by";
            lbl_ReportBy.Font = fontStyleBold;
            lbl_ReportBy.AutoSize = false;
            lbl_ReportBy.Size = controlSize;
            lbl_ReportBy.Location = new Point(tb_ReportFrom.Location.X, tb_ReportFrom.Location.Y + tb_ReportFrom.Height + 15);
            tp_Report.Controls.Add(lbl_ReportBy);

            //ReportBy textbox
            var tb_ReportBy = new TextBox();
            tb_ReportBy.Enter += new EventHandler(tb_ReportBy_Enter);
            tb_ReportBy.Leave += new EventHandler(tb_ReportBy_Leave);
            tb_ReportBy.Text = "Report done by...";
            tb_ReportBy.Size = controlSize;
            tb_ReportBy.Location = new Point(lbl_ReportBy.Location.X, lbl_ReportBy.Location.Y + lbl_ReportBy.Height);
            tp_Report.Controls.Add(tb_ReportBy);

            //Row chart label
            var lbl_RowChart = new Label();
            lbl_RowChart.Text = "Row chart";
            lbl_RowChart.Font = fontStyleBold;
            lbl_RowChart.AutoSize = true;
            lbl_RowChart.Location = new Point(lbl_Header.Location.X + lbl_Header.Width + 30, lbl_Header.Location.Y);
            tp_Report.Controls.Add(lbl_RowChart);

            //Charts checkbox row chart
            var chk_RowChart = new CheckBox();
            chk_RowChart.CheckStateChanged += new EventHandler(chk_RowChart_CheckStateChanged);
            chk_RowChart.Text = "Add Row Chart";
            chk_RowChart.Checked = true;
            chk_RowChart.AutoSize = true;            
            chk_RowChart.Location = new Point(lbl_RowChart.Location.X, lbl_RowChart.Location.Y + lbl_RowChart.Height + 8);
            tp_Report.Controls.Add(chk_RowChart);

            //Pie chart label
            var lbl_PieChart = new Label();
            lbl_PieChart.Text = "Pie chart";
            lbl_PieChart.Font = fontStyleBold;
            lbl_PieChart.AutoSize = true;
            lbl_PieChart.Location = new Point(chk_RowChart.Location.X + chk_RowChart.Width + 5, lbl_RowChart.Location.Y);
            tp_Report.Controls.Add(lbl_PieChart);

            //Charts checkbox pie chart
            var chk_PieChart = new CheckBox();
            chk_PieChart.CheckStateChanged += new EventHandler(chk_PieChart_CheckStateChanged);
            chk_PieChart.Text = "Add Pie Chart";
            chk_PieChart.Checked = true;
            chk_PieChart.AutoSize = true;
            chk_PieChart.Location = new Point(chk_RowChart.Location.X + chk_RowChart.Width + 5, chk_RowChart.Location.Y);
            tp_Report.Controls.Add(chk_PieChart);

            //Summary label
            var lbl_Summary = new Label();
            lbl_Summary.Text = "Summary";
            lbl_Summary.Font = fontStyleBold;
            lbl_Summary.AutoSize = true;
            lbl_Summary.Location = new Point(chk_PieChart.Location.X + chk_PieChart.Width + 5, lbl_RowChart.Location.Y);
            tp_Report.Controls.Add(lbl_Summary);

            //Summary checkbox
            var chk_Summary = new CheckBox();
            chk_Summary.CheckStateChanged += new EventHandler(chk_Summary_CheckStateChanged);
            chk_Summary.Text = "Add summary";
            chk_Summary.Checked = true;
            chk_Summary.AutoSize = true;
            chk_Summary.Location = new Point(lbl_Summary.Location.X, chk_PieChart.Location.Y);
            tp_Report.Controls.Add(chk_Summary);

            //Freetext label
            var lbl_FreeText = new Label();
            lbl_FreeText.Text = "Comments";
            lbl_FreeText.Font = fontStyleBold;
            lbl_FreeText.AutoSize = true;
            lbl_FreeText.Location = new Point(chk_RowChart.Location.X, lbl_Date.Location.Y);
            tp_Report.Controls.Add(lbl_FreeText);

            //Freetext textbox
            var tb_Freetext = new TextBox();
            tb_Freetext.Enter += new EventHandler(tb_Freetext_Enter);
            tb_Freetext.Leave += new EventHandler(tb_Freetext_Leave);
            tb_Freetext.Text = "";
            tb_Freetext.Multiline = true;
            tb_Freetext.Location = new Point(lbl_FreeText.Location.X, dtp_Report.Location.Y);
            tb_Freetext.Size = new Size(chk_Summary.Location.X + chk_Summary.Width - chk_RowChart.Location.X, tb_ReportBy.Location.Y + tb_ReportBy.Height - tb_Freetext.Location.Y);
            tp_Report.Controls.Add(tb_Freetext);

            //Generate report button
            var btn_GenerateReport = new Button();
            btn_GenerateReport.Click += new EventHandler(btn_GenerateReport_Click);
            btn_GenerateReport.Text = "Generate report";
            btn_GenerateReport.Font = new Font(btn_GenerateReport.Font.FontFamily, 18.0f);
            btn_GenerateReport.Size = new Size(chk_Summary.Location.X + chk_Summary.Width - tb_ReportBy.Location.X, 150);
            btn_GenerateReport.Location = new Point(tb_ReportBy.Location.X, tb_ReportBy.Location.Y + tb_ReportBy.Height + 15);
            tp_Report.Controls.Add(btn_GenerateReport);

            //Generate save dialog    
            saveDialog = new SaveFileDialog();
            saveDialog.FileOk += new CancelEventHandler(saveDialog_FileOk);
            saveDialog.DefaultExt = ".pdf";
            saveDialog.Filter = "Portable Document Format (.pdf)|*.pdf";
            saveDialog.RestoreDirectory = true;

        }

        private void saveDialog_FileOk(object sender, CancelEventArgs e)
        {
            SaveFileDialog s = (SaveFileDialog)sender;
            saveReportFilePath = s.FileName;

            StartCreatePDFReport();
        }

        private void StartCreatePDFReport()
        {
            generator = new Classes.ReportGenerator(this);

            Cursor = Cursors.WaitCursor;
            if (chk_PieChart_Checked || chk_RowChart_Checked)
            {
                paintForm = new Forms.PaintCharts(this, mySummary);
                paintForm.Show();
                //After the paintForm is showned it will draw charts and then set UC_NewLog property PaintFormCompleted,
                //this will in turn call function paintChartFormDone();
            }
            else
            {
                GenerateReport();
            }            
        }

        private void paintChartFormDone(ref Classes.ReportGenerator generator)
        {           

            if (rowChart != null)
                generator.rowChart = (Image)BitmapFromChart(rowChart);

            if (pieChart != null)
                generator.pieChart = (Image)BitmapFromChart(pieChart);

            paintForm.Close();
            paintForm.Dispose();

            GenerateReport();
        }

        private void GenerateReport()
        {
            var filepath = generator.Generate(saveReportFilePath, false);

            Process.Start(filepath);
            Cursor = Cursors.Default;
        }

        #endregion

        #region EVENTS

        private void tb_ReportFrom_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!tb_ReportFrom_Edited)
                thisTB.Text = "";
        }

        private void tb_ReportFrom_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            tb_ReportFrom_Text = thisTB.Text;
            tb_ReportFrom_Edited = true;
        }

        private void tb_ReportHeader_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!tb_Header_Edited)
                thisTB.Text = "";
        }

        private void tb_ReportHeader_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            tb_Header_Text = thisTB.Text;
            tb_Header_Edited = true;
        }

        private void dtp_Report_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;
            dtp_ReportDate = dtp.Value;
        }

        private void tb_ReportBy_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!tb_ReportBy_Edited)
                thisTB.Text = "";            
        }

        private void tb_ReportBy_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            tb_ReportBy_Text = thisTB.Text;
            tb_ReportBy_Edited = true;
        }

        private void chk_PieChart_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox thisChk = (CheckBox)sender;
            if (thisChk.Checked)
                chk_PieChart_Checked = true;
            else
                chk_PieChart_Checked = false;
        }

        private void chk_RowChart_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox thisChk = (CheckBox)sender;
            if (thisChk.Checked)
                chk_RowChart_Checked = true;
            else
                chk_RowChart_Checked = false;
        }

        private void chk_Summary_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox thisChk = (CheckBox)sender;
            if (thisChk.Checked)
                chk_Summary_Checked = true;
            else
                chk_Summary_Checked = false;
        }

        private void tb_Freetext_Enter(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            if (!tb_FreeText_Edited)
                thisTB.Text = "";
        }

        private void tb_Freetext_Leave(object sender, EventArgs e)
        {
            TextBox thisTB = (TextBox)sender;
            tb_FreeText_Text = thisTB.Text;
            tb_FreeText_Edited = true;
        }

        private void btn_GenerateReport_Click(object sender, EventArgs e)
        {
            saveDialog.ShowDialog();
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
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
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
            Cursor = Cursors.WaitCursor;

            mySummary.Clear();
            runTime = TimeSpan.Zero;
            AnalyzeLogFile();
            CreateSummaryTab();
            CreateDiagramTab();
            
            Cursor = Cursors.Default;

            tabControl1.SelectedIndex = 2;
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2 && tabControl1.TabPages.Count < 4)
                CreateReportTab();
        }
        #endregion


    }
}
