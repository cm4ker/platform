using System.Collections.Generic;

namespace Aquila.Core.Contracts.Environment
{
    /// <summary>
    /// Менеджер среды 
    /// </summary>
    public interface IPlatformEnvironmentManager
    {
        IEnvironment GetEnvironment(string name);

        void AddWorkEnvironment(IStartupConfig config);

        List<IEnvironment> GetEnvironmentList();
    }
}