using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Сумма
    /// </summary>
    public class LCSum : LTExpression
    {
        private readonly XCTypeBase _baseType;

        public LCSum(XCTypeBase baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }
}