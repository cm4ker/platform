using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration.Data.Contracts
{
    public interface IXCCompiller
    {
        IDictionary<string, Stream> Build(XCRoot root);
    }
}
