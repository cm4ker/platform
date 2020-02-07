using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Configuration.Contracts.Data.Entity
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
        void InjectDataSource(QueryMachine qm, IPType t, IQueryModelContext logicalTreeNode);
    }


    /// <summary>
    /// Контракт контекста модели запроса
    /// </summary>
    public interface IQueryModelContext
    {
        /// <summary>
        /// Параметры, которые идут с контекстом.
        /// Компонент, которому будет передано управление, сможет также их анализировать
        /// </summary>
        Dictionary<string, object> Parameters { get; set; }
    }

    /// <summary>
    /// Фрагмент источника данных
    /// </summary>
    public class SqlFragment
    {
//        public SqlFragment(SqlNode sqlNode, Dictionary<string, object> parameters)
//        {
//            SqlNode = sqlNode;
//
//            if (parameters == null)
//                Parameters = new Dictionary<string, object>();
//            else
//                Parameters = parameters;
//        }
//
//        /// <summary>
//        /// Кусок SQL который будет инжектирован
//        /// </summary>
//        public SqlNode SqlNode { get; }

        /// <summary>
        /// Если источник данных каким-либо образом зависит от параметров, в таком случае мы можем указать параметры, которые будут внутри запроса
        /// </summary>
        public Dictionary<string, object> Parameters { get; }
    }

    /// <summary>
    /// Контекст конструктора базы данных.
    /// </summary>
    public class DataQueryConstructorContext
    {
        public DataQueryConstructorContext()
        {
        }

        public List<object> CurrentStateParameters { get; set; }

        /// <summary>
        /// Текущий контекст параметров запроса
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Получить имя следующего параметра
        /// </summary>
        /// <returns></returns>
        string GetNextParamenterName()
        {
            return "";
        }
    }
}