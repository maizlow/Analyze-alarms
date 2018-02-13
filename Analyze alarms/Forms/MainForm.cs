using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.Configuration;
using Analyze_alarms.Classes;

namespace Analyze_alarms
{
    public partial class MainForm : Form
    {
        public Project openedProjectData;// = new Project();
        static List<DataTable> myDataTables = new List<DataTable>();
        private List<UC_NewLog> myUCs = new List<UC_NewLog>();
        List<string> openedFiles = new List<string>();
        const int MRUnumber = 6;
        Queue<string> MRLogsList = new Queue<string>();
        Queue<string> MRProjectsList = new Queue<string>();
        const string logSettingsFileName = "\\logsettings.xml";
        private string folderPath;

        public static List<LogSettings> logSettings;

        public MainForm()
        {
            InitializeComponent();            
            this.MinimumSize = new Size(640, 530);
            this.Size = this.MinimumSize;
            toolStripStatusLabel1.Text = "";
        }

        /// 
        /// All generic functions here
        /// 
        #region Functions

        private void AddNewLogControl(string filePath)
        {
            SaveRecentFile(filePath, true, MRLogsList);
            //Add tabpage with date as name
            string fileTabText = Path.GetFileName(filePath.Remove(filePath.Length - 4)).Replace('.', '_').Replace(' ', '_').Replace('-', '_');
            fileTabText = CheckIfDuplicateDate(fileTabText);

            TabPage tab = new TabPage();
            tab.Text = fileTabText;
            fileTabControl.TabPages.Add(tab);

            //Converts .csv to a datatable and stores it in datatable list: myDataTables
            ConvertCSVtoDataTable(filePath);

            ////Clear columns that have no valuable information
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(16);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(13);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(12);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(11);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(10);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(9);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(8);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(7);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(6);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(2);

            //Remove last row which just have a count
            myDataTables[myDataTables.Count - 1].Rows.RemoveAt(myDataTables[myDataTables.Count - 1].Rows.Count - 1);

            //Create new user control for this file
            var uc = CreateNewLogFromCSV(myDataTables[myDataTables.Count - 1]);
            uc.Name = fileTabText;
            myUCs.Add(uc);
            fileTabControl.TabPages[fileTabControl.TabCount - 1].Controls.Add(uc);
        }

        private void PrepareWindowForNewFiles(string[] filePaths = null, string filePath = "")
        {
            //if a single file is openend
            if (filePaths != null)
            {
                foreach (string x in filePaths)
                {
                    AddNewLogControl(x);
                }

            }
            else if (filePath != "")
            {
                AddNewLogControl(filePath);
            }
            else MessageBox.Show("Error: Prepare window for new log /n not a valid file path passed.");
        }

        static String GetDateFromString(string inputText)
        {
            string[] myStrings;
            myStrings = inputText.Split('_');

            var newDate = DateTime.ParseExact(myStrings[1],
                                  "yyyyMMdd",
                                   CultureInfo.InvariantCulture);
            if (newDate != null) return myStrings[1];
            else return null;
        }

        private String CheckIfDuplicateDate(string inputText)
        {
            //Should concat a number if the date is existing.
            return inputText;

        }

        public static void ConvertCSVtoDataTable(string strFilePath)
        {
            if (strFilePath != null)
            {
                DataTable dt = new DataTable();
                using (StreamReader sr = new StreamReader(strFilePath, System.Text.Encoding.Default))
                {
                    string[] headers = sr.ReadLine().Split(';');
                    string oneHeader;
                    int x = 0;
                    dt.Columns.Add("Id");
                    foreach (string header in headers)
                    {
                        oneHeader = header.Trim('"');
                        dt.Columns.Add(oneHeader);

                        switch (x)
                        {
                            case 1:
                                dt.Columns[x].DataType = typeof(double);
                                break;

                            case 3:
                                dt.Columns[x].DataType = typeof(short);
                                break;

                            case 4:
                                dt.Columns[x].DataType = typeof(short);
                                break;

                            case 5:
                                dt.Columns[x].DataType = typeof(short);
                                break;

                            case 14:
                                dt.Columns[x].DataType = typeof(DateTime);
                                break;
                        }

                        x++;
                    }

                    //dt.Columns.Add("fileName");

                    int y = 0;
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(';');
                        DataRow dr = dt.NewRow();

                        for (int i = 0; i < headers.Length; i++)
                        {
                            rows[i] = rows[i].Trim('"');
                            if (rows[i] != "")
                            {
                                dr[0] = y;
                                switch (i)
                                {
                                    case 0:
                                        double templong;
                                        if (double.TryParse(rows[i], out templong))
                                        {
                                            dr[i + 1] = templong;
                                        }
                                        break;

                                    case 2:
                                        dr[i + 1] = short.Parse(rows[i]);
                                        break;

                                    case 3:
                                        dr[i + 1] = short.Parse(rows[i]);
                                        break;

                                    case 4:
                                        dr[i + 1] = short.Parse(rows[i]);
                                        break;

                                    case 13:
                                        string temp = rows[i].Replace('.', '-');
                                        dr[i + 1] = DateTime.ParseExact(temp, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                        break;

                                    default:
                                        dr[i + 1] = rows[i];
                                        break;
                                }
                            }
                        }
                        //dr[17] = strFilePath;
                        dt.Rows.Add(dr);
                        y++;
                    }

                }
                myDataTables.Add(dt);
            }
        }

        private void SaveRecentFile(string path, bool log, Queue<string> mru)
        {
            string PathToRecentFile;
            //clear all recent list from menu
            if (log)
            {
                PathToRecentFile = "\\RecentLogs.txt";
                LoadRecentMRUList(PathToRecentFile, mru, log); //load list from file
            }
            else
            {
                PathToRecentFile = "\\RecentProjects.txt";
                LoadRecentMRUList(PathToRecentFile, mru, log); //load list from file
            }

            if (!(mru.Contains(path)))          //prevent duplication on recent list
                mru.Enqueue(path);              //insert given path into list
                                                //keep list number not exceeded the given value
            while (mru.Count > MRUnumber)
            {
                mru.Dequeue();
            }

            AddRecentMenuItems(mru, log);

            StreamWriter stringToWrite =
            new StreamWriter(folderPath + PathToRecentFile);
            foreach (string item in mru)
            {
                stringToWrite.WriteLine(item); //write list to stream
            }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory

        }

        private void AddRecentMenuItems(Queue<string> mru, bool log)
        {
            if (log) recentLogsToolStripMenuItem.DropDownItems.Clear();
            else recentProjectsToolStripMenuItem.DropDownItems.Clear();

            foreach (string item in mru)
            {
                //create new menu for each item in list
                if (log)
                {
                    ToolStripMenuItem logRecent = new ToolStripMenuItem(item, null, RecentLogs_click);
                    //add the menu to "recent" menu
                    //fileRecent.Text = GetDateFromString(item);
                    recentLogsToolStripMenuItem.DropDownItems.Add(logRecent);
                }
                else
                {
                    ToolStripMenuItem projectRecent = new ToolStripMenuItem(item, null, RecentProjects_click);
                    //add the menu to "recent" menu
                    //fileRecent.Text = GetDateFromString(item);
                    recentProjectsToolStripMenuItem.DropDownItems.Add(projectRecent);
                }
            }
        }

        private void RemoveRecentMenuItem(string filePath, Queue<string> mru, bool log)
        {
            mru = new Queue<string>(mru.Where(s => s != filePath));

            if (log)
            {
                string[] lines = File.ReadAllLines(folderPath + "\\RecentLogs.txt");
                string[] newLines = RemoveLineFromFile(lines, filePath);
                File.WriteAllLines(folderPath + "\\RecentLogs.txt", newLines);
            }
            else
            {
                string[] lines = File.ReadAllLines(folderPath + "\\RecentProjects.txt");
                string[] newLines = RemoveLineFromFile(lines, filePath);
                File.WriteAllLines(folderPath + "\\RecentProjects.txt", newLines);
            }
            AddRecentMenuItems(mru, log);

        }

        private string[] RemoveLineFromFile(string[] lines, string removeThis)
        {
            string[] s = lines;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == removeThis)
                {
                    s[i] = null;
                }
            }
            return s;
        }

        private void LoadRecentMRUList(string pathEnd, Queue<string> mru, bool log)
        {//try to load file. If file isn't found, do nothing
            mru.Clear();
            try
            {
                if (File.Exists(folderPath + pathEnd))
                {
                    //read file stream
                    StreamReader listToRead = new StreamReader(folderPath + pathEnd);

                    string line;

                    while ((line = listToRead.ReadLine()) != null)
                    {
                        if (line == "")
                            continue;
                        else mru.Enqueue(line); //insert to list
                    }
                    listToRead.Close(); //close the stream

                    AddRecentMenuItems(mru, log);
                }
            }
            catch (Exception) { }
        }

        private void CreateStandardSettingsXML()
        {
            // TODO: Create a functioning example file
            try
            {
                XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                                new XElement("classes",
                                    new XElement("class", new XAttribute("className", "Logbits"),
                                                          new XAttribute("classNr", "64"),
                                                          new XAttribute("classType", "1"),
                                                          new XAttribute("messageNr", "128"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "1"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Direct stop Tileline"),
                                                          new XAttribute("classNr", "77"),
                                                          new XAttribute("classType", "2"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Direct stop Racking"),
                                                          new XAttribute("classNr", "69"),
                                                          new XAttribute("classType", "2"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Direct stop Carousel"),
                                                          new XAttribute("classNr", "75"),
                                                          new XAttribute("classType", "2"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Direct stop Packaging"),
                                                          new XAttribute("classNr", "80"),
                                                          new XAttribute("classType", "2"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 1 Tileline"),
                                                          new XAttribute("classNr", "81"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "938"),
                                                          new XAttribute("subClassMember", "73"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 2 Tileline"),
                                                          new XAttribute("classNr", "81"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "939"),
                                                          new XAttribute("subClassMember", "73"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 3 Tileline"),
                                                          new XAttribute("classNr", "81"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "940"),
                                                          new XAttribute("subClassMember", "73"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 1 Racking"),
                                                          new XAttribute("classNr", "70"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "560"),
                                                          new XAttribute("subClassMember", "67"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 2 Racking"),
                                                          new XAttribute("classNr", "70"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "570"),
                                                          new XAttribute("subClassMember", "67"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 1 Curing"),
                                                          new XAttribute("classNr", "70"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "566"),
                                                          new XAttribute("subClassMember", "68"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 1 Carousel"),
                                                          new XAttribute("classNr", "78"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "1767"),
                                                          new XAttribute("subClassMember", "74"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 1 Packaging"),
                                                          new XAttribute("classNr", "79"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "914"),
                                                          new XAttribute("subClassMember", "82"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Indirect stop alarm 2 Packaging"),
                                                          new XAttribute("classNr", "79"),
                                                          new XAttribute("classType", "3"),
                                                          new XAttribute("messageNr", "1652"),
                                                          new XAttribute("subClassMember", "83"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 1 Tileline"),
                                                          new XAttribute("classNr", "73"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 1 Racking"),
                                                          new XAttribute("classNr", "67"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 1 Curing"),
                                                          new XAttribute("classNr", "68"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 1 General RackingCuring"),
                                                          new XAttribute("classNr", "66"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 1 Carousel"),
                                                          new XAttribute("classNr", "74"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 1 Packaging"),
                                                          new XAttribute("classNr", "82"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0")),
                                    new XElement("class", new XAttribute("className", "Subclass 2 Packaging"),
                                                          new XAttribute("classNr", "83"),
                                                          new XAttribute("classType", "4"),
                                                          new XAttribute("messageNr", "0"),
                                                          new XAttribute("subClassMember", "0"),
                                                          new XAttribute("prodActive", "0"),
                                                          new XAttribute("shiftActive", "0"))
                                                          ));

                doc.Save(folderPath + logSettingsFileName);
                LoadLogSettingsFromFile();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateSettingsXML()
        {

            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), new XElement("classes"));

            foreach (LogSettings item in logSettings)
            {
                var newElement = new XElement("class",
                                new XAttribute("className", item.className),
                                new XAttribute("classNr", item.classNr),
                                new XAttribute("classType", item.classType),
                                new XAttribute("messageNr", item.messageNr),
                                new XAttribute("subClassMember", item.subClassMember),
                                new XAttribute("prodActive", item.isProdActiveLogBit),
                                new XAttribute("shiftActive", item.isShiftActiveLogBit)
                                );


                doc.Element("classes").Add(newElement);
            }

            doc.Save(folderPath + logSettingsFileName);


        }

        private void LoadLogSettingsFromFile()
        {
            if (!File.Exists(folderPath + logSettingsFileName)) CreateStandardSettingsXML();
            else
            {
                try
                {
                    logSettings = new List<LogSettings>();
                    foreach (XElement item in XElement.Load(folderPath + logSettingsFileName).Elements("class"))
                    {
                        logSettings.Add(new LogSettings()
                        {
                            className = item.Attribute("className").Value,
                            classNr = int.Parse(item.Attribute("classNr").Value),
                            classType = int.Parse(item.Attribute("classType").Value),
                            messageNr = int.Parse(item.Attribute("messageNr").Value),
                            subClassMember = int.Parse(item.Attribute("subClassMember").Value),
                            isProdActiveLogBit = (bool)item.Attribute("prodActive"),
                            isShiftActiveLogBit = (bool)item.Attribute("shiftActive")
                        });
                    }


                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message); }
            }
        }

        private UC_NewLog CreateNewLogFromCSV(DataTable data)
        {
            var uc = new UC_NewLog(data, this);
            return uc;
        }

        private UC_NewLog CreateNewLogFromOpenedProject(string controlName)
        {
            var uc = new UC_NewLog(controlName, this);
            return uc;
        }

        private void OpenLogFile(string filePath = "")
        {
            if (filePath == "")
            {
                openFileDialog1.Title = "Select files...";
                openFileDialog1.Filter = "Comma separated files (*.csv)|*.csv";
                openFileDialog1.Multiselect = true;
                openFileDialog1.RestoreDirectory = true;
                DialogResult dRes;
                dRes = openFileDialog1.ShowDialog();

                if (dRes == DialogResult.OK)
                {
                    if (openFileDialog1.FileNames != null)
                    {
                        if (CheckIfLogIsDuplicate("", openFileDialog1.FileNames)) return;
                        foreach (string s in openFileDialog1.FileNames)
                        {
                            if (Path.GetFileName(s).Length > 40)
                            {
                                MessageBox.Show("Filename is too long, we only allow a maximum of 40 characters.");
                                return;
                            }
                        }
                        openedFiles.AddRange(openFileDialog1.FileNames);
                        PrepareWindowForNewFiles(openFileDialog1.FileNames, "");
                    }
                }
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File is not there anymore. Removing it from list.");
                    RemoveRecentMenuItem(filePath, MRLogsList, true);
                    return;
                }
                if (CheckIfLogIsDuplicate(filePath, null)) return;
                if (Path.GetFileName(filePath).Length > 40)
                {
                    MessageBox.Show("Filename is too long, we only allow a maximum of 40 characters.");
                    return;
                }
                openedFiles.Add(filePath);
                PrepareWindowForNewFiles(null, filePath);
            }

            saveAsToolStripMenuItem.Enabled = true;
            closeProjectToolStripMenuItem.Enabled = true;
            fileTabControl.SelectedIndex = 1;
        }

        private bool CheckIfLogIsDuplicate(string fileName = "", string[] fileNames = null)
        {
            bool duplicates = false;

            if (fileName != "")
            {
                foreach (string y in openedFiles)
                {
                    if (fileName == y)
                    {
                        duplicates = true;
                    }
                }
            }

            if (fileNames != null)
            {
                foreach (string x in fileNames)
                {
                    foreach (string y in openedFiles)
                    {
                        if (x == y)
                        {
                            duplicates = true;
                        }
                    }
                }
            }

            if (duplicates) MessageBox.Show("You can't add files with the same filename as any existing files!");

            return duplicates;
        }

        private void CreateProjectFile(string filePath, int ucCount, List<string> ucNames)
        {
            string path = filePath;
            string name = Path.GetFileName(path);
            if (path != "")
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(name);
                    sw.WriteLine(ucCount.ToString());
                    foreach (string s in ucNames)
                    {
                        sw.WriteLine(s);
                    }
                }
                openedProjectData = null;
                openedProjectData = new Project(name.Replace('.', '_').Replace(' ', '_'), ucCount, ucNames, path);
                openedProjectData.PropertyChanged += new PropertyChangedEventHandler(isSaved_Valuechanged);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Returns: Projectname [0] , Usercontrol count [0] , Usercontrol names [1->Count-1]</returns>
        private List<string> ReadProjectFile(string filePath)
        {
            using (StreamReader sr = File.OpenText(filePath))
            {
                string s = "";
                List<string> ls = new List<string>();
                while ((s = sr.ReadLine()) != null)
                {
                    ls.Add(s);
                }
                return ls;
            }
        }

        private void SaveToDB(string projectName)
        {
            Classes.DataBase db = new Classes.DataBase();
            foreach (UC_NewLog uc in myUCs)
            {
                db.SaveSummaryData(uc.mySummary, projectName + "_sData_" + uc.Name);
                db.SaveReportData(uc.myReportFormData, projectName + "_rData_" + uc.Name);
                db.SaveDataTable(uc.myDataTableRowsList, projectName + "_dTable_" + uc.Name);
                db.SaveAnalyzedRows(uc.analyzedRows, projectName + "_aRows_" + uc.Name);
            }
        }

        private void ReadFromDB(string projectName)
        {
            Classes.DataBase db = new Classes.DataBase();
            if (myUCs != null && myUCs.Count > 0)
            {
                List<Summary> summary = new List<Summary>();
                ReportFormData reportFormData = new ReportFormData();
                List<DataTableRowClass> dataTableRowsList = new List<DataTableRowClass>();
                List<AnalyzedRows> analyzedRows = new List<AnalyzedRows>();

                foreach (UC_NewLog uc in myUCs)
                {
                    summary = db.LoadSummaryData(projectName + "_sData_" + uc.Name);
                    reportFormData = db.LoadReportData(projectName + "_rData_" + uc.Name);
                    dataTableRowsList = db.LoadDataTablesData(projectName + "_dTable_" + uc.Name);
                    analyzedRows = db.LoadAnalyzedRowsData(projectName + "_aRows_" + uc.Name);

                    //Populate Usercontrols data
                    PopulateUCData(uc, summary, reportFormData, dataTableRowsList, analyzedRows);
                }
            }
        }

        private void SetProjectNameInWindow(string name = "")
        {
            if (name == "" || name == null)
                this.Text = "ABECE Alarm analyzer";
            else this.Text = "ABECE Alarm analyzer - " + name;

        }

        private void SaveProject(bool withDialog, string path = "")
        {
            string savePath = "";
            if (withDialog)
            {
                saveFileDialog1.Filter = "ABECE Project | *.abc;";
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != null && saveFileDialog1.FileName != "")
                        savePath = saveFileDialog1.FileName;
                    else
                    {
                        MessageBox.Show("Not a valid path.");
                        return;
                    }
                }
            }
            else
            {
                if (path != null && path != "")
                    savePath = path;
                else MessageBox.Show("Not a valid path.");
            }

            List<string> slist = new List<string>();
            foreach (UC_NewLog uc in myUCs)
            {
                slist.Add(uc.Name);
                if (uc.myReportFormData == null | uc.myDataTableRowsList == null | uc.mySummary == null)
                {
                    MessageBox.Show("No data to save.");
                    return;
                }
            }

            CreateProjectFile(savePath, myUCs.Count, slist);
            SaveToDB(openedProjectData.ProjectName.Replace('.', '_').Replace(' ', '_'));

            SetProjectNameInWindow(openedProjectData.ProjectName);
            toolStripStatusLabel1.Text = "Project saved.";
            saveToolStripMenuItem.Enabled = true;
            openedProjectData.isSaved = true;
        }

        private void OpenProject(string filePath = "")
        {
            if (openedProjectData == null)
            {
                string filePathToOpen = "";
                //If filePath == "" then use openfiledialog
                if (filePath == "")
                {
                    openFileDialog1.Filter = "ABECE Project | *.abc;";
                    openFileDialog1.Multiselect = false;
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if (openFileDialog1.FileName != null && openFileDialog1.FileName != "")
                        {
                            filePathToOpen = openFileDialog1.FileName;
                        }
                    }
                    else
                    {
                        return;
                    }
                    SaveRecentFile(filePathToOpen, false, MRProjectsList);
                }
                else
                {
                    if (File.Exists(filePath))
                        filePathToOpen = filePath;
                    else
                    {
                        MessageBox.Show("File does not exist any more, removing from list.");
                        RemoveRecentMenuItem(filePath, MRProjectsList, false);
                        return;
                    }
                }

                string pName = "";
                int ucCount = 0;
                List<string> ucNames = new List<string>();

                List<string> s = ReadProjectFile(filePathToOpen);

                pName = s[0].ToString();
                ucCount = int.Parse(s[1].ToString());
                for (int i = 2; i < 2 + ucCount; i++)
                {
                    ucNames.Add(s[i].ToString());
                }


                if (pName != "")
                {
                    //Fill the project data file
                    openedProjectData = new Project(pName, ucCount, ucNames, filePathToOpen);
                    openedProjectData.PropertyChanged += new PropertyChangedEventHandler(isSaved_Valuechanged);
                    //Create new Usercontrols for each name specified in project file
                    InsertNewUCFromSave(openedProjectData);
                    //Get the data from the DB and ReadFromDB will populate the usercontrols
                    ReadFromDB(openedProjectData.ProjectName.Replace('.', '_').Replace(' ', '_'));

                }
                SetProjectNameInWindow(openedProjectData.ProjectName);
                toolStripStatusLabel1.Text = "Project loaded.";
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                closeProjectToolStripMenuItem.Enabled = true;
                fileTabControl.SelectedIndex = 1;
                openedProjectData.isSaved = true;
            }
        }

        private void PopulateUCData(UC_NewLog uc, List<Summary> summary, ReportFormData reportFormData, List<DataTableRowClass> dataRowsList, List<AnalyzedRows> analyzedRows)
        {
            //Will populate controls and init data.
            uc.PopulateDataFromDB(summary, reportFormData, dataRowsList, analyzedRows);
        }

        private void InsertNewUCFromSave(Project project)
        {
            myUCs.Clear();
            foreach (string name in project.UserControlNames)
            {
                myUCs.Add(CreateNewLogFromOpenedProject(name));

                TabPage tab = new TabPage();
                tab.Text = myUCs[myUCs.Count - 1].Name;
                tab.Controls.Add(myUCs[myUCs.Count - 1]);
                fileTabControl.TabPages.Add(tab);
            }
        }
        #endregion

        /// 
        /// All events here
        /// 
        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            folderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            this.Icon = new Icon(folderPath + "\\logo.ico");
            LoadRecentMRUList("\\RecentLogs.txt", MRLogsList, true);
            LoadRecentMRUList("\\RecentProjects.txt", MRProjectsList, false);
            LoadLogSettingsFromFile();
            pictureBox1.Image = Image.FromFile(folderPath + "\\logo.png");
        }

        private void isSaved_Valuechanged(object sender, PropertyChangedEventArgs e)
        {
            if (openedProjectData.isSaved)
                saveToolStripMenuItem.Enabled = false;
            else
                saveToolStripMenuItem.Enabled = true;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLogFile();
        }

        private void RecentLogs_click(object sender, EventArgs e)
        {
            OpenLogFile(sender.ToString());
        }

        private void RecentProjects_click(object sender, EventArgs e)
        {
            OpenProject(sender.ToString());
        }

        private void logsettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings_Form frm = new Settings_Form();
            DialogResult dres;
            dres = frm.ShowDialog();
            if (dres == DialogResult.OK)
            {
                UpdateSettingsXML();
            }

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myUCs.Count == 0)
            {
                MessageBox.Show("Nothing to save yet.");
                return;
            }
            SaveProject(true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openedProjectData != null && openedProjectData.ProjectName != null && openedProjectData.ProjectName != "")
            {
                SaveProject(false, openedProjectData.FilePath);
            }
            else
            {
                MessageBox.Show("Nothing to save yet.");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void toolStripStatusLabel1_TextChanged(object sender, EventArgs e)
        {
            Timer t = new Timer();
            t.Tick += new EventHandler(toolStripStatusTimer_Tick);
            t.Interval = 1500;
            t.Start();
        }

        private void toolStripStatusTimer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            Timer t = (Timer)sender;
            t.Stop();
            t.Dispose();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTabControl.TabPages.Count > 1)
            {
                for (int i = 1; i < fileTabControl.TabPages.Count; i++)
                {
                    fileTabControl.TabPages.RemoveAt(i);
                }
                myDataTables.Clear();
                openedFiles.Clear();
                myUCs.Clear();
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                closeProjectToolStripMenuItem.Enabled = false;
                openedProjectData = null;
                SetProjectNameInWindow();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.About frm = new Forms.About();
            frm.Show();
        }
        #endregion

    }
}
