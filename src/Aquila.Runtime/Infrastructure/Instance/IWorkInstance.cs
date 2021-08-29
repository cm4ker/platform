namespace Aquila.Core.Contracts.Instance
{
    /// <summary>
    ///  Рабочая среда обеспечивает доступ пользователя к контексту какого-то прикладного решения
    /// </summary>
    public interface IWorkInstance : IPlatformInstance
    {
        ILinkFactory LinkFactory { get; }
    }
}