using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public class QExpression : QItem
    {
        /// <summary>
        /// Источник данных узла
        /// </summary>
        public QExpression Parent { get; set; }

        /// <summary>
        /// Выражение выборки
        /// </summary>
        public QExpression Child { get; set; }

        public virtual IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield break;
        }
    }
}