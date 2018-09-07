using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DDL.CreateDatabase
{
    public class DropDatabaseQueryNode : SqlNode, IDropDatabaseQuery
    {
        private SqlNode _name;

        public DropDatabaseQueryNode()
        {
        }

        public DropDatabaseQueryNode(string databaseName)
        {
            _name = new IdentifierNode(databaseName);
        }

        public DropDatabaseQueryNode WithName(string databaseName)
        {
            _name = new IdentifierNode(databaseName);
            return this;
        }

        SqlNode IDropDatabaseQuery.Name => _name;
    }

    public interface IDropDatabaseQuery : IChildItem<Node>, IParentItem<Node, Node>
    {
        SqlNode Name { get; }
    }
}