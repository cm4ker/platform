using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.ClientServices
{
    public class AssemblyManagerClientService: IAssemblyManagerClientService
    {
        private readonly IAssemblyManager _assemblyManager;
        public AssemblyManagerClientService(IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;
        }
        public List<AssemblyDescription> GetDiffAssemblies(List<AssemblyDescription> assemblies)
        {
            var lastAssem = _assemblyManager.GetLastAssemblies();
            return lastAssem.Where(a => assemblies.FirstOrDefault(s => s.Name.Equals(a.Name) && s.AssemblyHash.Equals(a.AssemblyHash)) is null).ToList();

        }

        public Stream GetAssembly(AssemblyDescription assembly)
        {
            return _assemblyManager.GetAssemblyByDescription(assembly);
                
        }
    }
}
