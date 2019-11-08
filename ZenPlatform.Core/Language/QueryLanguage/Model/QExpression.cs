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
        /// Родительский узел с точки зрения SQL графа
        /// </summary>
        public QItem Parent { get; set; }

        /// <summary>
        /// Дочерний узел с точки зрения SQL графа
        /// </summary>
        public QItem Child { get; set; }

        public virtual IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield break;
        }
    }
    
    
}