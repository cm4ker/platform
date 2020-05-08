using System.IO;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Assemlies;

namespace Aquila.Test.Tools.Assemblies
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