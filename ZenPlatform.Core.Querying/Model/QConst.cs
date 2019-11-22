using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Константное значение
    /// </summary>
    public partial class QConst : QExpression
    {
        private readonly XCTypeBase _baseType;


        public QConst(XCTypeBase baseType, object value)
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