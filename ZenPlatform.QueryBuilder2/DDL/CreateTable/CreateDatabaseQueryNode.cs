using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DDL.CreateTable
{
    public class CreateDatabaseQueryNode : Node
    {
        public CreateDatabaseQueryNode(string databaseName)
        {
            Childs.AddRange(Tokens.CreateToken, Tokens.SpaceToken,
                Tokens.DatabaseToken, Tokens.SpaceToken, new IdentifierNode(databaseName));
        }
    }
}