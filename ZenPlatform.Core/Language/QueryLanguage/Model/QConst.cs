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

        public QConst(XCTypeBase baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }
}