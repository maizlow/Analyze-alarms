using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LiteDB;

namespace Analyze_alarms.Classes
{
    public class DataBase
    {
        private string dbPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\localdb.db";
        
        #region SAVE
        public void SaveDataTable(List<DataTableRowClass> data, string colName)
        {
            if (!CheckIfCollectionExists(colName)) InsertDatatable(data, colName);
        }

        public void SaveAnalyzedRows(List<AnalyzedRows> data, string colName)
        {
            if (!CheckIfCollectionExists(colName)) InsertAnalyzedRows(data, colName);
        }

        /// <summary>
        /// Will save to a new Collection if colName don't exists, otherwise it will update the existing one.
        /// </summary>
        /// <param name="sData">Summary data class</param>
        /// <param name="sColName">Summary data collection name</param>
        public void SaveSummaryData(List<Summary> sData, string colName)
        {
            if (CheckIfCollectionExists(colName)) UpdateSummaryData(sData, colName);
            else InsertSummaryData(sData, colName);
        }

        /// <summary>
        /// Will save to a new Collection if colName don't exists, otherwise it will update the existing one.
        /// </summary>
        /// <param name="rData">Report data class</param>
        /// <param name="rColName">Report data collection name</param>
        public void SaveReportData(ReportFormData rData, string colName)
        {
            if (CheckIfCollectionExists(colName)) UpdateReportData(rData, colName);
            else InsertReportData(rData, colName);
        }

        #endregion

        #region LOAD
        /// <summary>
        /// Loads and return a list of all data entrys
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public List<DataTableRowClass> LoadDataTablesData(string colName)
        {
            if (CheckIfCollectionExists(colName))
                return ReadDataTableData(colName);
            else return null;
        }

        public List<AnalyzedRows> LoadAnalyzedRowsData(string colName)
        {
            if (CheckIfCollectionExists(colName))
                return ReadAnalyzedRowsData(colName);
            else return null;
        }

        /// <summary>
        /// Loads and returns a list of Summary class.
        /// </summary>
        /// <param name="sColName">Collection name of summary</param>
        /// <returns></returns>
        public List<Summary> LoadSummaryData(string colName)
        {
            if (CheckIfCollectionExists(colName))
                return ReadSummaryData(colName);
            else return null;
        }

        /// <summary>
        /// Loads and returns a copy of the ReportFormData class.
        /// </summary>
        /// <param name="rColName">Collection name of reportdata</param>
        /// <returns></returns>
        public ReportFormData LoadReportData(string colName)
        {
            if (CheckIfCollectionExists(colName))
                return ReadReportData(colName);
            else return null;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Check database for an existing document
        /// </summary>
        /// <param name="colName">Collection name</param>
        /// <returns></returns>
        private bool CheckIfCollectionExists(string colName)
        {
            using (var db = new LiteDatabase(dbPath))
            {                
                return db.CollectionExists(colName);
            }            
        }


        /// <summary>
        /// Updates an existing documents entrys
        /// </summary>
        /// <param name="data"></param>
        private void UpdateReportData(ReportFormData data, string colName)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<ReportFormData>(colName);
                
                col.Update(data);
            }
        }

        /// <summary>
        /// Updates an existing documents entrys
        /// </summary>
        /// <param name="data"></param>
        private void UpdateSummaryData(List<Summary> data, string colName)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Summary>(colName);
                
                col.Update(data);
            }
        }


        /// <summary>
        /// Insert the datatable in a collection
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        private void InsertDatatable(List<DataTableRowClass> data, string colName)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<DataTableRowClass>(colName);


                foreach (DataTableRowClass dtrc in data)
                {

                    col.Insert(dtrc);

                }

                //Create index
                col.EnsureIndex("Id");
            }
        }

        /// <summary>
        /// Insert the analyzed rows in a collection
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        private void InsertAnalyzedRows(List<AnalyzedRows> data, string colName)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<AnalyzedRows>(colName);


                foreach (AnalyzedRows dtrc in data)
                {

                    col.Insert(dtrc);

                }

                //Create index
                col.EnsureIndex("Id");
            }
        }

        /// <summary>
        /// Inserts new data in a collection. Generate a new collection if neccessary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        private void InsertReportData(ReportFormData data, string colName)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<ReportFormData>(colName);
                
                col.Insert(data);

                //Create index
                col.EnsureIndex("Id");
            }
        }

        /// <summary>
        /// Inserts new data in a collection. Generate a new collection if neccessary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colName"></param>
        private void InsertSummaryData(List<Summary> data, string colName)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(dbPath))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Summary>(colName);
                
                foreach (Summary s in data)
                {
                    col.Insert(s);                    
                }
                //Create index
                col.EnsureIndex("Id");

            }
        }
        
        /// <summary>
        /// Reads data table data and returns it
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        private List<DataTableRowClass> ReadDataTableData(string colName)
        {
            using (var db = new LiteDatabase(dbPath))
            {
                var col = db.GetCollection<DataTableRowClass>(colName);
                col.EnsureIndex("Id", true);
                var result = col.FindAll().ToList();
                List<DataTableRowClass> summary = new List<DataTableRowClass>();
                foreach (DataTableRowClass s in result)
                {
                    summary.Add(s);
                }
                return summary;
            }
        }

        /// <summary>
        /// Reads data from collection AnalyzedRows
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        private List<AnalyzedRows> ReadAnalyzedRowsData(string colName)
        {
            using (var db = new LiteDatabase(dbPath))
            {
                var col = db.GetCollection<AnalyzedRows>(colName);
                col.EnsureIndex("Id", true);
                var result = col.FindAll().ToList();
                List<AnalyzedRows> summary = new List<AnalyzedRows>();
                foreach (AnalyzedRows s in result)
                {
                    summary.Add(s);
                }
                return summary;
            }
        }

        /// <summary>
        /// Reads data in collection and returns List<> of type
        /// </summary>
        /// <param name="colName"></param>
        private List<Summary> ReadSummaryData(string colName)
        {
            using (var db = new LiteDatabase(dbPath))
            {
                var col = db.GetCollection<Summary>(colName);
                col.EnsureIndex("Id", true);
                var result = col.FindAll().ToList();
                List<Summary> summary = new List<Summary>();
                foreach (Summary s in result)
                {
                    summary.Add(s);
                }
                return summary;
            }
        }

        /// <summary>
        /// Reads data in collection and returns List<> of type
        /// </summary>
        /// <param name="colName"></param>
        private ReportFormData ReadReportData(string colName)
        {
            using (var db = new LiteDatabase(dbPath))
            {
                var col = db.GetCollection<ReportFormData>(colName);
                col.EnsureIndex("Id", true);
                var result = col.FindAll().ToList();
                List<ReportFormData> rlist = new List<ReportFormData>();
                foreach(ReportFormData r in result)
                {
                    rlist.Add(r);
                }

                return rlist[0];
            }
        }

        #endregion
    }
}
