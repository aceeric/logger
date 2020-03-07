using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Logging
{
    /// <summary>
    /// Provides the ability to log to the console, a text logfile, SQL Server, or the Windows Event Log
    /// </summary>

    public class Logger
    {
        private string    LoggingJobName;
        private string    LoggingAppName;
        private string    LoggingLogFile;
        private bool      LoggingEnabled;
        private LogOutput LoggingOutput;
        private LogLevel  LoggingLevel;
        private string    LoggingSQLServer;
        private string    LoggingSQLCatalog;
        private string    LoggingSQLSP;
        private Guid      LoggingGUID;

        /// <summary>
        /// Gets or sets the logging GUID for the logging subsystem
        /// </summary>

        public Guid GUID
        {
            get
            {
                return LoggingGUID;
            }
            set
            {
                LoggingGUID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Job Name for the logging subsystem
        /// </summary>

        public string JobName
        {
            get
            {
                return LoggingJobName;
            }
            set
            {
                LoggingJobName = value;
            }
        }

        /// <summary>
        /// Gets or sets the SQL Server that is used for database logging
        /// </summary>

        public string SQLServer
        {
            get
            {
                return LoggingSQLServer;
            }
            set
            {
                LoggingSQLServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the log file name
        /// </summary>

        public string LogFile
        {
            get
            {
                return LoggingLogFile;
            }
            set
            {
                LoggingLogFile = value;
            }
        }

        /// <summary>
        /// Enables or disables logging
        /// </summary>

        public bool Enabled
        {
            get
            {
                return LoggingEnabled;
            }

            set
            {
                LoggingEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of logging (text or event log)
        /// </summary>

        public LogOutput Output
        {
            get
            {
                return LoggingOutput;
            }

            set
            {
                LoggingOutput = value;
            }
        }

        /// <summary>
        /// Gets or sets the logging level
        /// </summary>

        public LogLevel Level
        {
            get
            {
                return LoggingLevel;
            }

            set
            {
                LoggingLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the application name that is used in the Windows event log. Has no effect for
        /// text logging. Only affects Windows Event Log logging.
        /// </summary>

        public string AppName
        {
            get
            {
                return LoggingAppName;
            }

            set
            {
                LoggingAppName = value;
            }
        }

        /// <summary>
        /// Initializes the instance for basic logging functionality: error-only text file logging in the application's
        /// EXE directory. Log file named the same as the EXE, with a ".log" extension. No Windows event logging.
        /// </summary>

        public Logger()
        {
            try
            {
                LoggingAppName = Assembly.GetEntryAssembly().GetName().Name;
            } catch
            {
                LoggingAppName = "ERROR";
            }
            LoggingJobName = "undefined";
            LoggingLogFile = Path.Combine(Environment.CurrentDirectory,
                LoggingAppName + ".log");
            LoggingEnabled = true;
            LoggingOutput = LogOutput.ToFile;
            LoggingLevel = LogLevel.Error; // only log errors
            LoggingSQLServer = "localhost";
            LoggingSQLCatalog = "meta";
            LoggingGUID = Guid.NewGuid();
            LoggingSQLSP = "sp_log";
        }

        /// <summary>
        /// Initializes an instance with the passed GUID. Calls the base class no-arg constructor then
        /// replaces the generated GUID with the passed GUID.
        /// </summary>
        /// <param name="LoggingGUID">A GUID to use for this instance</param>

        public Logger(Guid LoggingGUID) : this()
        {
            this.LoggingGUID = LoggingGUID;
        }

        /// <summary>
        /// Initializes an instance with the passed GUID. Calls the base class no-arg constructor then
        /// replaces the generated GUID with the passed GUID.
        /// </summary>
        /// <param name="LoggingGUID">A GUID to use for this instance</param>
        /// <param name="JobName">A job name for this instance</param>

        public Logger(Guid LoggingGUID, string JobName) : this(LoggingGUID)
        {
            this.JobName = JobName;
        }

        /// <summary>
        /// Writes a warning message.
        /// </summary>
        /// <param name="Msg">A string suitable for string.Format</param>
        /// <param name="Args">A params list suitable for string.Format (or null)</param>

        public void WarningMessage(string Msg, params object[] Args)
        {
            Message(Msg, LogLevel.Warning, Args);
        }

        /// <summary>
        /// Writes an information message.
        /// </summary>
        /// <param name="Msg">A string suitable for string.Format</param>
        /// <param name="Args">A params list suitable for string.Format (or null)</param>

        public void InformationMessage(string Msg, params object[] Args)
        {
            Message(Msg, LogLevel.Information, Args);
        }

        /// <summary>
        /// Writes an errror message.
        /// </summary>
        /// <param name="Msg">A string suitable for string.Format</param>
        /// <param name="Args">A params list suitable for string.Format (or null)</param>

        public void ErrorMessage(string Msg, params object[] Args)
        {
            Message(Msg, LogLevel.Error, Args);
        }

        /// <summary>
        /// Parses the message and arguments using string.Format. It's possible the caller could pass invalid arguments
        /// so this method catches errors and returns MsgArg un-modified if the format operation throws.
        /// </summary>
        /// <param name="Msg">A string suitable for string.Format</param>
        /// <param name="Args">A params list suitable for string.Format (or null)</param>
        /// <returns>The formatted message, or Msg un-modified if string.Format throws</returns>

        private string ParseMsg(string Msg, params object[] Args)
        {
            try
            {
                return string.Format(Msg, Args);
            }
            catch
            {
                return Msg;
            }
        }

        /// <summary>
        /// Writes the message to the log file or the event log based on the instance's configuration
        /// </summary>
        /// <param name="Msg">A string suitable for string.Format</param>
        /// <param name="MsgLevel">Message type</param>
        /// <param name="Args">A params list suitable for string.Format (or null)</param>

        public void Message(string Msg, LogLevel MsgLevel, params object[] Args)
        {
            try
            {
                string FormattedMsg = ParseMsg(Msg, Args);

                // Windows event log

                if (LoggingEnabled && LoggingOutput == LogOutput.ToEventLog && LoggingLevel.CompareTo(MsgLevel) >= 0 && LoggingAppName != null)
                {
                    if (!EventLog.SourceExists(LoggingAppName))
                    {
                        // If our application is not registered as a source with the event logging
                        // subsystem then go ahead and register it

                        EventLog.CreateEventSource(LoggingAppName, "Application");
                    }
                    EventLog.WriteEntry(LoggingAppName, FormattedMsg, XlatToWin(MsgLevel));
                }

                // Text file log

                else if (LoggingEnabled && LoggingOutput == LogOutput.ToFile && LoggingLogFile != null && LoggingLevel.CompareTo(MsgLevel) >= 0)
                {
                    using (StreamWriter sw = new StreamWriter(LoggingLogFile, true)) // true means append
                    {
                        sw.WriteLine(string.Format("{0} -- {1,-4} -- {2}", DateTime.Now, XlatToStr(MsgLevel), FormattedMsg));
                    }
                }

                // SQL log

                else if (LoggingEnabled && LoggingOutput == LogOutput.ToDatabase && LoggingLevel.CompareTo(MsgLevel) >= 0)
                {
                    LogToSQL(XlatToStr(MsgLevel), FormattedMsg);
                }

                // Console log

                else if (LoggingEnabled && LoggingOutput == LogOutput.ToConsole && LoggingLevel.CompareTo(MsgLevel) >= 0)
                {
                    Console.WriteLine(FormattedMsg);
                }
            }
            catch (Exception Ex)
            {
                if (LoggingOutput == LogOutput.ToDatabase) // If we get an error trying to log to the database, switch to text file logging
                {
                    if (LoggingLogFile != null)
                    {
                        LoggingOutput = LogOutput.ToFile;
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(LoggingLogFile, true)) // true means append
                            {
                                sw.WriteLine(string.Format("{0} -- {1,-4} -- {2}", DateTime.Now, "ERR", string.Format("An error occurred attempting to log to the database. The error was: {0}. Switching over to text file logging. Stack trace follows: {1}", Ex.Message, Ex.StackTrace)));
                            }
                        }
                        catch { } // no meaningful action
                    }
                    else // ok no text log - use event log
                    {
                        LoggingOutput = LogOutput.ToEventLog;
                        try
                        {
                            if (!EventLog.SourceExists(LoggingAppName))
                            {
                                // If our application is not registered as a source with the event logging
                                // subsystem then go ahead and register it

                                EventLog.CreateEventSource(LoggingAppName, "Application");
                            }
                            EventLog.WriteEntry(LoggingAppName, string.Format("An error occurred attempting to log to the database. The error was: {0}. Switching over to Windows Event Log logging. Stack trace follows: {1}", Ex.Message, Ex.StackTrace), EventLogEntryType.Error);
                        }
                        catch { } // no meaningful action
                    }
                }
            }
        }

        /// <summary>
        /// Translate the Logging enumeration level used by the class to appropriate the Windows event log level enumeration
        /// </summary>
        /// <param name="MsgLevel">The log level defined by the logging subsystem</param>
        /// <returns>The matching Windows event log enumeration value</returns>

        private EventLogEntryType XlatToWin(LogLevel MsgLevel)
        {
            switch (MsgLevel)
            {
                case LogLevel.Warning:
                    return EventLogEntryType.Warning;
                case LogLevel.Error:
                    return EventLogEntryType.Error;
                default:
                    return EventLogEntryType.Information;
            }
        }

        /// <summary>
        /// Translate the Logging enumeration level used by the class to a string for text-based logging
        /// </summary>
        /// <param name="MsgLevel">The log level defined by the logging subsystem</param>
        /// <returns>The matching Windows event log enumeration value</returns>

        private string XlatToStr(LogLevel MsgLevel)
        {
            switch (MsgLevel)
            {
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "err";
                default:
                    return "info";
            }
        }

        /// <summary>
        /// Log to the database using the default logging parameters - requires the sp_log procedure to be installed in the 
        /// 'meta' database. Override this method for customized database logging functionality.
        /// </summary>
        /// <param name="MsgTyp">A string representation of the type. By convention, should be "err", "info", or "warn" but these are not enforced</param>
        /// <param name="Msg">A message to log</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>

        public virtual void LogToSQL(string MsgTyp, string Msg)
        {
            using (SqlConnection Cnct = SqlUtil.GetSqlConnection(LoggingSQLServer, LoggingSQLCatalog))
            {
                Cnct.Open();
                using (SqlCommand Cmd = SqlUtil.BuildLoggingCmd(LoggingGUID, LoggingSQLSP, LoggingAppName, LoggingJobName, MsgTyp, Msg))
                {
                    Cmd.Connection = Cnct;
                    int RowCnt = Cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
