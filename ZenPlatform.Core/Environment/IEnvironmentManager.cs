using System.Collections.Generic;

namespace ZenPlatform.Core.Environment
{
    public interface IEnvironmentManager
    {
        IEnvironment GetEnvironment(string name);

        void AddWorkEnvironment(StartupConfig config);
        List<IEnvironment> GetEnvironmentList();
    }
}