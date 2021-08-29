using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aquila.Core.ClientServices;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Crypto;

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


        private IEnumerable<AssemblyDescriptor> LoadCacheDescription()
        {
            foreach (var filePath in Directory.GetFiles(CashPath))
            {
                using (var file = new FileStream(filePath, FileMode.Open))
                {
                    yield return new AssemblyDescriptor()
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

        private void Download(AssemblyDescriptor assemblyDescriptor)
        {
            using (var stream = _service.GetAssembly(assemblyDescriptor))
            {
                using (var file = new FileStream(Path.Combine(CashPath, assemblyDescriptor.Name), FileMode.Create))
                {
                    stream.CopyTo(file);
                }
            }
        }
    }
}