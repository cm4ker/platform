using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.Common.Operations;
using Aquila.QueryBuilder.Common.SqlTokens;

namespace Aquila.QueryBuilder.DML.From
{
    public class VarcharTypeDefinitionNode : TypeDefinitionNode
    {
        public VarcharTypeDefinitionNode(int size) : base("varchar")
        {
            Childs.Add(Tokens.LeftBracketToken);

            if (size == 0)
                Childs.Add(new RawSqlNode("MAX"));
            else
                Childs.Add(new RawSqlNode(size.ToString()));

            Childs.Add(Tokens.RightBracketToken);
        }
    }
}