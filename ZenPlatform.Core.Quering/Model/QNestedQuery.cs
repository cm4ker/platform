using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class QNestedQuery : QItem, IQDataSource
    {
        public QNestedQuery(QQuery nested)
        {
            Nested = nested;
        }

        public QQuery Nested { get; }

        public IEnumerable<QField> GetFields()
        {
            foreach (var prop in Nested.Select.Fields)
            {
                yield return prop;
            }
        }
    }
}