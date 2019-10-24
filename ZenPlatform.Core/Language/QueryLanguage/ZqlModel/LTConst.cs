using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Константное значение
    /// </summary>
    public class LTConst : LTExpression
    {
        private readonly XCTypeBase _baseType;

        public LTConst(XCTypeBase baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }
}