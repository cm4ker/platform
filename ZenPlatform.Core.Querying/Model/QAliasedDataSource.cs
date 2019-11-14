using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        public override IEnumerable<QField> GetFields()
        {
            foreach (var field in Parent.GetFields())
            {
                yield return new QIntermediateSourceField(field, this);
            }
        }

        public override string? ToString()
        {
            return Parent.ToString() + " AS " + Alias;
        }
    }
}