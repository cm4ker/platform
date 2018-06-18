using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.From;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public class CreateTableQueryNode : SqlNode
    {
        private AliasedTableNode _aliasedTable;

        public CreateTableQueryNode()
        {
            Childs.Add(Tokens.CreateToken);
            Childs.Add(Tokens.SpaceToken);
            Childs.Add(Tokens.TableToken);
            Childs.Add(Tokens.SpaceToken);
        }
    }
}