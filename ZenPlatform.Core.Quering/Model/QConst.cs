using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Константное значение
    /// </summary>
    public class QConst : QExpression
    {
        private readonly XCTypeBase _baseType;


        public QConst(XCTypeBase baseType, object value)
        {
            Value = value;
            _baseType = baseType;
        }

        public object Value { get; }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }
}