using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aquila.Compiler;
using Aquila.Core.ClientServices;
using Aquila.Core.Crypto;
using Aquila.Core.Network.Contracts;

namespace Aquila.Core.Assemlies
{
    public class PlatformClientAssemblyManager : IClientAssemblyManager
    {
        IAssemblyManagerClientService _service;
        private IPlatformClient _client;

        public string CashPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "ZenClient",
                    _client.Database, "AssemblyCache");
            }
        }

        public PlatformClientAssemblyManager(IAssemblyManagerClientService service, IPlatformClient client)
        {
            _service = service;
            _client = client;

            if (!Directory.Exists(CashPath)) Directory.CreateDirectory(CashPath);
        }


        private IEnumerable<AssemblyDescription> LoadCacheDescription()
        {
            foreach (var filePath in Directory.GetFiles(CashPath))
            {
                using (var file = new FileStream(filePath, FileMode.Open))
                {
                    yield return new AssemblyDescription()
                    {
                        AssemblyHash = HashHelper.HashMD5(file),
                        Name = Path.GetFileName(filePath),
                    };
                }
            }
        }

        public void UpdateAssemblies()
        {
            var descriptions = LoadCacheDescription().ToList();
            var diff = _service.GetDiffAssemblies(descriptions);

            foreach (var desc in diff)
            {
                Download(desc);
            }
        }

        public Stream GetAssembly(string name)
        {
            return new FileStream(Path.Combine(CashPath, name), FileMode.Open);
        }

        private void Download(AssemblyDescription assemblyDescription)
        {
            using (var stream = _service.GetAssembly(assemblyDescription))
            {
                using (var file = new FileStream(Path.Combine(CashPath, assemblyDescription.Name), FileMode.Create))
                {
                    stream.CopyTo(file);
                }
            }
        }
    }
}