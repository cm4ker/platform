using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class QNastedQuery : QItem, IQDataSource
    {
        public QNastedQuery(QQuery nested)
        {
            Nested = nested;
        }

        public QQuery Nested;

        public IEnumerable<QField> GetFields()
        {
            foreach (var prop in Nested.Select)
            {
                yield return prop;
            }
        }
    }
}