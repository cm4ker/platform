using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.Common.Operations;
using ZenPlatform.QueryBuilder2.Common.Tokens;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
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