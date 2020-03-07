using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    /// <summary>
    /// 
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Lowest level of logging - only Errors will be logged
        /// </summary>

        Error = 1,

        /// <summary>
        /// Next level up from Error - Errors and Warnings will be logged
        /// </summary>

        Warning = 2,

        /// <summary>
        /// All messages will be logged
        /// </summary>

        Information = 4
    }
}
