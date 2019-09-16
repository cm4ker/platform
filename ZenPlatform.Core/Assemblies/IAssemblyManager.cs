using System.Collections.Generic;
using System.IO;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Core.Assemblies
{
    public interface IAssemblyManager
    {
        void BuildConfiguration(XCRoot configuration);

        void CheckConfiguration(XCRoot configuration);
    }
}