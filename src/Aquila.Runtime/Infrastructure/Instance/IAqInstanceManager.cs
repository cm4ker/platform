using System.Collections.Generic;

namespace Aquila.Core.Instance
{
    public interface IAqInstanceManager
    {
        void AddInstance(StartupConfig config);
        AqInstance TryGetInstance(string name);
        IEnumerable<AqInstance> GetInstances();
    }
}