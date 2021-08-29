using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Assemlies;

namespace Aquila.Test.Tools.Assemblies
{
    public class TestAssemblyStorage : IAssemblyStorage
    {
        private Dictionary<AssemblyDescriptor, byte[]> _assemblies;

        public TestAssemblyStorage()
        {
            _assemblies = new Dictionary<AssemblyDescriptor, byte[]>();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssemblyDescriptor> GetAssemblies(string configurationHash)
        {
            return _assemblies.Where(p => p.Key.ConfigurationHash == configurationHash).Select(p => p.Key);
        }

        public byte[] GetAssembly(AssemblyDescriptor descriptor)
        {
            return _assemblies.First(p => p.Key.AssemblyHash.Equals(descriptor.AssemblyHash)).Value;
        }

        public byte[] GetAssembly(string configurationHash, string name)
        {
            return _assemblies.First(p => p.Key.ConfigurationHash.Equals(configurationHash) && p.Key.Name.Equals(name))
                .Value;
        }

        public void SaveAssembly(AssemblyDescriptor descriptor, byte[] blob)
        {
            _assemblies.Add(descriptor, blob);
        }

        public void RemoveAssembly(string hash)
        {
            throw new NotImplementedException();
        }
    }
}