using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public partial class Block : IScoped
    {
        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public Block(StatementList statements) : this(null, statements)
        {
        }
    }

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

        public T this[int index] => (T) Childs[index];

        public int Count => Childs.Count;

        // public List<T> ToList()
        // {
        //     return Childs.Cast<T>().ToList();
        // }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var node in Childs)
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