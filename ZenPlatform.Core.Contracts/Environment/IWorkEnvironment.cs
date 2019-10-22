using ZenPlatform.Core.Environment.Contracts;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Environment
{
    public interface IPlatformEnvironment : IInitializibleEnvironment<IStartupConfig>
    {
        /// <summary>
        /// Менеджер доступа к данным
        /// </summary>
        IDataContextManager DataContextManager { get; }
    }
}