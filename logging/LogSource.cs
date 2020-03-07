
namespace Logging
{
    /// <summary>
    /// Specifies the method of logging
    /// </summary>

    public enum LogOutput
    {
        /// <summary>
        /// Log the the file specified by the LogFile attribute of the Logger class. By default this is the 
        /// executable name and directory with a ".log" extension
        /// </summary>

        ToFile,

        /// <summary>
        /// Log to the Windows event log
        /// </summary>

        ToEventLog,

        /// <summary>
        /// Log to the database.
        /// </summary>

        ToDatabase,

        /// <summary>
        /// Log to the console.
        /// </summary>

        ToConsole
    }
}
