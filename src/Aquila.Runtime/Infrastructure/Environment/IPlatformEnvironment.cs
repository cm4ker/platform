using Aquila.Data;

namespace Aquila.Core.Contracts.Environment
{
    public interface IPlatformEnvironment : IInitializibleEnvironment<IStartupConfig>
    {
        /// <summary>
        /// Manager of data connection contexts
        /// </summary>
        DataContextManager DataContextManager { get; }
    }
}