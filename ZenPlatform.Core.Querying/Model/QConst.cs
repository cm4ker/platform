using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;


namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Константное значение
    /// </summary>
    public partial class QConst : QExpression
    {
        private readonly IXCType _baseType;


        public QConst(IXCType baseType, object value)
        {
            Value = value;
            _baseType = baseType;
        }

        public object Value { get; }

        public override IEnumerable<IXCType> GetExpressionType()
        {
            yield return _baseType;
        }
    }
}