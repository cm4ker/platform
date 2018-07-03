using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Shared.Tree
{
    public class SqlNode : IChildItem<SqlNode>, IParentItem<SqlNode, SqlNode>
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

        public SqlNode Parent => _parent;

        public ChildItemCollection<SqlNode, SqlNode> Childs { get; }

        public virtual void Add(SqlNode sqlNode)
        {
            if (sqlNode == this) throw new Exception("Recursial dependency not allowed");
            Childs.Add(sqlNode);
        }

        public virtual void AddRange(IEnumerable<SqlNode> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        public virtual SqlNode GetChild<T>()
        {
            foreach (var child in Childs)
            {
                if (child is T) return child;
            }

            return null;
        }

        public virtual SqlNode FirstParent<T>()
        {
            return _parent is T ? _parent : _parent?.FirstParent<T>();
        }

        public virtual void Detach()
        {
            _parent?.Childs.Remove(this);
        }

        public virtual void Attach(SqlNode sqlNode)
        {
            Detach();
            sqlNode.Childs.Add(this);
        }
    }
}