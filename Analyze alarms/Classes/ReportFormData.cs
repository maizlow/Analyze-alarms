using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class ReportFormData
    {
        public Int32 Id { get; set; } = 0;
        public string tb_Header_Text { get; set; } = "Report header...";
        public string tb_ReportFrom_Text { get; set; } = "Report from...";
        public string tb_ReportBy_Text { get; set; } = "Report by...";
        public string tb_FreeText_Text { get; set; } = "";
        public bool tb_Header_Edited { get; set; } = false;
        public bool tb_ReportFrom_Edited { get; set; } = false;
        public bool tb_ReportBy_Edited { get; set; } = false;
        public bool tb_FreeText_Edited { get; set; } = false;
        public DateTime dtp_ReportDate { get; set; } = DateTime.Now;
        public bool chk_RowChart_Checked { get; set; } = true;
        public bool chk_PieChart_Checked { get; set; } = true;
        public bool chk_Summary_Checked { get; set; } = true;
        public string customLogoPath { get; set; } = String.Empty;
        public string[] attachmentsFilePaths { get; set; } = null;

    }
}
