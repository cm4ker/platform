using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Сумма
    /// </summary>
    public class QSum : QExpression
    {
        private readonly XCTypeBase _baseType;

        public QSum(XCTypeBase baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }
}