using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCombinedDataSource : QDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= DataSources.Where(x => !(x is QAliasedDataSource)).SelectMany(x => x.GetFields())
                .ToList();
        }
    }
}