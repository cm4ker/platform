using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCombinedDataSource : QDataSource
    {
        private List<QField> _fields;

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= DataSources
                .SelectMany(x => x.GetFields())
               // .Select()
                .ToList();
        }
    }
}