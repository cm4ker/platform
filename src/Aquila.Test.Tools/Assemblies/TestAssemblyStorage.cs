using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Assemlies;

namespace Aquila.Test.Tools.Assemblies
{
    public class TestAssemblyStorage 
    {
        private Dictionary<FileDescriptor, byte[]> _assemblies;

        public TestAssemblyStorage()
        {
            _assemblies = new Dictionary<FileDescriptor, byte[]>();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
        //
        // public IEnumerable<FileDescriptor> GetAssemblies(string configurationHash)
        // {
        //     return _assemblies.Where(p => p.Key.ConfigurationHash == configurationHash).Select(p => p.Key);
        // }
        //
        // public byte[] GetAssembly(FileDescriptor descriptor)
        // {
        //     return _assemblies.First(p => p.Key.AssemblyHash.Equals(descriptor.AssemblyHash)).Value;
        // }
        //
        // public byte[] GetAssembly(string configurationHash, string name)
        // {
        //     return _assemblies.First(p => p.Key.ConfigurationHash.Equals(configurationHash) && p.Key.Name.Equals(name))
        //         .Value;
        // }

        public void SaveAssembly(FileDescriptor descriptor, byte[] blob)
        {
            _assemblies.Add(descriptor, blob);
        }

        public void RemoveAssembly(string hash)
        {
            throw new NotImplementedException();
        }
    }
}