using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QIntermediateSourceField
    {
        public override string GetName()
        {
            return Field.GetName();
        }

        public override IEnumerable<XCTypeBase> GetExpressionType()
        {
            return Field.GetExpressionType();
        }

        public override string? ToString()
        {
            return $"In-ate: {Field} From {DataSource}";
        }
    }
}