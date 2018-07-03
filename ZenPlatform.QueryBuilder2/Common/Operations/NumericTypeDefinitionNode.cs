namespace ZenPlatform.QueryBuilder2.Common.Operations
{
    public class NumericTypeDefinitionNode : TypeDefinitionNode
    {
        public NumericTypeDefinitionNode(int scale, int precision) : base("numeric")
        {
            Childs.AddRange(
                Tokens.Tokens.LeftBracketToken,
                new RawSqlNode(scale.ToString()),
                Tokens.Tokens.CommaToken,
                new RawSqlNode(precision.ToString()),
                Tokens.Tokens.RightBracketToken
            );
        }
    }
}