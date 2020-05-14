using Aquila.QueryBuilder.Common;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Where
{
    public class ParameterNode : SqlNode
    {
        public ParameterNode(string parameterName)
        {
            Childs.Add(new RawSqlNode(parameterName));
        }
    }
}