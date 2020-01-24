using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public partial class QExpression : QItem
    {
        public virtual IEnumerable<IType> GetExpressionType()
        {
            yield break;
        }
    }
}