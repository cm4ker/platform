using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QGroupBy : QItem
    {
        public List<QExpression> Expressions { get; }

        public QGroupBy(List<QExpression> expressions)
        {
            Expressions = expressions;
        }
    }
}