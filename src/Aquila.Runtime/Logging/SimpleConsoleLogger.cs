using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Logging
{
    public class SimpleConsoleLogger<T> : AbstractLogger, ILogger<T>
    {
        public override bool IsTrace => true;

        public override bool IsDebug => true;

        public override bool IsInfo => true;

        public override bool IsWarn => true;

        public override bool IsError => true;

        private readonly string className = typeof(T).FullName;

        public override void Debug(string message, params object[] arg)
        {
            Console.WriteLine($"DEBUG|{className}|{message}", arg);
        }

        public override void Debug(Exception ex, string message, params object[] arg)
        {
            Debug($"{message}|{ex.ToString()}", arg);
        }

        public override void Error(string message, params object[] arg)
        {
            Console.WriteLine($"ERROR|{className}|{message}", arg);
        }

        public override void Error(Exception ex, string message, params object[] arg)
        {
            Error($"{message}|{ex.ToString()}", arg);
        }

        public override void Info(string message, params object[] arg)
        {
            Console.WriteLine($"INFO|{className}|{message}", arg);
        }

        public override void Info(Exception ex, string message, params object[] arg)
        {
            Info($"{message}|{ex.ToString()}", arg);
        }

        public override void Trace(string message, params object[] arg)
        {
            Console.WriteLine($@"TRACE|{className}|{message}");
        }

        public override void Trace(Exception ex, string message, params object[] arg)
        {
            Trace($"{message}|{ex.ToString()}", arg);
        }

        public override void Warn(string message, params object[] arg)
        {
            Console.WriteLine($"WARN|{className}|{message}", arg);
        }

        public override void Warn(Exception ex, string message, params object[] arg)
        {
            Warn($"{message}|{ex.ToString()}", arg);
        }
    }
}