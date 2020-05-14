using Aquila.QueryBuilder.DML.Where;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
{
    public class IsNullWhereNode : WhereNode
    {
        public IsNullWhereNode(Node exp)
        {
            Childs.Add(exp);
        }
    }
}