using System.Collections.Generic;
using ZenPlatform.Core.Querying.Visitor;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QDataRequest : QItem
    {
        public QDataRequest(List<QField> source)
        {
            Source = source;
        }

        public List<QField> Source { get; }
        
        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}