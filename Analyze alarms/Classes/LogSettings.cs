using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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

        //TODO: Could add other settings such as Color and such

        /*
        Class types are as following:
        1 Logging               {req. messageNr}
        2 Direct
        3 Indirect              {req. messageNr, subClassMember}
        4 Indirect subclass
        */
    }



}
