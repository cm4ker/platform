using System.Collections.Generic;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    /// Менеджер среды 
    /// </summary>
    public interface IPlatformEnvironmentManager
    {
        IEnvironment GetEnvironment(string name);

        void AddWorkEnvironment(StartupConfig config);

        List<IEnvironment> GetEnvironmentList();
    }
}