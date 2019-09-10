using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZenPlatform.Core.Contracts
{
    public interface IClientInvoker
    {
        TResponce Invoke<TResponce>(Route route, params object[] args);
        Stream InvokeStream(Route route, params object[] args);
    }
}
