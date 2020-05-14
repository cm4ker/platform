using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Сумма
    /// </summary>
    public class QSum : QExpression
    {
        private readonly IPType _baseIpType;

        public QSum(IPType baseIpType)
        {
            _baseIpType = baseIpType;
        }

        public override IEnumerable<IPType> GetExpressionType()
        {
            yield return _baseIpType;
        }
    }
}