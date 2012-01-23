using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Crawler
{
    class DatabaseAccessor
    {
        /* NOTE:  Need to figure out authentication details for string.
         * This is PROBABLY right.  Probably.*/
        static string connectionDetails = "Initial Catalogue=SecurityCrawlerDatabase;" +
            "user id=eatonmi;" +
            "password=eatonmi;" +
            "server=whale.cs.rose-hulman.edu;" +
            "connection timeout=30;" +
            "Trusted_Connection=yes;";
        static SqlConnection con = new SqlConnection(connectionDetails);
        static string databaseAccessLogFile = "DatabaseLog.txt";
        static Logger.Log databaseLogger = new Logger.Log(databaseAccessLogFile);

        public DatabaseAccessor()
        {
        }

        public List<String> getWebsiteVulnerabilites(string url)
        {
            List<String> result = new List<String>();
            /* TODO:  Write the command.  Should return single column table of vulnerabilities */
            string command = "";
            SqlDataAdapter adapt = new SqlDataAdapter(command, con);
            DataTable data = new DataTable();

            try
            {
                con.Open();

                int records = adapt.Fill(data);

                foreach (DataRow row in data.Rows)
                {
                    result.Add(row[0].ToString());
                }

            }
            catch (SqlException e)
            {
                string msg = "";
                for (int i = 0; i < e.Errors.Count; i++)
                {
                    msg += "Error #" + i + " Message: " + e.Errors[i].Message + "\n";
                }
                databaseLogger.writeError(msg);
            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
            }
            return result;
        }
    }

}
