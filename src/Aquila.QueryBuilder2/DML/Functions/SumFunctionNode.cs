using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.SqlTokens;

namespace Aquila.QueryBuilder.DML.Functions
{
    public class SumFunctionNode : SqlNode
    {
        public SumFunctionNode(SqlNode aggregate)
        {
            Childs.AddRange(Tokens.SumToken, Tokens.LeftBracketToken, aggregate, Tokens.RightBracketToken);
        }
    }
}