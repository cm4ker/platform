using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Core.Querying.Model
{
    public interface IQCollection
    {
        void Add(object item);
    }

    public class QCollectionItem<T> : QItem, IQCollection, IEnumerable<T> where T : QItem
    {
        public QCollectionItem() : base()
        {
        }

        public void Add(QItem node)
        {
            if (!(node is T s))
                throw new Exception("This is not allowed");

            Add(s);
        }

        public T Last()
        {
            return (T) Children.Last();
        }

        public void Add(T item)
        {
           AddChild(item);
        }

        public void Add(object item)
        {
            Add((T) item);
        }

        public void Remove(T item)
        {
            RemoveChild(item);
        }

        public override T Accept<T>(QLangVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void Accept(QLangVisitorBase visitor)
        {
            throw new NotImplementedException();
        }

        public T this[int index] => (T) Children[index];

        public int Count => Children.Count;

        // public List<T> ToList()
        // {
        //     return Childs.Cast<T>().ToList();
        // }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var node in Children)
            {
                var child = (T) node;

                yield return child;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}