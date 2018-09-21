using System.Collections.Generic;
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
        SqlFragment GetDataSourceFragment(DataQueryConstructorContext context);

        /// <summary>
        /// Получить фрагмент поля
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        SqlFragment GetColumnFragment(DataQueryConstructorContext context);
    }

    /// <summary>
    /// Фрагмент источника данных
    /// </summary>
    public class SqlFragment
    {
        public SqlFragment(SqlNode sqlNode, Dictionary<string, object> parameters)
        {
            SqlNode = sqlNode;

            if (parameters == null)
                Parameters = new Dictionary<string, object>();
            else
                Parameters = parameters;
        }

        /// <summary>
        /// Кусок SQL который будет инжектирован
        /// </summary>
        public SqlNode SqlNode { get; }

        /// <summary>
        /// Если источник данных каким-либо образом зависит от параметров, в таком случае мы можем указать параметры, которые будут внутри запроса
        /// </summary>
        public Dictionary<string, object> Parameters { get; }
    }


    /// <summary>
    /// Контекст конструктора базы данных
    /// </summary>
    public class DataQueryConstructorContext
    {
        public DataQueryConstructorContext()
        {
            NodeStack = new Stack<SqlNode>();
        }

        /// <summary>
        /// Имя объекта
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Имя поля
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Stack<SqlNode> NodeStack { get; }

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