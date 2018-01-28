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

namespace Analyze_alarms
{
    public partial class MainForm : Form
    {
        static List<DataTable> myDataTables = new List<DataTable>();
        List<string> openedFiles = new List<string>();
        const int MRUnumber = 6;
        System.Collections.Generic.Queue<string> MRUlist = new Queue<string>();
        const string logSettingsFileName = "\\logsettings.xml";

        public static List<LogSettings> logSettings;



        public MainForm()
        {
            InitializeComponent();
        }


        /// 
        /// All generic functions here
        /// 
        #region Functions


        private void AddNewLogControl(string filePath)
        {
            SaveRecentLogFile(filePath);
            //Add tabpage with date as name
            string fileTabText = GetDateFromString(filePath).ToString();
            fileTabText = CheckIfDuplicateDate(fileTabText);

            TabPage tab = new TabPage();
            tab.Text = fileTabText;
            fileTabControl.TabPages.Add(tab);

            //Converts .csv to a datatable and stores it in datatable list: myDataTables
            ConvertCSVtoDataTable(filePath);

            ////Clear columns that have no valuable information
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(15);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(12);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(11);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(10);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(9);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(8);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(7);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(6);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(5);
            myDataTables[myDataTables.Count - 1].Columns.RemoveAt(1);

            //Remove last row which just have a count
            myDataTables[myDataTables.Count - 1].Rows.RemoveAt(myDataTables[myDataTables.Count - 1].Rows.Count - 1);

            //TODO: Only save an analyzed log to database
            //Store this data in DB
            //DatabaseUtilitys dbutil = new DatabaseUtilitys();
            //dbutil.StoreLogFileInDB(myDataTables[myDataTables.Count - 1]);

            //Create new user control for this file
            fileTabControl.TabPages[fileTabControl.TabCount - 1].Controls.Add(CreateNewLog(myDataTables[myDataTables.Count - 1]));

            ////Add TabControl with tabs: Data, Summary, Diagram
            //tab.Controls.Add(new TabControl());
            //TabControl tabCntrl = (TabControl)tab.Controls[0];
            //tabCntrl.Dock = DockStyle.Fill;
            //tabCntrl.TabPages.Add(new TabPage());
            //tabCntrl.TabPages.Add(new TabPage());
            //tabCntrl.TabPages.Add(new TabPage());

            //var tp_data = (TabPage)tabCntrl.TabPages[0];
            //tp_data.Text = "Data";
            //DataGridView dgv = new DataGridView();
            //dgv.ReadOnly = true;
            //dgv.Height = tabCntrl.TabPages[0].Height - 30;
            //dgv.Dock = DockStyle.Bottom;
            //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //DataTable dt = GetData(x);

            //dgv.DataSource = dt;
            //tp_data.Controls.Add(dgv);

            //TabPage tp_summary = (TabPage)tabCntrl.TabPages[1];
            //tp_summary.Text = "Summary";

            //TabPage tp_diagram = (TabPage)tabCntrl.TabPages[2];
            //tp_diagram.Text = "Diagram";
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
                    foreach (string header in headers)
                    {
                        oneHeader = header.Trim('"');
                        dt.Columns.Add(oneHeader);

                        switch (x)
                        {
                            case 0:
                                dt.Columns[x].DataType = typeof(double);
                                break;

                            case 2:
                                dt.Columns[x].DataType = typeof(short);
                                break;

                            case 3:
                                dt.Columns[x].DataType = typeof(short);
                                break;

                            case 4:
                                dt.Columns[x].DataType = typeof(short);
                                break;

                            case 13:
                                dt.Columns[x].DataType = typeof(DateTime);
                                break;
                        }

                        x++;
                    }

                    dt.Columns.Add("fileName");
                    //dt.Columns["fileName"].DataType = typeof(String);

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(';');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            rows[i] = rows[i].Trim('"');
                            if (rows[i] != "")
                            {
                                switch (i)
                                {
                                    case 0:
                                        double templong;
                                        if (double.TryParse(rows[i], out templong))
                                        {
                                            dr[i] = templong;
                                        }
                                        break;

                                    case 2:
                                        dr[i] = short.Parse(rows[i]);
                                        break;

                                    case 3:
                                        dr[i] = short.Parse(rows[i]);
                                        break;

                                    case 4:
                                        dr[i] = short.Parse(rows[i]);
                                        break;

                                    case 13:
                                        string temp = rows[i].Replace('.', '-');
                                        dr[i] = DateTime.ParseExact(temp, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                        break;

                                    default:
                                        dr[i] = rows[i];
                                        break;
                                }
                            }
                        }
                        dr[16] = strFilePath;
                        dt.Rows.Add(dr);

                    }

                }
                myDataTables.Add(dt);
            }
        }

        private void SaveRecentLogFile(string path)
        {
            //clear all recent list from menu

            LoadRecentList("\\Data\\RecentLogs.txt"); //load list from file
            if (!(MRUlist.Contains(path))) //prevent duplication on recent list
                MRUlist.Enqueue(path); //insert given path into list
                                       //keep list number not exceeded the given value
            while (MRUlist.Count > MRUnumber)
            {
                MRUlist.Dequeue();
            }

            AddRecentMenuItems();

            StreamWriter stringToWrite =
            new StreamWriter(System.Environment.CurrentDirectory + "\\Data\\RecentLogs.txt");
            foreach (string item in MRUlist)
            {
                stringToWrite.WriteLine(item); //write list to stream
            }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }

        private void AddRecentMenuItems()
        {
            recentLogsToolStripMenuItem.DropDownItems.Clear();
            foreach (string item in MRUlist)
            {
                //create new menu for each item in list
                ToolStripMenuItem fileRecent = new ToolStripMenuItem
                             (item, null, RecentLogs_click);
                //add the menu to "recent" menu
                //fileRecent.Text = GetDateFromString(item);
                recentLogsToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
        }

        private void LoadRecentList(string pathEnd)
        {//try to load file. If file isn't found, do nothing
            MRUlist.Clear();
            try
            {
                //read file stream
                StreamReader listToRead =
              new StreamReader(System.Environment.CurrentDirectory + pathEnd);
                string line;
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                    MRUlist.Enqueue(line); //insert to list
                listToRead.Close(); //close the stream

                AddRecentMenuItems();
            }
            catch (Exception) { }
        }

        static void CreateStandardSettingsXML()
        {
            // TODO: Create a functioning example file


            //Creates pretty much this
            //<classes>
            //    <class className=""Test"" classNr=""1"" classType=""1"" messageNr=""123"" subClassMember=""0"" prodActive=""0"" shiftActive=""1""/>
            //    <class className=""Test"" classNr=""1"" classType=""1"" messageNr=""123"" subClassMember=""0"" prodActive=""0"" shiftActive=""1""/>
            //</classes>";

            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                            new XElement("classes",
                                new XElement("class", new XAttribute("className", "Test"),
                                                      new XAttribute("classNr", "1"),
                                                      new XAttribute("classType", "1"),
                                                      new XAttribute("messageNr", "123"),
                                                      new XAttribute("subClassMember", "0"),
                                                      new XAttribute("prodActive", "0"),
                                                      new XAttribute("shiftActive", "1")),
                                new XElement("class", new XAttribute("className", "Test"),
                                                      new XAttribute("classNr", "1"),
                                                      new XAttribute("classType", "1"),
                                                      new XAttribute("messageNr", "123"),
                                                      new XAttribute("subClassMember", "0"),
                                                      new XAttribute("prodActive", "0"),
                                                      new XAttribute("shiftActive", "1"))));



            doc.Save(System.Environment.CurrentDirectory + logSettingsFileName);

        }

        static void UpdateSettingsXML()
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

            doc.Save(System.Environment.CurrentDirectory + logSettingsFileName);


        }

        private void LoadLogSettingsFromFile()
        {
            if (!File.Exists(System.Environment.CurrentDirectory + logSettingsFileName)) CreateStandardSettingsXML();
            else
            {
                try
                {
                    logSettings = new List<LogSettings>();
                    foreach (XElement item in XElement.Load(System.Environment.CurrentDirectory + logSettingsFileName).Elements("class"))
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

        private UC_NewLog CreateNewLog(DataTable data)
        {
            var uc = new UC_NewLog(data);
            uc.AnalyzeButtonClick += new EventHandler(UC_NewLog_Filter_Button_Click);
            uc.Dock = DockStyle.Fill;
            //uc.data = data;

            return uc;
        }

        protected void UC_NewLog_Filter_Button_Click(object sender, EventArgs e)
        {
            //string test = "";
            //var uc = (UC_NewLog)sender;
            //foreach (DataRow dr in uc.data.Rows)
            //{
            //    test = dr[0].ToString();
            //    break;
            //}
            //MessageBox.Show(test);
            //test = "HEJSAN";
            //foreach (DataRow dr1 in uc.data.Rows)
            //{
            //    dr1[0] = test;
            //    break;
            //}
            //DataRow dr2;
            //dr2 = uc.data.Rows[0];
            //test = dr2[0].ToString();
            //MessageBox.Show(test);
            //uc.UpdateDataGridView(uc.data);
            var uc = (UC_NewLog)sender;


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

                        openedFiles.AddRange(openFileDialog1.FileNames);
                        PrepareWindowForNewFiles(openFileDialog1.FileNames, "");
                    }
                }
            }
            else
            {
                if (CheckIfLogIsDuplicate(filePath, null)) return;

                openedFiles.Add(filePath);
                PrepareWindowForNewFiles(null, filePath);
            }


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
        #endregion

        /// 
        /// All events here
        /// 
        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {

            LoadRecentList("\\Data\\RecentLogs.txt");
            LoadLogSettingsFromFile();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileTabControl.TabPages.Clear();
            myDataTables.Clear();
            openedFiles.Clear();


        }

        private void openLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLogFile();
        }

        private void RecentLogs_click(object sender, EventArgs e)
        {
            OpenLogFile(sender.ToString());
        }
        #endregion

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

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
