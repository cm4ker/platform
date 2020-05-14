using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using Aquila.Core.Logging;

namespace Aquila.Core.Logging
{
    public class NLogger<T> : AbstractLogger, ILogger<T>
    {
        private readonly Logger _nLogger;

        public NLogger()
        {
            _nLogger = NLog.LogManager.GetLogger(typeof(T).FullName);
        }

        public override bool IsTrace => _nLogger.IsTraceEnabled;

        public override bool IsDebug => _nLogger.IsDebugEnabled;

        public override bool IsInfo => _nLogger.IsInfoEnabled;

        public override bool IsWarn => _nLogger.IsWarnEnabled;

        public override bool IsError => _nLogger.IsErrorEnabled;

        public override void Debug(string message, params object[] arg)
        {
            _nLogger.Debug(message, arg);
        }

        public override void Debug(Exception ex, string message, params object[] arg)
        {
            _nLogger.Debug(ex, message, arg);
        }

        public override void Error(string message, params object[] arg)
        {
            _nLogger.Error(message, arg);
        }

        public override void Error(Exception ex, string message, params object[] arg)
        {
            _nLogger.Error(ex, message, arg);
        }

        public override void Info(string message, params object[] arg)
        {
            _nLogger.Info(message, arg);
        }

        public override void Info(Exception ex, string message, params object[] arg)
        {
            _nLogger.Info(ex, message, arg);
        }

        public override void Trace(string message, params object[] arg)
        {
            _nLogger.Trace(message, arg);
        }

        public override void Trace(Exception ex, string message, params object[] arg)
        {
            _nLogger.Trace(ex, message, arg);
        }

        public override void Warn(string message, params object[] arg)
        {
            _nLogger.Warn(message, arg);
        }

        public override void Warn(Exception ex, string message, params object[] arg)
        {
            _nLogger.Warn(ex, message, arg);
        }
    }
}
