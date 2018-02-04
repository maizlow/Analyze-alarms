using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyze_alarms.Classes
{
    public class Project
    {
        public string ProjectName { get; set; }
        public int UserControlCount { get; set; }
        public List<string> UserControlNames { get; set; }

        public Project()
        {
            UserControlNames = new List<string>();
        }
        public Project(string ProjectName, int UserControlCount, List<string> UserControlNames)
        {
            this.ProjectName = ProjectName;
            this.UserControlCount = UserControlCount;
            this.UserControlNames = UserControlNames;
        }
    }
}
