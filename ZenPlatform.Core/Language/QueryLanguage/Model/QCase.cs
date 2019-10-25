using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Кейс
    /// </summary>
    public class QCase : QOperationExpression
    {
        protected override int ParamCount => 3;

        public QExpression When => Arguments[0];
        public QExpression Then => Arguments[1];
        public QExpression Else => Arguments[2];

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            foreach (var typeBase in Then.GetRexpressionType())
            {
                yield return typeBase;
            }

            foreach (var typeBase in Else.GetRexpressionType())
            {
                yield return typeBase;
            }
        }
    }
}