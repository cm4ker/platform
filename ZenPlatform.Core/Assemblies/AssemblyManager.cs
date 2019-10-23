using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DML.Insert;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Core.Logging;


namespace ZenPlatform.Core.Assemblies
{
    public class AssemblyManager : IAssemblyManager
    {
        private IAssemblyStorage _assemblyStorage;
        private IXCCompiller _compiller;
        private ILogger _logger;

        public AssemblyManager(IXCCompiller compiller, IAssemblyStorage assemblyStorage,
            ILogger<AssemblyManager> logger)
        {
            _assemblyStorage = assemblyStorage;
            _compiller = compiller;
            _logger = logger;
        }

        public void CheckConfiguration(XCRoot configuration)
        {
            var hash = HashHelper.HashMD5(configuration.SerializeToStream());

            var assemblies = _assemblyStorage.GetAssemblies(hash);

            if (assemblies.FirstOrDefault(a =>
                    a.Name.Equals($"{configuration.ProjectName}_server")
                    || a.Name.Equals($"{configuration.ProjectName}_client")) == null)
            {
                BuildConfiguration(configuration);
            }
        }

        public IEnumerable<AssemblyDescription> GetAssemblies(XCRoot conf)
        {
            return _assemblyStorage.GetAssemblies(conf.GetHash());
        }

        public byte[] GetAssemblyBytes(AssemblyDescription description)
        {
            return _assemblyStorage.GetAssembly(description);
        }

        public void BuildConfiguration(XCRoot configuration)
        {
            _logger.Info("Build configuration.");
            var assembly = _compiller.Build(configuration, CompilationMode.Server);

            var stream = new MemoryStream();
            assembly.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var description = new AssemblyDescription()
            {
                AssemblyHash = HashHelper.HashMD5(stream),
                ConfigurationHash = configuration.GetHash(),
                Name = assembly.Name,
                Type = AssemblyType.Server,
            };

            _assemblyStorage.SaveAssembly(description, stream.ToArray());

            assembly = _compiller.Build(configuration, CompilationMode.Client);

            var clientStream = new MemoryStream();
            assembly.Write(clientStream);
            clientStream.Seek(0, SeekOrigin.Begin);
            description = new AssemblyDescription()
            {
                AssemblyHash = HashHelper.HashMD5(clientStream),
                ConfigurationHash = configuration.GetHash(),
                Name = assembly.Name,
                Type = AssemblyType.Client,
            };

            _assemblyStorage.SaveAssembly(description, clientStream.ToArray());
        }
    }
}