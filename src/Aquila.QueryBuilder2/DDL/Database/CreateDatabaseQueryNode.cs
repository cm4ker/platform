using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.ParenChildCollection;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DDL.CreateDatabase
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