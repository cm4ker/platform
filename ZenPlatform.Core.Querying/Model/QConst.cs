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
        private readonly IType _baseType;


        public QConst(IType baseType, object value)
        {
            Value = value;
            _baseType = baseType;
        }

        public object Value { get; }

        public override IEnumerable<IType> GetExpressionType()
        {
            yield return _baseType;
        }
    }
}