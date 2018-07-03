using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Where;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public class IsNullWhereNode : WhereNode
    {
        public IsNullWhereNode(SqlNode exp)
        {
            Childs.Add(exp);
        }
    }
}