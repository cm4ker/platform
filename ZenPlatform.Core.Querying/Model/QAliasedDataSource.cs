using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ParentSource.GetFields().Select(x => (QField) new QIntermediateSourceField(x, this))
                .ToList();
        }

        public override string ToString()
        {
            return ParentSource + " AS " + Alias;
        }
    }
}