using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class AnalyzedRows
    {        
        public Int32 Id { get; set; }
        public int rowNr { get; set; }
        public int color { get; set; }

        public AnalyzedRows()
        { }

        public AnalyzedRows(int rowNr, Color color)
        {
            this.rowNr = rowNr;
            this.color = color.ToArgb();
        }
    }

}
