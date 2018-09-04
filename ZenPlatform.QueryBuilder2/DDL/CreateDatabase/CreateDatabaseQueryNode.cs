using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DDL.CreateDatabase
{
    public class CreateDatabaseQueryNode : SqlNode, ICreateDatabaseQuery
    {
        private SqlNode _name;

        public CreateDatabaseQueryNode()
        {
        }

        public CreateDatabaseQueryNode(string dataBaseName)
        {
            _name = new IdentifierNode(dataBaseName);
        }

        public CreateDatabaseQueryNode WithName(string dataBaseName)
        {
            _name = new IdentifierNode(dataBaseName);
            return this;
        }

        SqlNode ICreateDatabaseQuery.Name => _name;
    }


    public interface ICreateDatabaseQuery : IChildItem<Node>, IParentItem<Node, Node>
    {
        SqlNode Name { get; }
    }


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