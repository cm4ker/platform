using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QDataRequest : QItem
    {
        public QDataRequest(List<QField> source)
        {
            Source = source;
        }

        public List<QField> Source { get; }
    }
}