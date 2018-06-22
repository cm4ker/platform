using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class NumericTypeDefinitionNode : TypeDefinitionNode
    {
        public NumericTypeDefinitionNode(int scale, int precision) : base("numeric")
        {
            Childs.AddRange(
                Tokens.LeftBracketToken,
                new RawSqlNode(scale.ToString()),
                Tokens.CommaToken,
                new RawSqlNode(precision.ToString()),
                Tokens.RightBracketToken
            );
        }
    }
}