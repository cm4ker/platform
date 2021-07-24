using System.IO;

namespace Aquila.Core.Assemlies
{
    public interface IClientAssemblyManager
    {
        Stream GetAssembly(string name);
        void UpdateAssemblies();
    }
}