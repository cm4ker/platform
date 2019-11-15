using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public partial class QExpression : QItem
    {
        public virtual IEnumerable<XCTypeBase> GetExpressionType()
        {
            yield break;
        }
    }
}