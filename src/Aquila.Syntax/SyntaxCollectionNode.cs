using System;
using System.Collections;
using System.Collections.Generic;
using Aquila.Syntax.Text;
using Aquila.Syntax.Tree;

namespace Aquila.Syntax
{
    public class SyntaxCollectionNode<T> : LangElement, IList<T> where T : LangElement
    {
        public SyntaxCollectionNode(Span span) : base(span, SyntaxKind.Argument)
        {
        }

        public override void Add(Node node)
        {
            if (!(node is T s))
                throw new Exception("This is not allowed");

            Add(s);
        }

        public void Add(T statement)
        {
            base.Add(statement);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var element in this)
            {
                array[arrayIndex] = element;
            }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index] 
        {

            get { return (T) Children[index];}
            set { throw new NotImplementedException(); }
        }

        public int Count => Children.Count;
        public bool IsReadOnly { get; }

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