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
        public QItem Parent { get; set; }

        /// <summary>
        /// Выражение выборки
        /// </summary>
        public QItem Child { get; set; }

        public virtual IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield break;
        }
    }

    public class QParameter : QExpression
    {
        public string Name { get; }

        public QParameter(string name)
        {
            Name = name;
        }
    }
}