using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QDataRequest : QItem
    {
        public QDataRequest(FieldList source)
        {
            Source = source;
        }

        public FieldList Source { get; }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}