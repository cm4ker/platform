using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Поле объекта имеет конкретную привязку к конкретному объекту
    /// </summary>
    public class LTObjectField : LTExpression
    {
        public LTObjectField(XCObjectPropertyBase property)
        {
            Property = property;
        }

        public XCObjectPropertyBase Property { get; set; }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Property.Types;
        }
    }
}