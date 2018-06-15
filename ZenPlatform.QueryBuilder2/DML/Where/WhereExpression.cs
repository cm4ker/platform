using System.Security.Cryptography.X509Certificates;
using ZenPlatform.QueryBuilder2.Select;

namespace ZenPlatform.QueryBuilder2
{
    public class WhereExpression : SqlNode
    {
        public bool IsNot { get; set; }
    }

    public class ParameterNode : SqlNode
    {
        public ParameterNode(string parameterName)
        {
            Childs.Add(new RawSqlNode(parameterName));
        }
    }
}