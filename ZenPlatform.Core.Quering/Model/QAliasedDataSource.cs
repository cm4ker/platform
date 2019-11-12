using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    public class QAliasedDataSource : IQDataSource
    {
        public QAliasedDataSource(IQDataSource parent, string alias)
        {
            Parent = parent;
            Alias = alias;
        }

        public IQDataSource Parent { get; }

        public string Alias { get; }

        public IEnumerable<QField> GetFields()
        {
            return Parent.GetFields();
        }
    }
}