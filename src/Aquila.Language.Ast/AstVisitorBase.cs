using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Language.Ast.AST;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;

namespace Aquila.Language.Ast
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


    public abstract partial class AstVisitorBase
    {
        private Stack<SyntaxNode> _visitStack;
        private bool _break;

        protected Stack<SyntaxNode> VisitStack => _visitStack;

        public AstVisitorBase()
        {
            _visitStack = new Stack<SyntaxNode>();
        }

        public virtual void Visit(SyntaxNode visitable)
        {
            visitable?.Accept(this);
        }

        public virtual void DefaultVisit(SyntaxNode node)
        {
        }

        public virtual void VisitTypeEntity(TypeEntity obj)
        {
            DefaultVisit(obj);
        }
    }


    public class AstWalker<T> : AstVisitorBase<T>
    {
        public override T DefaultVisit(SyntaxNode node)
        {
            Console.WriteLine($"We are visit: {node}");

            var childs = node.Children.ToList();

            foreach (var child in childs)
            {
                Visit(child as SyntaxNode);
            }

            return base.DefaultVisit(node);
        }
    }

    public class AstWalker : AstVisitorBase
    {
        public override void DefaultVisit(SyntaxNode node)
        {
            Console.WriteLine($"We are visit: {node}");

            var childs = node.Children.ToList();

            foreach (var child in childs)
            {
                Visit(child as SyntaxNode);
            }

            base.DefaultVisit(node);
        }
    }
}