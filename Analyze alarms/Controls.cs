using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Analyze_alarms
{
    class Controls
    {
        public TabControl tabCntrl { get; set; }
        public DateTimePicker DTP { get; set; }
        public TabPage tabPageData { get; set; }
        public TabPage tabPageSummary { get; set; }
        public TabPage tabPageDiagram { get; set; }
    }
}
