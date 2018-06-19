using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class TypeDefinitionNode : SqlNode
    {
        public TypeDefinitionNode(string typeName)
        {
            Childs.Add(new IdentifierNode(typeName));
        }

        public TypeDefinitionNode WithSize(int size)
        {
            Childs.Add(Tokens.LeftBracketToken);
            Childs.Add(new RawSqlNode(size.ToString()));
            Childs.Add(Tokens.RightBracketToken);
            return this;
        }
        public TypeDefinitionNode WithScaleAndPresision(int scale, int precision)
        {
            Childs.Add(Tokens.LeftBracketToken);
            Childs.Add(new RawSqlNode(scale.ToString()));
            Childs.Add(Tokens.CommaToken);
            Childs.Add(new RawSqlNode(precision.ToString()));
            Childs.Add(Tokens.RightBracketToken);
            return this;
        }
    }
}