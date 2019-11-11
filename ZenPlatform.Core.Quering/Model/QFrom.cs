using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Представляет собой логическую структуру запроса части запроса FROM
    /// </summary>
    public class QFrom : QItem
    {
        public QFrom(IEnumerable<QFromItem> joins, IQDataSource source)
        {
            Joins = joins;
            Source = source;
        }

        public IQDataSource Source { get; }

        public IEnumerable<QFromItem> Joins { get; }
    }
}