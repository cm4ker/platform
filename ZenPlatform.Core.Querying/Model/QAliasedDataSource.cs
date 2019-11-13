using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedDataSource : QDataSource
    {
        public QAliasedDataSource(QDataSource parent, string alias)
        {
            Parent = parent;
            Alias = alias;
        }

        public QDataSource Parent { get; }

        public string Alias { get; }

        public override IEnumerable<QField> GetFields()
        {
            return Parent.GetFields();
        }
    }
}