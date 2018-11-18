using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Shared.Tree
{

    /// <summary>
    /// Класс ноды иерархического дерева 
    /// </summary>
    [DataContract]
    public class Node : IChildItem<Node>, IParentItem<Node, Node>
    {
        private Node _parent;

        public Node()
        {
            Childs = new ChildItemCollection<Node, Node>(this);
        }

        /// <summary>
        /// Родитель
        /// </summary>
        [IgnoreDataMember]
        Node IChildItem<Node>.Parent
        {
            get => _parent;
            set => _parent = value;
        }

        /// <summary>
        /// Родитель
        /// </summary>
        [IgnoreDataMember]
        public Node Parent => _parent;


        /// <summary>
        /// Дочерние элекменты
        /// </summary>
        public ChildItemCollection<Node, Node> Childs { get; }

        /// <summary>
        /// Добавить подчинённую ноду
        /// </summary>
        /// <param name="sqlNode"></param>
        public virtual void Add(Node sqlNode)
        {
            if (sqlNode == this) throw new Exception("Recursial dependency not allowed");
            Childs.Add(sqlNode);
        }

        /// <summary>
        /// Добавить список нод
        /// </summary>
        /// <param name="nodes"></param>
        public virtual void AddRange(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        /// <summary>
        /// Получить дочерний элемент по типу
        /// </summary>
        /// <typeparam name="T">Тип дочернего элемента</typeparam>
        /// <returns></returns>
        public virtual Node GetChild<T>() where T : Node
        {
            foreach (var child in Childs)
            {
                if (child is T) return child;
            }

            return null;
        }

        /// <summary>
        /// Получить первого родителя с определённым типом
        /// </summary>
        /// <typeparam name="T">Тип получаемого родителя</typeparam>
        /// <returns></returns>
        public virtual Node FirstParent<T>()
        {
            return _parent is T ? _parent : _parent?.FirstParent<T>();
        }


        /// <summary>
        /// Отсоединить ноду. Внимание свойство Parent автоматически будет присвоено в null
        /// </summary>
        public virtual void Detach()
        {
            _parent?.Childs.Remove(this);
        }

        /// <summary>
        /// Прикрепить элемент. Внимание сначала будет выполнена процедура <see cref="Detach"/> а затем уже элемент будет добавлен в качесте дочернего
        /// </summary>
        /// <param name="node"></param>
        public virtual void Attach(Node node)
        {
            Detach();
            node.Childs.Add(this);
        }

        /// <summary>
        /// Прикрепить элемент в определённое место. Внимание сначала будет выполнена процедура <see cref="Detach"/> а затем уже элемент будет добавлен в качесте дочернего
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        public virtual void Attach(int index, Node node)
        {
            Detach();
            node.Childs.Insert(index, this);
        }
    }
}