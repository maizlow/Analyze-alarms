using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class AlarmInterval
    {
        public Int32 Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan Duration { get; set; }
        public string AlarmText { get; set; }

        public AlarmInterval() { }
        public AlarmInterval(DateTime TimeStamp, TimeSpan Duration, string AlarmText)
        {
            this.TimeStamp = TimeStamp;
            this.Duration = Duration;
            this.AlarmText = AlarmText;
        }

    }
}
