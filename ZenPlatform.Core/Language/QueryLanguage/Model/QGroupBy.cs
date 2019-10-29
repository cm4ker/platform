using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QGroupBy
    {
        public List<QExpression> Expressions { get; }

        public QGroupBy(List<QExpression> expressions)
        {
            Expressions = expressions;
        }
    }
}