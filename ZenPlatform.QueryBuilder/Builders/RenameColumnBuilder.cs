using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class RenameColumnBuilder
    {
        private RenameColumnNode _node;

        public RenameColumnBuilder(RenameColumnNode node)
        {
            _node = node;
        }

        public RenameColumnBuilder OnTable(string tablename)
        {
            _node.Table = new Table { Value = tablename };
            return this;
        }

        public RenameColumnBuilder From(string oldName)
        {
            _node.From = new Column() { Value = oldName };
            return this;
        }

        public RenameColumnBuilder To(string newName)
        {
            _node.To = new Column() { Value = newName };
            return this;
        }


    }
}
