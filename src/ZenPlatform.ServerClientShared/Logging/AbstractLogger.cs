using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Logging
{
    public abstract class AbstractLogger: ILogger
    {
        public abstract bool IsTrace { get; }

        public abstract bool IsDebug { get; }

        public abstract bool IsInfo  { get; }

        public abstract bool IsWarn { get; }

        public abstract bool IsError { get; }

        public abstract void Debug(string message, params object[] arg);

        public abstract void Debug(Exception ex, string message, params object[] arg);

        public void Debug(Func<string> messageGenerator)
        {
            if (IsDebug)
                Debug(messageGenerator());
        }

        public abstract void Error(string message, params object[] arg);

        public abstract void Error(Exception ex, string message, params object[] arg);

        public void Error(Func<string> messageGenerator)
        {
            if (IsError)
                Error(messageGenerator());
        }

        public abstract void Info(string message, params object[] arg);

        public abstract void Info(Exception ex, string message, params object[] arg);

        public void Info(Func<string> messageGenerator)
        {
            if (IsInfo)
                Info(messageGenerator());
        }

        public abstract void Trace(string message, params object[] arg);

        public abstract void Trace(Exception ex, string message, params object[] arg);

        public void Trace(Func<string> messageGenerator)
        {
            if (IsTrace)
                Trace(messageGenerator());
        }

        public abstract void Warn(string message, params object[] arg);

        public abstract void Warn(Exception ex, string message, params object[] arg);

        public void Warn(Func<string> messageGenerator)
        {
            if (IsWarn)
                Warn(messageGenerator());
        }
    }
}
