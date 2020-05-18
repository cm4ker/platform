using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;


namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Константное значение
    /// </summary>
    public partial class QConst : QExpression
    {
        private readonly IPType _baseIpType;


        public QConst(IPType baseIpType, object value)
        {
            Value = value;
            _baseIpType = baseIpType;
        }

        public object Value { get; }

        public override IEnumerable<IPType> GetExpressionType()
        {
            yield return _baseIpType;
        }

        public override string ToString()
        {
            return $"Const = {Value}";
        }
    }
}