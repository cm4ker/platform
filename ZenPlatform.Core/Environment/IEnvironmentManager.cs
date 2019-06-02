using System.Collections.Generic;

namespace ZenPlatform.Core.Environment
{
    public interface IEnvironmentManager
    {
        void CreateEnvironment(StartupConfig config);
        IEnvironment GetEnvironment(string name);
        List<string> GetEnvironmentList();
    }
}