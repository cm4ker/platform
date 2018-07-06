using ZenPlatform.QueryBuilder.DML.Where;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class IsNullWhereNode : WhereNode
    {
        public IsNullWhereNode(SqlNode exp)
        {
            Childs.Add(exp);
        }
    }
}