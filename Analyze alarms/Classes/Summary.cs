using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class Summary
    {
        public int MsgNumber { get; set; }
        public string MsgText { get; set; }
        public int Amount { get; set; }
        public TimeSpan stopDuration { get; set; }
    }
}
