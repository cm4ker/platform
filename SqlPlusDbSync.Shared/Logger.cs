using System;
using log4net;
using log4net.Config;

namespace SqlPlusDbSync.Shared
{
    public static class Logger
    {
        private static ILog log = LogManager.GetLogger("LOGGER");


        public static ILog Log
        {
            get { return log; }
        }

        private static void ConsoleOutput(string type, string message, Exception ex = null)
        {
            if (ex is null)
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{type}] {message}");
            else
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{type}] {message}\nException: {ex.Message} \nStackTrace:{ex.StackTrace}");
        }

        public static void LogInfo(string message, Exception ex = null)
        {
            if (log.IsInfoEnabled)
            {
                ConsoleOutput("INFO", message, ex);
                log.Info(message, ex);
            }
        }

        public static void LogWarning(string message, Exception ex = null)
        {
            if (log.IsWarnEnabled)
            {
                ConsoleOutput("WARNING", message, ex);
                log.Warn(message, ex);
            }
        }

        public static void LogError(string message, Exception ex)
        {
            if (log.IsErrorEnabled)
            {
                ConsoleOutput("ERROR", message, ex);
                log.Error(message, ex);
            }
        }

        public static void LogDebug(string message, Exception ex = null)
        {
            if (log.IsDebugEnabled)
            {
                ConsoleOutput("DEBUG", message, ex);
                log.Debug(message, ex);
            }
        }

        public static void LogFatal(string message, Exception ex)
        {
            if (log.IsFatalEnabled)
            {
                ConsoleOutput("FATAL", message, ex);
                log.Fatal(message, ex);
            }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
