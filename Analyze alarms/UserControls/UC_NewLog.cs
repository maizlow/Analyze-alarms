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

        public void UpdateDataGridView(DataTable data)
        {
           // InitDataTable(data);
        }

        private void FindLogBits()
        { 
            foreach(DataRow dr in data.Rows)
            {
                //Logbit production running: classnr 64 msgnr 128
                if (Convert.ToInt16(dr[2]) == 64 && Convert.ToInt16(dr[3]) == 128)
                {
                    if (Convert.ToBoolean(dr[1]))
                    {
                        dataGridView1.Rows[data.Rows.IndexOf(dr)].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        dataGridView1.Rows[data.Rows.IndexOf(dr)].DefaultCellStyle.BackColor = Color.OrangeRed;
                    }
                }
            }

            UpdateDataGridView(data);
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



        }

        private void UC_NewLog_Load(object sender, EventArgs e)
        {
            FindLogBits();
        }
    }
}
