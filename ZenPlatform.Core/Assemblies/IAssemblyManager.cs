using System.Collections.Generic;
using System.IO;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemlies;

namespace ZenPlatform.Core.Assemblies
{
    public interface IAssemblyManager
    {
        void BuildConfiguration(XCRoot configuration);

        void CheckConfiguration(XCRoot configuration);

        IEnumerable<AssemblyDescription> GetAssemblies(XCRoot conf);

        byte[] GetAssemblyBytes(AssemblyDescription description);
    }
}