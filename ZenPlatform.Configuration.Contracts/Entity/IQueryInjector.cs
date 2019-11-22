using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Configuration.Contracts.Entity
{
    /// <summary>
    /// Интерфейс обязательный для реализации, если мы хотим, чтобы компонент учавствовал в запросах
    /// </summary>
    public interface IQueryInjector
    {
        /// <summary>
        /// Получить фрагмент источника данных
        /// </summary>
        /// <param name="logicalTreeNode">Элемент логического дерева, связанный с данным источником данных</param>
        /// <returns></returns>
        void InjectDataSource(QueryMachine qm, IXCObjectProperty t, IQueryModelContext logicalTreeNode);
    }
}