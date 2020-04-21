using System;
using System.Collections;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Language.Ast.Definitions
{
    public class SyntaxCollectionNode<T> : SyntaxNode, IEnumerable<T> where T : SyntaxNode
    {
        public SyntaxCollectionNode(ILineInfo lineInfo) : base(lineInfo)
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

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void Accept(AstVisitorBase visitor)
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