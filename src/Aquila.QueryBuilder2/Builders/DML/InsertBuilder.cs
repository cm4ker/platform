using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class InsertBuilder
    {
        private InsertNode _insertNode;

        public InsertBuilder(InsertNode insertNode)
        {
            _insertNode = insertNode;

        }

        public InsertBuilder Into(string tableName)
        {
            _insertNode.Into = new Table() { Value = tableName };
            
            return this;
        }

        public InsertBuilder From(Action<SelectBuilder> subSelectBuilder)
        {

            var subSelectNode = new SelectNode();
            var builder = new SelectBuilder(subSelectNode);
            subSelectBuilder(builder);

            _insertNode.DataSource = subSelectNode;

            return this;
        }




    }
}
