

namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Заппрос
    /// </summary>
    public class QQuery : QItem
    {
        public QQuery(QOrderBy orderBy, QSelect select, QHaving having, QGroupBy groupBy, QWhere where, QFrom from)
        {
            From = from;
            Select = select;
            Where = where;
            OrderBy = orderBy;
            Having = having;
            GroupBy = groupBy;
        }

        /// <summary>
        /// Список выбранных полей
        /// </summary>
        public QSelect Select { get; }


        /// <summary>
        /// Список выбранных таблиц
        /// </summary>
        public QFrom From { get; }

        /// <summary>
        /// Список наложенной фильтрации
        /// </summary>
        public QWhere Where { get; }

        /// <summary>
        /// Список сгруппировнных данных
        /// </summary>
        public QGroupBy GroupBy { get; }

        /// <summary>
        /// Список наложенной фильтрации на группы
        /// </summary>
        public QHaving Having { get; }

        /// <summary>
        /// Список полей сортировки
        /// </summary>
        public QOrderBy OrderBy { get; }
    }
}