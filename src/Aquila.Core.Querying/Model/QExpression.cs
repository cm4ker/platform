using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public partial class QExpression : QItem
    {
        public virtual IEnumerable<IPType> GetExpressionType()
        {
            yield break;
        }
    }
}