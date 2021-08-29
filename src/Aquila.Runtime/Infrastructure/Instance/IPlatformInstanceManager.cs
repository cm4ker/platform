using System.Collections.Generic;

namespace Aquila.Core.Contracts.Instance
{
    /// <summary>
    /// Менеджер среды 
    /// </summary>
    public interface IPlatformInstanceManager
    {
        IInstance GetInstance(string name);

        void AddWorkInstance(IStartupConfig config);

        List<IInstance> GetInstanceList();
    }
}