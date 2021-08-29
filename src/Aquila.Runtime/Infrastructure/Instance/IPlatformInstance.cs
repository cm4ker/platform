using Aquila.Data;

namespace Aquila.Core.Contracts.Instance
{
    public interface IPlatformInstance : IInitializableInstance<IStartupConfig>
    {
        /// <summary>
        /// Manager of data connection contexts
        /// </summary>
        DataContextManager DataContextManager { get; }
    }
}