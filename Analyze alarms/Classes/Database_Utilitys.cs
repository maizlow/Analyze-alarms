using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;

namespace Analyze_alarms
{
    public class DatabaseUtilitys
    {

        public bool StoreLogFileInDB(DataTable data)
        {
            try
            {

                using (var bulkCopy = new SqlBulkCopy(GetConnectionString(), SqlBulkCopyOptions.Default))
                {
                    //my DataTable column names match my SQL Column names, so I simply made this loop.However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
                    foreach (DataColumn col in data.Columns)
                    {
                        //MessageBox.Show(col.ColumnName + " + " + col.DataType.ToString());
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "LogFiles";
                    bulkCopy.WriteToServer(data);
                }
                return true;
                ////TEST CODE THAT READS BACK AND SHOWS DATABASE
                //SqlConnection conn = new SqlConnection(GetConnectionString());
                //conn.Open();

                //SqlCommand command = new SqlCommand("Select * from [LogFiles]", conn);
                //DataTable dt = new DataTable();
                
                //int i = 0;

                //for (i = 0; i < 6; i++)
                //{
                //    dt.Columns.Add();
                //}

                //i = 0;
                //using (SqlDataReader reader = command.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                //        DataRow dr = dt.NewRow();
                //        dr[0] = reader[0];
                //        dr[1] = reader[1];
                //        dr[2] = reader[2];
                //        dr[3] = reader[3];
                //        dr[4] = reader[4];
                //        dr[5] = reader[5];
                //        dt.Rows.Add(dr);
                //    }


                //}

                //TEMP tmp = new TEMP();
                //DataGridView dgv = (DataGridView)tmp.Controls[0];
                //dgv.DataSource = dt;
                //tmp.Show();

                //conn.Close();
              
            }
            catch(Exception ex)
            {
                MessageBox.Show("Log could not be stored in database.  " + ex.Message);
            }

            return false;
        }


        //Get the connection string from App config file.  
        internal static string GetConnectionString()
        {
            return "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+ System.Environment.CurrentDirectory + "\\Data\\LocalDatabase.mdf;Integrated Security=True";

        }
    }
}
