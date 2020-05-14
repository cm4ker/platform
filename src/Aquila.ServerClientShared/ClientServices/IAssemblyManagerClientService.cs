using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aquila.Compiler;

namespace Aquila.Core.ClientServices
{
    public interface IAssemblyManagerClientService
    {
        List<AssemblyDescription> GetDiffAssemblies(List<AssemblyDescription> assemblies);

        Stream GetAssembly(AssemblyDescription assembly);
    }
}
