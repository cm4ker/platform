using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOrderBy
    {
        public List<QExpression> Expressions { get; }

        public QOrderBy(List<QExpression> expressions)
        {
            Expressions = expressions;
        }
    }
}