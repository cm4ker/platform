using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders.DML
{
    public class SelectBuilder
    {
        private SelectNode _selectNode;

        public SelectBuilder(SelectNode selectNode)
        {
            _selectNode = selectNode;
        }

        public SelectBuilder SelectField(string fieldName)
        {
            _selectNode.Fields.Add(new TableFieldNode() { Field = fieldName });
            return this;
        }

        public SelectBuilder From(string tableName)
        {
            _selectNode.From.DataSource = new TableSourceNode() { Table = new Table() { Value = tableName } };
            return this;
        }

        public SelectBuilder Where(string tableName)
        {
            _selectNode.From.DataSource = new TableSourceNode() { Table = new Table() { Value = tableName } };
            return this;
        }


    }
}
