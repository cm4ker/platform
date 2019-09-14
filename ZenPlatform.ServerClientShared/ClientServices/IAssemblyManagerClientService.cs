using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Compiler;

namespace ZenPlatform.Core.ClientServices
{
    public interface IAssemblyManagerClientService
    {
        List<AssemblyDescription> GetDiffAssemblies(List<AssemblyDescription> assemblies);

        Stream GetAssembly(AssemblyDescription assembly);
    }
}
