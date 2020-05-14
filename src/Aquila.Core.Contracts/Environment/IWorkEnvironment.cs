using Aquila.Configuration.Contracts;

namespace Aquila.Core.Contracts.Environment
{
    /// <summary>
    ///  Рабочая среда обеспечивает доступ пользователя к контексту какого-то прикладного решения
    /// </summary>
    public interface IWorkEnvironment : IPlatformEnvironment
    {
        ILinkFactory LinkFactory { get; }
    }
}