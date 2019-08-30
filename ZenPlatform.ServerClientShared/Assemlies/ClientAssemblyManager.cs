using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Crypto;

namespace ZenPlatform.Core.Assemlies
{
    public class ClientAssemblyManager
    {
        IAssemblyManagerClientService _service;
        string _cachePath;

        public List<Assembly> Assemblies { get; private set; }
        public ClientAssemblyManager(IAssemblyManagerClientService service, string cachePath)
        {
            _service = service;
            if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);
            _cachePath = cachePath;
        }

        private IEnumerable<AssemblyDescription> LoadCacheDescription()
        {
            
            foreach (var filePath in Directory.GetFiles(_cachePath))
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

        public void UpdateAssemblyes()
        {

            var descriptions = LoadCacheDescription().ToList();


            var diff = _service.GetDiffAssemblies(descriptions);

            foreach (var desc in diff)
            {
                Download(desc);
            }


        }

        public void LoadAssemblyes()
        {
            Assemblies = Directory.GetFiles(_cachePath).Select(path => Assembly.LoadFile(path)).ToList();
            
        }

        private void Download(AssemblyDescription assemblyDescription)
        {
            using (var stream = _service.GetAssembly(assemblyDescription))
            {
                using (var file = new FileStream(Path.Combine(_cachePath, assemblyDescription.Name), FileMode.Create))
                {
                    stream.CopyTo(file);
                }
            }
        }


    }
}
