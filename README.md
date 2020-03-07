# C# Console Application Logger

A C# DLL project for console applications to use for logging. Supports logging to the console, a text file, to the Windows Event Log, and to a SQL Server database table. (Requires creating the table and a logging stored procedure as part of setup. DDL included.) On the project where this was used, we had a mix of processing by various console utilities, as well as in the SQL Server database so we wanted to log everything to the database so all logs would be in one location.

# Usage

Create a console application, and add a reference to the `logger` DLL. Then, use the logger in your program:

```
using Logging;

namespace LogTest
{
    class Program
    {
        static readonly Logger logger = new Logger
        {
            Level = LogLevel.Information,
            Output = LogOutput.ToFile
        };

        static void Main(string[] args)
        {
            logger.InformationMessage("This is an information message");
        }
    }
}
```
In the above example after you build, then:
```
C:\LogTest\LogTest\bin\Debug> LogTest
C:\LogTest\LogTest\bin\Debug> type LogTest.log
3/7/2020 5:08:18 PM -- info -- This is an information message
```
