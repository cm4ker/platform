using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QCombinedDataSource : QDataSource
    {
        public override IEnumerable<QField> GetFields()
        {
            foreach (var source in DataSources)
            {
                foreach (var field in source.GetFields())
                {
                    yield return field;
                }
            }
        }
    }
}