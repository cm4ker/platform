using System.Collections.Generic;

namespace ZenPlatform.Core.Environment
{
    public interface IEnvironmentManager
    {
        IEnvironment GetEnvironment(string name);
        List<IEnvironment> GetEnvironmentList();
    }
}