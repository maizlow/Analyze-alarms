using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Analyze_alarms
{
    public partial class HELP_LogSettings : Form
    {
        public HELP_LogSettings()
        {
            InitializeComponent();
        }

        private void HELP_LogSettings_Load(object sender, EventArgs e)
        {
            this.Icon = new Icon(System.Environment.CurrentDirectory + "\\logo.ico");
        }
    }
}
