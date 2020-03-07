using System;
using System.Data;
using System.Data.SqlClient;

namespace Logging
{
    internal class SqlUtil
    {
        /// <summary>
        /// Creates and returns a connection to SQL Server
        /// </summary>
        /// <param name="SQLServer">The host name of the SQL server (e.g. "localhost")</param>
        /// <param name="Database">The database bane containing the logging stored procedure</param>
        /// <returns>A System.Data.SqlClient.SqlConnection object</returns>

        public static SqlConnection GetSqlConnection(string SQLServer, string Database)
        {
            SqlConnection Cnct = null;
            try
            {
                string CnctStr = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=true;", SQLServer, Database);
                Cnct = new SqlConnection(CnctStr);
            }
            catch
            {
                //TODO --> Log.ErrorMessage("An exception occurred attempting to esablish a SQL connection to server {0}", .SQLServer);
            }
            return Cnct;
        }

        /// <summary>
        /// Build a SQL Command object to log to the database using the default SQL logging configuration
        /// </summary>
        /// <param name="GUID">A GUID ties together a "session" of activity</param>
        /// <param name="SQLSP">The logging stored procedure that is invoked on the SQL Server</param>
        /// <param name="AppName">The application name generating the log entry</param>
        /// <param name="JobName">The job name</param>
        /// <param name="MsgType">A string characterizing the message type</param>
        /// <param name="Msg">The message contents</param>
        /// <returns>A System.Data.SqlClient.SqlCommand object</returns>

        public static SqlCommand BuildLoggingCmd(Guid GUID, string SQLSP, string AppName, string JobName, string MsgType, string Msg)
        {
            SqlParameter[] Parms =
            {
                new SqlParameter("@job_id", GUID),
                new SqlParameter("@job_name", JobName),
                new SqlParameter("@type", MsgType),
                new SqlParameter("@component", AppName),
                new SqlParameter("@message", Msg)
            };

            SqlCommand Cmd = new SqlCommand();
            Cmd.CommandText = SQLSP;
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddRange(Parms);
            return Cmd;
        }
    }
}
