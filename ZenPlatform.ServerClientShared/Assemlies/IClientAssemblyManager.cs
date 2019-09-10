using System.IO;

namespace ZenPlatform.Core.Assemlies
{
    public interface IClientAssemblyManager
    {
        Stream GetAssembly(string name);
        void UpdateAssemblyes();
    }
}