using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public partial class QNestedQuery : QDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= Nested.Select.Fields.Select(x => (QField)new QIntermediateSourceField(x, this)).ToList();
        }

        public override string? ToString()
        {
            return "Nested Query";
        }
    }
}