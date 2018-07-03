using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Where
{
    public class ParameterNode : SqlNode
    {
        public ParameterNode(string parameterName)
        {
            Childs.Add(new RawSqlNode(parameterName));
        }
    }
}