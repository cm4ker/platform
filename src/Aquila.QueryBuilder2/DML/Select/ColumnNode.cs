using Aquila.QueryBuilder.Common;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
{
    public class ColumnNode : SqlNode
    {
        public ColumnNode(string fieldName)
        {
            Childs.Add(new IdentifierNode(fieldName));
        }

        public ColumnNode WithParent(string parentName)
        {
            Childs.Insert(0, new IdentifierNode(parentName));
            Childs.Insert(1, new SchemaSeparatorNode());

            return this;
        }
    }
}