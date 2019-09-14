﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler;
using ZenPlatform.Core.Assemlies;

namespace ZenPlatform.Core.Test.Assemblies
{
    public class TestAssemblyStorage : IAssemblyStorage
    {

        private Dictionary<AssemblyDescription, byte[]> _assemblies;
        public TestAssemblyStorage()
        {
            _assemblies = new Dictionary<AssemblyDescription, byte[]>();
        }
        public IEnumerable<AssemblyDescription> GetAssemblies(string configurationHash)
        {
            return _assemblies.Where(p => p.Key.ConfigurationHash == configurationHash).Select(p => p.Key);
        }

        public byte[] GetAssembly(AssemblyDescription description)
        {
            return _assemblies.First(p=>p.Key.AssemblyHash.Equals(description.AssemblyHash)).Value;
        }

        public byte[] GetAssembly(string configurationHash, string name)
        {
            return _assemblies.First(p => p.Key.ConfigurationHash.Equals(configurationHash) && p.Key.Name.Equals(name)).Value;
        }

        public void SaveAssembly(AssemblyDescription description, byte[] blob)
        {
            _assemblies.Add(description, blob);
        }
    }
}
