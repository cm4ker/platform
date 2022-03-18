using System.Collections.Generic;
using System.IO;
using Aquila.Core.Assemlies;

namespace Aquila.Core.ClientServices
{
    public interface IAssemblyManagerClientService
    {
        List<FileDescriptor> GetDiffAssemblies(List<FileDescriptor> assemblies);

        Stream GetAssembly(FileDescriptor file);
    }
}