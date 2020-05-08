using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Logging
{
    public interface ILogger<T> :ILogger
    {

    }
    public interface ILogger
    {
        bool IsTrace { get; }
        bool IsDebug { get; }
        bool IsInfo { get; }
        bool IsWarn { get; }
        bool IsError { get; }

        void Trace(string message, params object[] arg);
        void Debug(string message, params object[] arg);
        void Info(string message, params object[] arg);
        void Warn(string message, params object[] arg);
        void Error(string message, params object[] arg);

        void Trace(Exception ex, string message, params object[] arg);
        void Debug(Exception ex, string message, params object[] arg);
        void Info(Exception ex, string message, params object[] arg);
        void Warn(Exception ex, string message, params object[] arg);
        void Error(Exception ex, string message, params object[] arg);

        void Trace(Func<string> messageGenerator);
        void Debug(Func<string> messageGenerator);
        void Info(Func<string> messageGenerator);
        void Warn(Func<string> messageGenerator);
        void Error(Func<string> messageGenerator);
    }

     
    

}
