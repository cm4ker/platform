namespace ZenPlatform.QueryBuilder.Common.SqlTokens
{
    public class Token : SqlNode
    {
        public Token(string name)
        {
            Childs.Add(new RawSqlNode(name));
        }
    }
}