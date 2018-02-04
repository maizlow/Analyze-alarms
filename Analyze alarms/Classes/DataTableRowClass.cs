using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class DataTableRowClass
    {
        public Int32 Id { get; set; }
        public double Time_Ms { get; set; }
        public short StateAfter { get; set; }
        public short MsgClass { get; set; }
        public short MsgNumber { get; set; }
        public DateTime TimeString { get; set; }
        public string MsgText { get; set; }

        public DataTableRowClass() { }
        
        public DataTableRowClass(double Time_Ms, short StateAfter, short MsgClass, short MsgNumber, DateTime TimeString, string MsgText)
        {
            this.Time_Ms = Time_Ms;
            this.StateAfter = StateAfter;
            this.MsgClass = MsgClass;
            this.MsgNumber = MsgNumber;
            this.TimeString = TimeString;
            this.MsgText = MsgText;
        }

    }

}
