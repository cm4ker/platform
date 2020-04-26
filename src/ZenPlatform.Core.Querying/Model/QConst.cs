using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;


namespace ZenPlatform.Core.Querying.Model
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