using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.ClientServices
{
    public class AssemblyManagerClientService: IAssemblyManagerClientService
    {
        private readonly IEnvironment _environment;
        public AssemblyManagerClientService(IWorkEnvironment environment)
        {
            _environment = environment;
        }
        public List<AssemblyDescription> GetDiffAssemblies(List<AssemblyDescription> assemblies)
        {
            var lastAssem = ConfigurationTools.GetLastAssemblies(_environment.DataContextManager.SqlCompiler, _environment.DataContextManager.GetContext());
            return lastAssem.Where(a => assemblies.FirstOrDefault(s => s.Name.Equals(a.Name) && s.AssemblyHash.Equals(a.AssemblyHash)) is null).ToList();

        }

        public Stream GetAssembly(AssemblyDescription assembly)
        {
            return ConfigurationTools.GetAssemblyByDescription(assembly, _environment.DataContextManager.SqlCompiler, 
                _environment.DataContextManager.GetContext());
                
        }
    }
}
