using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class RenameTableBuilder
    {
        private RenameTableNode _node;

        public RenameTableBuilder(RenameTableNode node)
        {
            _node = node;
        }

        public RenameTableBuilder From(string oldName)
        {
            _node.From = new Table() { Value = oldName };
            return this;
        }

        public RenameTableBuilder To(string newName)
        {
            _node.To = new Table() { Value = newName };
            return this;
        }
    }
}