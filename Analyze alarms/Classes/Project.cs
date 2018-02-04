using System.Collections.Generic;
using System.ComponentModel;

namespace Analyze_alarms.Classes
{
    public class Project : INotifyPropertyChanged
    {
        private bool IsSaved;

        public string ProjectName { get; set; }
        public int UserControlCount { get; set; }
        public List<string> UserControlNames { get; set; }
        public string FilePath { get; set; }
        public bool isSaved
        {
            get
            {
                return IsSaved;
            }
            set
            {
                if (value != IsSaved)
                {
                    IsSaved = value;
                    OnPropertyChanged("IsSaved");
                }
            }
        }
        public Project()
        {
            UserControlNames = new List<string>();
        }
        public Project(string ProjectName, int UserControlCount, List<string> UserControlNames, string FilePath)
        {
            this.ProjectName = ProjectName;
            this.UserControlCount = UserControlCount;
            this.UserControlNames = UserControlNames;
            this.FilePath = FilePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(string isSaved)
        {
            if (PropertyChanged == null)
                return;

            OnPropertyChanged(new PropertyChangedEventArgs("isSaved"));

        }

    }
}
