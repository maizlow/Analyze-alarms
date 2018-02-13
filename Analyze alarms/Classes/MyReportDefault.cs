using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Analyze_alarms.Classes
{
    public class MyReportDefault
    {
        private const string path = "\\ReportDefault.xml";

        public string Header { get; set; }
        public string ReportFrom { get; set; }
        public string ReportBy { get; set; }
        public bool RowChart { get; set; }
        public bool PieChart { get; set; }
        //TODO: public bool ScatterChart { get; set; }
        public bool Summary { get; set; }
        public string LogoFilePath { get; set; }

        /// <summary>
        /// Use this constructor to create a new save of Default report data
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="ReportFrom"></param>
        /// <param name="ReportBy"></param>
        /// <param name="RowChart"></param>
        /// <param name="PieChart"></param>
        /// <param name="Summary"></param>
        /// <param name="LogoFilePath"></param>
        public MyReportDefault(string Header, string ReportFrom, string ReportBy, 
                               bool RowChart, bool PieChart, bool Summary, string LogoFilePath)
        {
            this.Header = Header;
            this.ReportFrom = ReportFrom;
            this.ReportBy = ReportBy;
            this.RowChart = RowChart;
            this.PieChart = PieChart;
            this.Summary = Summary;
            this.LogoFilePath = LogoFilePath;

            UpdateXMLFile();
        }

        /// <summary>
        /// Use this constructor to read from file and populate class
        /// </summary>
        public MyReportDefault()
        {
            if (File.Exists(Environment.CurrentDirectory + path))
                ReadXML();
        }

        public void UpdateXMLFile()
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), 
                new XElement("ReportDefault",
                    new XAttribute("Header", Header),
                    new XAttribute("ReportFrom", ReportFrom),
                    new XAttribute("ReportBy", ReportBy),
                    new XAttribute("RowChart", RowChart),
                    new XAttribute("PieChart", PieChart),
                    new XAttribute("Summary", Summary),
                    new XAttribute("LogoFilePath", LogoFilePath)));

            
            doc.Save(Environment.CurrentDirectory + path);
        }

        private void ReadXML()
        {
            XElement item = XElement.Load(Environment.CurrentDirectory + path);

            Header = item.Attribute("Header").Value;
            ReportFrom = item.Attribute("ReportFrom").Value;
            ReportBy = item.Attribute("ReportBy").Value;
            RowChart = bool.Parse(item.Attribute("RowChart").Value);
            PieChart = bool.Parse(item.Attribute("PieChart").Value);
            Summary = bool.Parse(item.Attribute("Summary").Value);
            LogoFilePath = item.Attribute("LogoFilePath").Value;            
        }

    }



}
