using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration.Data.Contracts
{
    public interface IXCCompiller
    {
        IAssembly Build(XCRoot configuration, CompilationMode mode);
    }
}
