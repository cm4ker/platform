using System;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder2.ParenChildCollection;

namespace ZenPlatform.QueryBuilder2.Common
{
    public class SqlNode : IChildItem<SqlNode>
    {
        private SqlNode _parent;

        public SqlNode()
        {
            Childs = new ChildItemCollection<SqlNode, SqlNode>(this);
        }


        SqlNode IChildItem<SqlNode>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        internal SqlNode Parent => _parent;

        public ChildItemCollection<SqlNode, SqlNode> Childs { get; }

        internal virtual void Add(SqlNode node)
        {
            if (node == this) throw new Exception("Recursial dependency not allowed");
            Childs.Add(node);
        }

        internal virtual void AddRange(IEnumerable<SqlNode> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }
    }
}