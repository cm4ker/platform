using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public partial class QNestedQuery : QDataSource
    {
        public QNestedQuery(QQuery nested)
        {
            Nested = nested;
        }

        public QQuery Nested { get; }

        public override IEnumerable<QField> GetFields()
        {
            foreach (var prop in Nested.Select.Fields)
            {
                yield return prop;
            }
        }
    }
}