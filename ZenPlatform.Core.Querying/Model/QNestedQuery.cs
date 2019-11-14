using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public partial class QNestedQuery : QDataSource
    {
        public override IEnumerable<QField> GetFields()
        {
            foreach (var prop in Nested.Select.Fields)
            {
                yield return new QIntermediateSourceField(prop, this);
            }
        }

        public override string? ToString()
        {
            return "Nested Query";
        }
    }
}