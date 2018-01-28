using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Analyze_alarms
{
    public partial class UC_NewLog : UserControl
    {
        private DataTable data;
        TimeSpan runTime = new TimeSpan();

        //TODO: This data should be stored for future in DB
        //Summary data for summary tabpage
        private List<Classes.Summary> mySummary = new List<Classes.Summary>();

        //First dimension = ClassNr, Second dimension = MessageNr
        private int[] ProdRunning_LB = { 0, 0 };
        private int[] ShiftActive_LB = { 0, 0 };

        public UC_NewLog(DataTable data)
        {
            InitializeComponent();

            this.data = data;
            InitDataTable(data);
            InitDateTimePickers();

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
        private Classes.Summary CheckIfExist(int MsgNr)
        {
            Classes.Summary sum = mySummary.Find(
            delegate (Classes.Summary summ)
            {
                return summ.MsgNumber == MsgNr;
            });

            return sum;
        }

        private void Analyze()
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
                                    Classes.Summary sum = CheckIfExist(StopCauseMsgNumber);
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

        private void CreateOrUpdateSummary()
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

            foreach (Classes.Summary s in mySummary)
            {
                TotalAmountOfStops += s.Amount;
                TotalStopTime += s.stopDuration;
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

        public DateTime dTP_From_Value
        {
            get { return dTP_From.Value; }
            set { dTP_From.Value = dTP_From_Value; }
        }

        public DateTime dTP_To_Value
        {
            get { return dTP_To.Value; }
            set { dTP_To.Value = dTP_To_Value; }
        }

        public bool gb_Modify_Visible
        {
            get { return gb_Modify.Visible; }
            set { gb_Modify.Visible = gb_Modify_Visible; }
        }

        public bool gb_Data_Visible
        {
            get { return gb_Data.Visible; }
            set { gb_Data.Visible = gb_Data_Visible; }
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
            Analyze();
            CreateOrUpdateSummary();

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
    }
}
