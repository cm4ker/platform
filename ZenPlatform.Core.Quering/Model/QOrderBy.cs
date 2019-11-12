using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    public class QOrderBy
    {
        public List<QExpression> Expressions { get; }

        public QOrderBy(List<QExpression> expressions)
        {
            Expressions = expressions;
        }
    }
}