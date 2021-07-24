using System.Collections.Generic;
using System.IO;
using Aquila.Core.Assemlies;

namespace Aquila.Core.ClientServices
{
    public interface IAssemblyManagerClientService
    {
        List<AssemblyDescription> GetDiffAssemblies(List<AssemblyDescription> assemblies);

        Stream GetAssembly(AssemblyDescription assembly);
    }
}