using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    public class QCombinedDataSource : IQDataSource
    {
        public List<IQDataSource> DataSources { get; }

        public QCombinedDataSource(List<IQDataSource> dataSources)
        {
            DataSources = dataSources;
        }

        public IEnumerable<QField> GetFields()
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