using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Analyze_alarms.Classes
{
    public class AttachmentImages
    {
        public Image img { get; set; }
        /// <summary>
        /// false = Horizontal, true = Verical
        /// </summary>
        public bool orientation { get; set; }
    }
}
