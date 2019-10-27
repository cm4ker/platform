using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public class QSelectExpression : QField
    {
        public QSelectExpression(QExpression expression)
        {
            Child = expression;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Child.GetRexpressionType();
        }
    }

    /// <summary>
    /// Выражение, основанное на конечном поле данных
    /// </summary>
    public class QSourceFieldExpression : QField
    {
        public QSourceFieldExpression(XCObjectPropertyBase property)
        {
            Property = property;
        }

        public XCObjectPropertyBase Property { get; }
    }
}