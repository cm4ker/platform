using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.Language.Ast
{
    public abstract partial class AstVisitorBase<T>
    {
        private Stack<SyntaxNode> _visitStack;
        private bool _break;

        protected Stack<SyntaxNode> VisitStack => _visitStack;

        public AstVisitorBase()
        {
            _visitStack = new Stack<SyntaxNode>();
        }

        public virtual T Visit(SyntaxNode visitable)
        {
            if (visitable is null) return default;

            return visitable.Accept(this);
        }

        public virtual T DefaultVisit(SyntaxNode node)
        {
            return default;
        }

        public virtual T VisitTypeEntity(TypeEntity obj)
        {
            return DefaultVisit(obj);
        }
    }

    public class AstWalker<T> : AstVisitorBase<T>
    {
        public override T DefaultVisit(SyntaxNode node)
        {
            Console.WriteLine($"We are visit: {node}");

            var childs = node.Childs.ToList();
            
            foreach (var child in childs)
            {
                Visit(child as SyntaxNode);
            }

            return base.DefaultVisit(node);
        }
    }
}