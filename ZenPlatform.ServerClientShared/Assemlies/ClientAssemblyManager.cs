using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ZenPlatform.Compiler;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.Core.Assemlies
{





    public class ClientAssemblyManager : IClientAssemblyManager
    {
        IAssemblyManagerClientService _service;
        private IProtocolClient _client;

        public string CashPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZenClient", _client.Database, "AssemblyCache");
            }
        }

        public ClientAssemblyManager(IAssemblyManagerClientService service, IProtocolClient client)
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
