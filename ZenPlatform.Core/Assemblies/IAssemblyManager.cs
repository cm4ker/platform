using System.Collections.Generic;
using System.IO;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemlies;

namespace ZenPlatform.Core.Assemblies
{
    public interface IAssemblyManager
    {
        Stream GetAssemblyByDescription(AssemblyDescription description);
        Stream GetAssemblyById(int id);
        List<AssemblyDescription> GetLastAssemblies();
        Stream GetLastAssemblyByName(string name);
        AssemblyDescription GetLastAssemblyDescriptionByName(string name);
        void SaveAssembly(string name, string configurationHash, Stream stream);
        void BuildConfiguration(XCRoot configuration);
    }
}