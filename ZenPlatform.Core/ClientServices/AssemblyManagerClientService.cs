using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Tools;

namespace ZenPlatform.Core.ClientServices
{
    public class AssemblyManagerClientService : IAssemblyManagerClientService
    {
        private readonly IAssemblyStorage _assemblyStorage;
        private readonly IWorkEnvironment _environment;
        private readonly IConfigurationManipulator _m;

        public AssemblyManagerClientService(IAssemblyStorage assemblyStorage, IWorkEnvironment environment,
            IConfigurationManipulator m)
        {
            _assemblyStorage = assemblyStorage;
            _environment = environment;
            _m = m;
        }

        public List<AssemblyDescription> GetDiffAssemblies(List<AssemblyDescription> assemblies)
        {
            var actualAssemblies = _assemblyStorage.GetAssemblies(_m.GetHash(_environment.Configuration));

            return actualAssemblies.Where(a => assemblies.FirstOrDefault(s =>
                                                       s.Name.Equals(a.Name) &&
                                                       s.AssemblyHash.Equals(a.AssemblyHash)) is
                                                   null && a.Type == AssemblyType.Client).ToList();
        }

        public Stream GetAssembly(AssemblyDescription assembly)
        {
            return new MemoryStream(_assemblyStorage.GetAssembly(assembly));
        }
    }
}