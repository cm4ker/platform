using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    public class QGroupBy : QItem
    {
        public List<QExpression> Expressions { get; }

        public QGroupBy(List<QExpression> expressions)
        {
            Expressions = expressions;
            foreach (var ex in expressions)
            {
                ex.Parent = this;
            }
        }
    }
}