using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class ReportFormData
    {
        public Int32 Id { get; set; }
        public string tb_Header_Text { get; set; }
        public string tb_ReportFrom_Text { get; set; }
        public string tb_ReportBy_Text { get; set; } 
        public string tb_FreeText_Text { get; set; } 
        public bool tb_Header_Edited { get; set; } 
        public bool tb_ReportFrom_Edited { get; set; }
        public bool tb_ReportBy_Edited { get; set; } 
        public bool tb_FreeText_Edited { get; set; } 
        public DateTime dtp_ReportDate { get; set; }   
        public bool chk_RowChart_Checked { get; set; }
        public bool chk_PieChart_Checked { get; set; }
        public bool chk_Summary_Checked { get; set; } 
        public string customLogoPath { get; set; }
        public string[] attachmentsFilePaths { get; set; } 
        public bool chk_Default_Checked { get; set; }

        public ReportFormData(){}

        public void Init()
        {
            this.tb_Header_Text = "Report header...";
            this.tb_ReportFrom_Text = "Report from...";
            this.tb_ReportBy_Text = "Report by...";
            this.tb_FreeText_Text = "";
            this.dtp_ReportDate = DateTime.Now;
            this.chk_RowChart_Checked = true;
            this.chk_PieChart_Checked = true;
            this.chk_Summary_Checked = true;
            this.customLogoPath = "";
            this.chk_Default_Checked = true;
        }
        

    }
}
