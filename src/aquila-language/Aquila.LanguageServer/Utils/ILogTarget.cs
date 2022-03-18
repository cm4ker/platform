using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.LanguageServer.Utils
{
    interface ILogTarget
    {
        void LogMessage(string message);
    }
}
