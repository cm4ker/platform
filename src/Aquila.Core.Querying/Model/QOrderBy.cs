using System.Collections.Generic;

namespace Aquila.Core.Querying.Model
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