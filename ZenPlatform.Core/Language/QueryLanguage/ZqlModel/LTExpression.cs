using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public class LTExpression : LTItem
    {
        /// <summary>
        /// Источник данных узла
        /// </summary>
        public LTExpression Parent { get; set; }

        /// <summary>
        /// Выражение выборки
        /// </summary>
        public LTExpression Child { get; set; }

        public virtual IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield break;
        }
    }
}