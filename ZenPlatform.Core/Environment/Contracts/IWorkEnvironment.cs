using ZenPlatform.Configuration.Structure;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Environment
{
    /// <summary>
    ///  Рабочая среда обеспечивает доступ пользователя к контексту какого-то прикладного решения
    /// </summary>
    public interface IWorkEnvironment : IPlatformEnvironment
    {
        XCRoot Configuration { get; }
    }

    public interface IPlatformEnvironment : IInitializibleEnvironment<StartupConfig>
    {
        /// <summary>
        /// Менеджер доступа к данным
        /// </summary>
        IDataContextManager DataContextManager { get; }
    }
}