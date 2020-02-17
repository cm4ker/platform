using System;
using System.Collections;
using System.Collections.Generic;
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
        public Block(List<Statement> statements) : this(null, statements)
        {
            if (statements == null)
                return;

            Statements = statements;
        }
    }

    public static partial class Factory
    {
        public static SyntaxCollectionNode<Statement> StatementsCollection(List<Statement> statements)
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
            if (!(node is Statement s))
                throw new Exception("This is not allowed");

            Add(s);
        }

        public void Add(Statement statement)
        {
            base.Add(statement);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }

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