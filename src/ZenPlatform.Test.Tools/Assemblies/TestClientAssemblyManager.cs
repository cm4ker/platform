using System.IO;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Core.Assemlies;

namespace ZenPlatform.Test.Tools.Assemblies
{
    public class TestClientAssemblyManager : IClientAssemblyManager
    {
        private RoslynAssemblyBuilder _assembly;

        public TestClientAssemblyManager(RoslynAssemblyBuilder assembly)
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