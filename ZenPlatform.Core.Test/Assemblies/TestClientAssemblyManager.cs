using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Core.Assemlies;

namespace ZenPlatform.Core.Test.Assemblies
{
    public class TestClientAssemblyManager : IClientAssemblyManager
    {
        private IAssembly _assembly;

        public TestClientAssemblyManager(IAssembly assembly)
        {
            _assembly = assembly;
        }

        public Stream GetAssembly(string name)
        {
            if (_assembly != null && _assembly.Name == name)
            {
                var stream = new MemoryStream();
                _assembly.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            return new MemoryStream();
        }

        public void UpdateAssemblies()
        {
        }
    }
}