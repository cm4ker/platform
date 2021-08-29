using System.Collections.Generic;
using System.IO;
using Aquila.Core.Assemlies;

namespace Aquila.Core.ClientServices
{
    public interface IAssemblyManagerClientService
    {
        List<AssemblyDescriptor> GetDiffAssemblies(List<AssemblyDescriptor> assemblies);

        Stream GetAssembly(AssemblyDescriptor assembly);
    }
}