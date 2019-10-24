using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Кейс
    /// </summary>
    public class LTCase : LTOperationExpression
    {
        protected override int ParamCount => 3;

        public LTExpression When => Arguments[0];
        public LTExpression Then => Arguments[1];
        public LTExpression Else => Arguments[2];

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