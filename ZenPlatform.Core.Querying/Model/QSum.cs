using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Сумма
    /// </summary>
    public class QSum : QExpression
    {
        private readonly IType _baseType;

        public QSum(IType baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<IType> GetExpressionType()
        {
            yield return _baseType;
        }
    }
}