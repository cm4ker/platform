using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public class CreateDatabaseQueryNode : SqlNode
    {
        public CreateDatabaseQueryNode(string databaseName)
        {
            Childs.AddRange(Tokens.CreateToken, Tokens.SpaceToken,
                Tokens.DatabaseToken, Tokens.SpaceToken, new IdentifierNode(databaseName));
        }
    }
}