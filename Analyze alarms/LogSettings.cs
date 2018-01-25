using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms
{
    public class LogSettings
    {
        public string className { get; set; }
        public int classNr { get; set; }
        public int classType { get; set; }
        public int messageNr { get; set; }
        public int subClassMember { get; set; } //classNr of member class
        public bool isProdActiveLogBit { get; set; }
        public bool isShiftActiveLogBit { get; set; }

        /*
        Class types are as following:
        1 Logging               {req. messageNr}
        2 Direct
        3 Indirect              {req. messageNr, subClassMember}
        4 Indirect subclass
        */
    }



}
