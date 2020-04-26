using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.Core.Test.Logging
{
    public class XUnitLogger<T> : ILogger<T>
    {
        private ITestOutputHelper _testOutput;
        public XUnitLogger(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        public bool IsTrace => true;

        public bool IsDebug => true;

        public bool IsInfo => true;

        public bool IsWarn => true;

        public bool IsError => true;

        public void Debug(string message, params object[] arg)
        {
            _testOutput.WriteLine(message, arg);
        }

        public void Debug(Exception ex, string message, params object[] arg)
        {
            _testOutput.WriteLine("Exception: {0}", ex);
            _testOutput.WriteLine(message, arg);
        }

        public void Debug(Func<string> messageGenerator)
        {
            _testOutput.WriteLine(messageGenerator());
        }

        public void Error(string message, params object[] arg)
        {
            _testOutput.WriteLine(message, arg);
        }

        public void Error(Exception ex, string message, params object[] arg)
        {
            _testOutput.WriteLine("Exception: {0}", ex);
            _testOutput.WriteLine(message, arg);
        }

        public void Error(Func<string> messageGenerator)
        {
            _testOutput.WriteLine(messageGenerator());
        }

        public void Info(string message, params object[] arg)
        {
            _testOutput.WriteLine(message, arg);
        }

        public void Info(Exception ex, string message, params object[] arg)
        {
            _testOutput.WriteLine("Exception: {0}", ex);
            _testOutput.WriteLine(message, arg);
        }

        public void Info(Func<string> messageGenerator)
        {
            _testOutput.WriteLine(messageGenerator());
        }

        public void Trace(string message, params object[] arg)
        {
            _testOutput.WriteLine(message, arg);
        }

        public void Trace(Exception ex, string message, params object[] arg)
        {
            _testOutput.WriteLine("Exception: {0}", ex);
            _testOutput.WriteLine(message, arg);
        }

        public void Trace(Func<string> messageGenerator)
        {
            _testOutput.WriteLine(messageGenerator());
        }

        public void Warn(string message, params object[] arg)
        {
            _testOutput.WriteLine(message, arg);
        }

        public void Warn(Exception ex, string message, params object[] arg)
        {
            _testOutput.WriteLine("Exception: {0}", ex);
            _testOutput.WriteLine(message, arg);
        }

        public void Warn(Func<string> messageGenerator)
        {
            _testOutput.WriteLine(messageGenerator());
        }
    }
}
