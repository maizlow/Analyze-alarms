﻿using System;
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
        4 Indirect subclass 1
        5 Indirect subclass 2
        6 Indirect subclass 3
        7 Indirect subclass 4
        8 Indirect subclass 5
        9 Indirect subclass 6
        10 Indirect subclass 7
        11 Indirect subclass 8
        */
    }



}