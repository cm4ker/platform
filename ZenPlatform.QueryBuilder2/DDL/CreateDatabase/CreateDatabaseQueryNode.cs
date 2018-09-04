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
}
