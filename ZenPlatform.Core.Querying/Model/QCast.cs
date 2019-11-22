using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCast
    {
        public override IEnumerable<IXCType> GetExpressionType()
        {
            yield return Type;
        }

        public override string ToString()
        {
            return $"Cast: {string.Join(',', BaseExpression.GetExpressionType().Select(x => x.Name))} -> {Type.Name}";
        }
    }
}