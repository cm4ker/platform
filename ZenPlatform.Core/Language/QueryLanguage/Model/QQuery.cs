using System.Collections.Generic;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QWhere
    {
        public QExpression Expression { get; }

        public QWhere(QExpression expression)
        {
            Expression = expression;
        }
    }

    public class QGroupBy
    {
        public List<QExpression> Expressions { get; }

        public QGroupBy(List<QExpression> expressions)
        {
            Expressions = expressions;
        }
    }

    public class QHaving
    {
        public QExpression Expression { get; }

        public QHaving(QExpression expression)
        {
            Expression = expression;
        }
    }

    public class QOrderBy
    {
        public List<QExpression> Expressions { get; }

        public QOrderBy(List<QExpression> expressions)
        {
            Expressions = expressions;
        }
    }

    public class QSelect
    {
        public List<QField> Fields { get; }

        public QSelect(List<QField> fields)
        {
            Fields = fields;
        }
    }

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
        public List<QExpression> GroupBy { get; }

        /// <summary>
        /// Список наложенной фильтрации на группы
        /// </summary>
        public QExpression Having { get; }

        /// <summary>
        /// Список полей сортировки
        /// </summary>
        public List<QExpression> OrderBy { get; }
    }
}