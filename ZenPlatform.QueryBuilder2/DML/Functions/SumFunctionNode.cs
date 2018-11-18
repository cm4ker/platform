using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.SqlTokens;

namespace ZenPlatform.QueryBuilder.DML.Functions
{
    public class SumFunctionNode : SqlNode
    {
        public SumFunctionNode(SqlNode aggregate)
        {
            Childs.AddRange(Tokens.SumToken, Tokens.LeftBracketToken, aggregate, Tokens.RightBracketToken);
        }
    }
}