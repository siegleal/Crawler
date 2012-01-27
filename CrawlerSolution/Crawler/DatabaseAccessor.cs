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
        /* Hey guys, please don't abuse my whale login ID while I get sriram to make us
         * an ID we can use >.> */
        static string connectionDetails = "Database=SecurityCrawlerDatabase;" +
            "user id=eatonmi;" +
            "password=WoWLog#1;" +
            "server=whale.cs.rose-hulman.edu;" +
            "connection timeout=30;";
        static SqlConnection con = new SqlConnection(connectionDetails);
        static Logger.Log databaseLogger;

        public DatabaseAccessor(Logger.Log logger)
        {
            databaseLogger = logger;
        }

        public void addVulnerability(int crawlID, string details)
        {
            string commandString = "dbo.addVulnerability";

            SqlCommand command = new SqlCommand(commandString, con);

            SqlParameter idParam = new SqlParameter("@crawlID", SqlDbType.Int, 4);
            idParam.Value = crawlID;
            command.Parameters.Add(idParam);

            SqlParameter detailParam = new SqlParameter("@details", SqlDbType.VarChar, 600);
            detailParam.Value = details;
            command.Parameters.Add(details);

            try
            {
                con.Open();
                command.ExecuteNonQuery();
                crawlID = (int)command.Parameters["@newID"].Value;
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
        }

        public int newCrawl(string url, string email)
        {
            int crawlID = -1;
            string commandString = "dbo.createNewCrawl";

            SqlCommand command = new SqlCommand(commandString, con);

            SqlParameter outputParameter = new SqlParameter("@newID", SqlDbType.Int, 4);
            outputParameter.Direction = ParameterDirection.Output;

            SqlParameter urlParam = new SqlParameter("@url", SqlDbType.VarChar, 500);
            urlParam.Value = url;
            command.Parameters.Add(urlParam);

            SqlParameter emailParam = new SqlParameter("@reqEmail", SqlDbType.VarChar, 60);
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            command.Parameters.Add(outputParameter);

            try
            {
                con.Open();
                command.ExecuteNonQuery();
                crawlID = (int)command.Parameters["@newID"].Value;
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
            return crawlID;
        }

        /* Add a website.  Return its database ID
         * Returns -1 if the command failed */
        public int addWebsite(string url, string software, string language, string version)
        {
            int websiteID = -1;
            /* TODO:  Write the command */
            string commandString = "dbo.InsertNewWebsite";
            
            SqlCommand command = new SqlCommand(commandString, con);
            command.CommandType = CommandType.StoredProcedure;
            /* Define Parameters */
            SqlParameter outputParam = new SqlParameter("@output", SqlDbType.Int, 4);
            outputParam.Direction = ParameterDirection.Output;

            SqlParameter urlParam = new SqlParameter("@url", SqlDbType.VarChar, 500);
            urlParam.Value = url;
            command.Parameters.Add( urlParam );

            SqlParameter languageParam = new SqlParameter("@language", SqlDbType.Char, 15);
            languageParam.Value = language;
            command.Parameters.Add(languageParam);

            SqlParameter softwareParam = new SqlParameter("@serverSoftware", SqlDbType.Char, 25);
            softwareParam.Value = software;
            command.Parameters.Add(softwareParam);

            SqlParameter versionParam = new SqlParameter("@version", SqlDbType.Char, 15);
            versionParam.Value = version;
            command.Parameters.Add( versionParam );
            command.Parameters.Add( outputParam );

            try
            {
                con.Open();
                command.ExecuteNonQuery();
                websiteID = (int)command.Parameters["@output"].Value;
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
            return websiteID;
        }

        /* DO NOT USE THIS YET:  It's far from complete, mostly because I don't yet know
         * how to handle tables as output from a stored procedure */
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
