using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aquila.Compiler;
using Aquila.Core.ClientServices;
using Aquila.Core.Serialisers;

namespace Aquila.Core.Assemlies
{
    public interface IAssemblyStorage
    {
        void SaveAssembly(AssemblyDescription description, byte[] blob);
        void RemoveAssembly(string hash);

        void Clear();
        
        IEnumerable<AssemblyDescription> GetAssemblies(string configurationHash);
        byte[] GetAssembly(AssemblyDescription description);
        byte[] GetAssembly(string configurationHash, string name);

    }
    public class FileAssemblyStorage : IAssemblyStorage
    {
        private string _storagePath;
        private ISerializer _serializer;
        private const string FILE_DESCRIPTION_EXT = ".desc";
        private const string FILE_ASSEMBLY_EXT = ".assembly";
        public FileAssemblyStorage(ISerializer serializer)
        {
            _serializer = serializer;

            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        private IEnumerable<AssemblyDescription> GetAssemblies()
        {
            return Directory.GetFiles(_storagePath, string.Concat("*", FILE_DESCRIPTION_EXT), SearchOption.TopDirectoryOnly).Select(path =>
            _serializer.FromBytes<AssemblyDescription>(File.ReadAllBytes(path)));
        }

        private byte[] GetAssembly(string assemblyHash)
        {
            var assemFileName = Path.Combine(_storagePath, assemblyHash, FILE_ASSEMBLY_EXT);

            return File.ReadAllBytes(assemFileName);
        }

        public byte[] GetAssembly(string configurationHash, string name)
        {
            return GetAssembly(GetAssemblies().First(a => a.ConfigurationHash.Equals(configurationHash) && a.Name.Equals(name)));
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssemblyDescription> GetAssemblies(string configurationHash)
        {
            return GetAssemblies().Where(a => a.ConfigurationHash.Equals(configurationHash));
        }


        public byte[] GetAssembly(AssemblyDescription description)
        {
            return GetAssembly(description.AssemblyHash);
        }

        public void SaveAssembly(AssemblyDescription description, byte[] blob)
        {
            var descFileName = Path.Combine(_storagePath, description.AssemblyHash, FILE_DESCRIPTION_EXT);

            File.WriteAllBytes(descFileName, _serializer.ToBytes(description));

            var assemFileName = Path.Combine(_storagePath, description.AssemblyHash, FILE_ASSEMBLY_EXT);

            File.WriteAllBytes(assemFileName, blob);

        }

        public void RemoveAssembly(string hash)
        {
            throw new NotImplementedException();
        }
    }

}
