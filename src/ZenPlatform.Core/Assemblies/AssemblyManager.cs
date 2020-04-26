using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Core.Logging;


namespace ZenPlatform.Core.Assemblies
{
    public class AssemblyManager : IAssemblyManager
    {
        private IAssemblyStorage _assemblyStorage;
        private IXCCompiller _compiller;
        private ILogger _logger;
        private readonly IConfigurationManipulator _m;

        public AssemblyManager(IXCCompiller compiller, IAssemblyStorage assemblyStorage,
            ILogger<AssemblyManager> logger, IConfigurationManipulator m)
        {
            _assemblyStorage = assemblyStorage;
            _compiller = compiller;
            _logger = logger;
            _m = m;
        }


        public bool CheckConfiguration(IProject configuration)
        {
            //TODO need rewrite this part because now we usign VFS for store configuration and get MD5 we need from some object. Zip archive? 

            //Return always create new asssembly
            return true;

            var hash = HashHelper.HashMD5(_m.SaveToStream(configuration));

            var assemblies = _assemblyStorage.GetAssemblies(hash);

            if (assemblies.FirstOrDefault(a =>
                a.Name.Equals($"{configuration.ProjectName}_server")
                || a.Name.Equals($"{configuration.ProjectName}_client")) == null)
            {
                return true;
            }

            return false;
        }

        public IEnumerable<AssemblyDescription> GetAssemblies(IProject conf)
        {
            return _assemblyStorage.GetAssemblies(_m.GetHash(conf));
        }

        public byte[] GetAssemblyBytes(AssemblyDescription description)
        {
            return _assemblyStorage.GetAssembly(description);
        }

        public void BuildConfiguration(IProject configuration, SqlDatabaseType dbType)
        {
            _logger.Info("Build configuration.");
            var assembly = _compiller.Build(configuration, CompilationMode.Server, dbType);

            var stream = new MemoryStream();
            assembly.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var description = new AssemblyDescription()
            {
                AssemblyHash = HashHelper.HashMD5(stream),
                ConfigurationHash = _m.GetHash(configuration),
                Name = assembly.Name,
                Type = AssemblyType.Server,
            };

            _assemblyStorage.SaveAssembly(description, stream.ToArray());

            assembly = _compiller.Build(configuration, CompilationMode.Client, dbType);

            var clientStream = new MemoryStream();
            assembly.Write(clientStream);
            clientStream.Seek(0, SeekOrigin.Begin);
            description = new AssemblyDescription()
            {
                AssemblyHash = HashHelper.HashMD5(clientStream),
                ConfigurationHash = _m.GetHash(configuration),
                Name = assembly.Name,
                Type = AssemblyType.Client,
            };

            _assemblyStorage.SaveAssembly(description, clientStream.ToArray());
        }
    }
}