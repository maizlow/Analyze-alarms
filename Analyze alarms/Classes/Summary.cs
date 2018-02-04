using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class Summary
    {
        public Int32 Id { get; set; }
        public int MsgNumber { get; set; }
        public string MsgText { get; set; }
        public int Amount { get; set; }
        public TimeSpan stopDuration { get; set; }
    }
}
