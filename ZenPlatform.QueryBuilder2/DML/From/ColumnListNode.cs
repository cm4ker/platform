using System.Linq;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
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
                Add(Tokens.CommaToken);
            }

            Add(node);

            return this;
        }
    }
}