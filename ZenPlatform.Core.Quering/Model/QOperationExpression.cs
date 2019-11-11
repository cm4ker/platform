using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Core.Quering.Model
{
    public class QOperationExpression : QExpression
    {
        public QOperationExpression()
        {
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return new XCBoolean();
        }
    }
}