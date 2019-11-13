using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QDataSource : QItem
    {
        public virtual IEnumerable<QField> GetFields()
        {
            return null;
        }
    }
}