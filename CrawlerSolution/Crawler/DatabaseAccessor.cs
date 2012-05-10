using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Crawler
{
    public class DatabaseAccessor
    {
        /* And we have a default user!  WOO! */
        private string connectionDetails;
        private SqlConnection con;
        private Log databaseLogger;

        public DatabaseAccessor(Log logger, string connectionString)
        {

            databaseLogger = logger;
            connectionDetails = connectionString;
            con = new SqlConnection(connectionDetails);
        }

        public void AddVulnerabilities(int crawlID, List<String> details)
        {
            for(int i = 0; i < details.Count; i++)
            {
                try
                {
                    addVulnerability(crawlID, details[i]);
                }
                catch (SqlException e)
                {

                    string msg = "";
                    for (int j = 0; j < e.Errors.Count; j++)
                    {
                         msg += "Error #" + j + " Message: " + e.Errors[j].Message + "\n";
                    }
                    databaseLogger.writeError(msg);
                }
                catch (Exception e)
                {
                    databaseLogger.writeError(e.Message);
                }
            }
        }

        private void addVulnerability(int crawlID, string details)
        {
            string commandString = "addVulnerability";

            SqlCommand command = new SqlCommand(commandString, con);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter idParam = new SqlParameter();
            idParam.ParameterName ="@crawlID";
            idParam.SqlDbType = SqlDbType.Int;
            idParam.Size = 4;
            idParam.Value = crawlID;
            idParam.Direction = ParameterDirection.Input;
            command.Parameters.Add(idParam);


            SqlParameter detailParam = new SqlParameter("@details", SqlDbType.VarChar, 600);
            detailParam.Value = details;
            detailParam.Direction = ParameterDirection.Input;
            command.Parameters.Add(detailParam);

            try
            {
                con.Open();
                command.ExecuteNonQuery();
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
            string commandString = "createNewCrawl";

            SqlCommand command = new SqlCommand(commandString, con);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter outputParameter = new SqlParameter();
            outputParameter.ParameterName = "@newID";
            outputParameter.SqlDbType = SqlDbType.Int;
            outputParameter.Size = 4;
            outputParameter.Value = DBNull.Value;
            outputParameter.Direction = ParameterDirection.InputOutput;

            SqlParameter urlParam = new SqlParameter();
            urlParam.Value = url;
            urlParam.ParameterName = "@url";
            urlParam.SqlDbType = SqlDbType.VarChar;
            urlParam.Size = 500;
            command.Parameters.Add(urlParam);

            SqlParameter emailParam = new SqlParameter();
            emailParam.ParameterName = "@reqEmail";
            emailParam.SqlDbType = SqlDbType.VarChar;
            emailParam.Size = 60;
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

            databaseLogger.writeInfo("New Crawl ID is " + String.Format("{0:####}", crawlID));
            return crawlID;
        }

        /* Add a website.  Return its database ID.
         * Returns -1 if the command failed */
        public int addWebsite(string url, string software, string language, string version)
        {
            int websiteID = -1;
            


            string commandString = "insertNewWebsite";

            SqlCommand command = new SqlCommand(commandString, con);
            command.CommandType = CommandType.StoredProcedure;
            
            /* Define Parameters */
            SqlParameter outputParam = new SqlParameter();
            outputParam.ParameterName = "@ret";
            outputParam.SqlDbType = SqlDbType.Int;
            outputParam.Size = 4;
            outputParam.Value = DBNull.Value;
            outputParam.Direction = ParameterDirection.InputOutput;
            
            SqlParameter urlParam = new SqlParameter();
            urlParam.ParameterName = "@url";
            urlParam.SqlDbType = SqlDbType.VarChar;
            urlParam.Size = 500;
            urlParam.Value = url;
            urlParam.Direction = ParameterDirection.Input;
            command.Parameters.Add( urlParam );
            
            
            SqlParameter languageParam = new SqlParameter();
            languageParam.ParameterName = "@lang";
            languageParam.SqlDbType = SqlDbType.Char;
            languageParam.Size = 25;
            if (language == null)
            {
                languageParam.Value = DBNull.Value;
            }
            else
            {
                languageParam.Value = language;
            }
            languageParam.Direction = ParameterDirection.Input;
            command.Parameters.Add( languageParam );
            
            SqlParameter softwareParam = new SqlParameter();
            softwareParam.ParameterName = "@serverSoftware";
            softwareParam.SqlDbType = SqlDbType.Char;
            softwareParam.Size = 25;
            if (software == null)
            {
                softwareParam.Value = DBNull.Value;
            }
            else
            {
                softwareParam.Value = software;
            }
            softwareParam.Direction = ParameterDirection.Input;
            command.Parameters.Add( softwareParam );

            SqlParameter versionParam = new SqlParameter();
            versionParam.ParameterName = "@version";
            versionParam.SqlDbType = SqlDbType.Char;
            versionParam.Size = 15;
            if (version == null)
            {
                versionParam.Value = DBNull.Value;
            }
            else
            {
                versionParam.Value = version;
            }
            versionParam.Direction = ParameterDirection.Input;
            command.Parameters.Add( versionParam );
            
            command.Parameters.Add( outputParam );
            
            try
            {
                con.Open();
                command.ExecuteNonQuery();
                websiteID = (int)command.Parameters["@ret"].Value;
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

   }

}
