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
            _node.From = oldName;
            return this;
        }

        public RenameTableBuilder To(string newName)
        {
            _node.To = newName;
            return this;
        }


    }
}
