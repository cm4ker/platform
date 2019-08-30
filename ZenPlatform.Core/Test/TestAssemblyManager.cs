using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Crypto;

namespace ZenPlatform.Core.Test
{
    public class TestAssemblyManager : IAssemblyManager
    {
        private IXCCompiller _compiller;
        private IDictionary<string, Stream> _assemblies;
        
        private List<AssemblyDescription> _assemblyDescriptions;
        public TestAssemblyManager(IXCCompiller compiller)
        {
            _compiller = compiller;
            _assemblies = new Dictionary<string, Stream>();
            _assemblyDescriptions = new List<AssemblyDescription>();
        }
        public void BuildConfiguration(XCRoot configuration)
        {
            _assemblies = _compiller.Build(configuration);
            var random = new Random();

            _assemblyDescriptions = _assemblies.Select(a =>
            new AssemblyDescription()
            {
                Id = random.Next(),
                Name = a.Key,
                AssemblyHash = HashHelper.HashMD5(a.Value),
                ConfigurationHash = HashHelper.HashMD5(configuration.SerializeToStream()),
                CreateDataTime = DateTime.Now
            }
                ).ToList();
        }

        public Stream GetAssemblyByDescription(AssemblyDescription description)
        {
            var stream = _assemblies.First(a => a.Key.Equals(description.Name)).Value;
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public Stream GetAssemblyById(int id)
        {
            throw new NotImplementedException();
        }

        public List<AssemblyDescription> GetLastAssemblies()
        {
            return _assemblyDescriptions;
        }

        public Stream GetLastAssemblyByName(string name)
        {
            if (_assemblies.ContainsKey(name))
            {
                var stream = _assemblies[name];
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            return null;
        }

        public AssemblyDescription GetLastAssemblyDescriptionByName(string name)
        {
            return _assemblyDescriptions.FirstOrDefault(a => a.Name.Equals(name));
        }

        public void SaveAssembly(string name, string configurationHash, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
