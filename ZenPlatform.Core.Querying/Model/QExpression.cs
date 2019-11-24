using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public partial class QExpression : QItem
    {
        public virtual IEnumerable<IXCType> GetExpressionType()
        {
            yield break;
        }
    }
}