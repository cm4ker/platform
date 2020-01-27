using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Core.Environment.Contracts
{
    /// <summary>
    ///  Рабочая среда обеспечивает доступ пользователя к контексту какого-то прикладного решения
    /// </summary>
    public interface IWorkEnvironment : IPlatformEnvironment
    {
        IRoot Configuration { get; }
    }
}