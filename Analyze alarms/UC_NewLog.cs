using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Analyze_alarms
{
    public partial class UC_NewLog : UserControl
    {
        public DataTable data;

        public UC_NewLog(DataTable data)
        {
            InitializeComponent();
            this.data = data;
            InitDataTable(data);

        }

        private void InitDataTable(DataTable data)
        {
         
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.DataSource = data;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public void UpdateDataGridView(DataTable data)
        {
            InitDataTable(data);
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

        public event EventHandler FilterButtonClick;
        public event EventHandler dtp_From_ValueChanged;
        public event EventHandler dtp_To_ValueChanged;

        protected void btn_Filter_Click(object sender, EventArgs e)
        {
            //bubble the event up to the parent
            if (this.FilterButtonClick != null)
                this.FilterButtonClick(this, e);
        }

        protected void dTP_From_ValueChanged(object sender, EventArgs e)
        {
            //bubble the event up to the parent
            this.dtp_From_ValueChanged(this, e);
        }

        protected void dTP_To_ValueChanged(object sender, EventArgs e)
        {
            //bubble the event up to the parent
            this.dtp_To_ValueChanged(this, e);
        }
    }
}
