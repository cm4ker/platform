using System.Collections.Generic;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
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

        public override IEnumerable<IXCType> GetExpressionType()
        {
            yield return _baseType;
        }
    }
}