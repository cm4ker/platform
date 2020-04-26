using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Contracts.Environment
{
    public interface IPlatformEnvironment : IInitializibleEnvironment<IStartupConfig>
    {
        /// <summary>
        /// Менеджер доступа к данным
        /// </summary>
        IDataContextManager DataContextManager { get; }

        IProject Configuration { get; }
    }
}