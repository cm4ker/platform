using System.Linq;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common.Columns
{
    public class ColumnListNode : Node
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