using ZenPlatform.QueryBuilder.Common.SqlTokens;

namespace ZenPlatform.QueryBuilder.Common.Operations
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