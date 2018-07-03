using System.Linq;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.Common.Columns
{
    public class ColumnListNode : SqlNode
    {
        public ColumnListNode WithField(string fieldName)
        {
            return WithField(new FieldNode(fieldName));
        }

        public ColumnListNode WithField(FieldNode node)
        {
            if (Childs.Any())
            {
                Add(Tokens.Tokens.CommaToken);
            }

            Add(node);

            return this;
        }
    }
}