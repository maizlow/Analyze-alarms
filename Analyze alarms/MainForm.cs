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

namespace Analyze_alarms
{
    public partial class MainForm : Form
    {
        
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


        private void PrepareWindowForNewFiles(String[] fileNames)
        {
            //if a single file is openend
            if (fileNames.Length > 0)
            {
                
                foreach (string x in fileNames)
                {
                    SaveRecentFile(x);
                    //Add tabpage with date as name
                    string fileTabText = GetDateFromString(x).ToString();
                    fileTabText = CheckIfDuplicateDate(fileTabText);

                    TabPage tab = new TabPage();
                    tab.Text = Path.GetFileName(fileTabText);
                    fileTabControl.TabPages.Add(tab);

                    fileTabControl.TabPages[0].Controls.Add(CreateNewLog(GetData(x)));

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
                    ////Clear columns that have no valuable information
                    //dt.Columns.RemoveAt(15);
                    //dt.Columns.RemoveAt(12);
                    //dt.Columns.RemoveAt(11);
                    //dt.Columns.RemoveAt(10);
                    //dt.Columns.RemoveAt(9);
                    //dt.Columns.RemoveAt(8);
                    //dt.Columns.RemoveAt(7);
                    //dt.Columns.RemoveAt(6);
                    //dt.Columns.RemoveAt(5);
                    //dt.Columns.RemoveAt(1);
                    //dgv.DataSource = dt;
                    //tp_data.Controls.Add(dgv);

                    //TabPage tp_summary = (TabPage)tabCntrl.TabPages[1];
                    //tp_summary.Text = "Summary";

                    //TabPage tp_diagram = (TabPage)tabCntrl.TabPages[2];
                    //tp_diagram.Text = "Diagram";




                }


            }
            }

        private DataTable GetData(string fileName)
        {
            if (fileName != null) return ConvertCSVtoDataTable(fileName);
            else return null;
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

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath, System.Text.Encoding.Default))
            {
                string[] headers = sr.ReadLine().Split(';');
                string oneHeader;
                foreach (string header in headers)
                {
                    oneHeader = header.Trim('"');
                    dt.Columns.Add(oneHeader);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(';');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        rows[i] = rows[i].Trim('"');
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            return dt;
        }

        private void SaveRecentFile(string path)
        {
            //clear all recent list from menu
            recentProjectsToolStripMenuItem.DropDownItems.Clear();
            LoadRecentList(); //load list from file
            if (!(MRUlist.Contains(path))) //prevent duplication on recent list
                MRUlist.Enqueue(path); //insert given path into list
                                       //keep list number not exceeded the given value
            while (MRUlist.Count > MRUnumber)
            {
                MRUlist.Dequeue();
            }
            foreach (string item in MRUlist)
            {
                //create new menu for each item in list
                ToolStripMenuItem fileRecent = new ToolStripMenuItem
                             (item, null, RecentProject_click);
                //add the menu to "recent" menu
                recentProjectsToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
            //writing menu list to file
            //create file called "Recent.txt" located on app folder
            StreamWriter stringToWrite =
            new StreamWriter(System.Environment.CurrentDirectory + "\\RecentProjects.txt");
            foreach (string item in MRUlist)
            {
                stringToWrite.WriteLine(item); //write list to stream
            }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }

        private void LoadRecentList()
        {//try to load file. If file isn't found, do nothing
            MRUlist.Clear();
            try
            {
                //read file stream
                StreamReader listToRead =
              new StreamReader(System.Environment.CurrentDirectory + "\\RecentProjects.txt");
                string line;
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                    MRUlist.Enqueue(line); //insert to list
                listToRead.Close(); //close the stream
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

        private UC_NewLog CreateNewLog(DataTable data)
        {
            var uc = new UC_NewLog(data);
            uc.FilterButtonClick += new EventHandler(UC_NewLog_Filter_Button_Click);
            uc.Dock = DockStyle.Fill;
            //uc.data = data;

            return uc;
        }

        protected void UC_NewLog_Filter_Button_Click(object sender, EventArgs e)
        {
            string test = "";
            var uc = (UC_NewLog)sender;
            foreach (DataRow dr in uc.data.Rows)
            {
                test = dr[0].ToString();
                break;
            }
            MessageBox.Show(test);
            test = "HEJSAN";
            foreach (DataRow dr1 in uc.data.Rows)
            {
                dr1[0] = test;
                break;
            }
            DataRow dr2;
            dr2 = uc.data.Rows[0];
            test = dr2[0].ToString();
            MessageBox.Show(test);
            uc.UpdateDataGridView(uc.data);
        }

        #endregion

        /// 
        /// All events here
        /// 
        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            
            LoadRecentList();
            if (!File.Exists(System.Environment.CurrentDirectory + logSettingsFileName)) CreateStandardSettingsXML();
            else
            {
             try {
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
                catch(Exception ex)
                { MessageBox.Show(ex.Message); }
            }

        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

       

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileTabControl.TabPages.Clear();
            
        }

        private void openLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select files...";
            openFileDialog1.Filter = "Comma separated files (*.csv)|*.csv";
            openFileDialog1.Multiselect = true;
            openFileDialog1.RestoreDirectory = true;
            DialogResult dRes;
            dRes = openFileDialog1.ShowDialog();
            if (dRes == DialogResult.OK)
            {
                //Check if multiple files
                if (openFileDialog1.FileNames != null)
                {
                    String[] fileNames = openFileDialog1.FileNames;
                    Boolean duplicates = false;
                    foreach ( string x in fileNames)
                    {
                        foreach ( string y in openedFiles)
                        {
                            if (x == y)
                            {
                                duplicates = true;                                
                            }
                        }
                    }

                    if (duplicates == true) MessageBox.Show("You can't add files with the same filename as any existing files!");
                    else
                    {
                        openedFiles.AddRange(fileNames);
                        PrepareWindowForNewFiles(fileNames);
                    }

                }
            }
        }

        private void RecentProject_click(object sender, EventArgs e)
        {
            //richTextBox1.LoadFile(sender.ToString(), RichTextBoxStreamType.PlainText); //same as open menu
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
