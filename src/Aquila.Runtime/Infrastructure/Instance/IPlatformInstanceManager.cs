using System.Collections.Generic;

namespace Aquila.Core.Contracts.Instance
{
    /// <summary>
    /// Менеджер среды 
    /// </summary>
    public interface IPlatformInstanceManager
    {
        IPlatformInstance GetInstance(string name);

        void AddInstance(IStartupConfig config);

        IEnumerable<IPlatformInstance> GetInstances();
    }
}