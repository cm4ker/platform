using ZenPlatform.QueryBuilder.Common;

namespace ZenPlatform.Configuration.Data.Contracts.Entity
{
    /// <summary>
    /// Интерфейс обязательный для реализации, если мы хотим, чтобы компонент учавствовал в запросах
    /// </summary>
    public interface IQueryInjector
    {
        /// <summary>
        /// Получить фрагмент источника данных
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        SqlNode GetDataSourceFragment(string objectName);

        /// <summary>
        /// Получить фрагмент поля
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        SqlNode GetColumnFragment(string objectName, string fieldName);
    }
}