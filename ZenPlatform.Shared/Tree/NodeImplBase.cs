using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Shared.Tree
{
    public class Node : IChildItem<Node>, IParentItem<Node, Node>
    {
        private Node _parent;

        public Node()
        {
            Childs = new ChildItemCollection<Node, Node>(this);
        }

        Node IChildItem<Node>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public Node Parent => _parent;

        public ChildItemCollection<Node, Node> Childs { get; }

        public virtual void Add(Node sqlNode)
        {
            if (sqlNode == this) throw new Exception("Recursial dependency not allowed");
            Childs.Add(sqlNode);
        }

        public virtual void AddRange(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        public virtual Node GetChild<T>()
        {
            foreach (var child in Childs)
            {
                if (child is T) return child;
            }

            return null;
        }

        public virtual Node FirstParent<T>()
        {
            return _parent is T ? _parent : _parent?.FirstParent<T>();
        }

        public virtual void Detach()
        {
            _parent?.Childs.Remove(this);
        }

        public virtual void Attach(Node node)
        {
            Detach();
            node.Childs.Add(this);
        }

        public virtual void Attach(int index, Node node)
        {
            Detach();
            node.Childs.Insert(index, this);
        }
    }
}