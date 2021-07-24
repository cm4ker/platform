using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.Syntax
{
    public abstract partial class AstVisitorBase<T>
    {
        private Stack<LangElement> _visitStack;
        private bool _break;

        protected Stack<LangElement> VisitStack => _visitStack;

        public AstVisitorBase()
        {
            _visitStack = new Stack<LangElement>();
        }

        public virtual T Visit(LangElement visitable)
        {
            if (visitable is null) return default;

            return visitable.Accept(this);
        }

        public virtual T DefaultVisit(LangElement node)
        {
            return default;
        }

        public virtual T VisitSyntaxToken(SyntaxToken obj)
        {
            return DefaultVisit(obj);
        }
    }


    public abstract partial class AstVisitorBase
    {
        private Stack<LangElement> _visitStack;
        private bool _break;

        protected Stack<LangElement> VisitStack => _visitStack;

        public AstVisitorBase()
        {
            _visitStack = new Stack<LangElement>();
        }

        public virtual void Visit(LangElement visitable)
        {
            visitable?.Accept(this);
        }

        public virtual void DefaultVisit(LangElement node)
        {
        }

        public virtual void VisitSyntaxToken(SyntaxToken obj)
        {
            DefaultVisit(obj);
        }
    }


    public class AstWalker<T> : AstVisitorBase<T>
    {
        public override T DefaultVisit(LangElement node)
        {
            Console.WriteLine($"We are visit: {node}");

            var childs = node.Children.ToList();

            foreach (var child in childs)
            {
                Visit(child as LangElement);
            }

            return base.DefaultVisit(node);
        }
    }

    public class AstWalker : AstVisitorBase
    {
        public override void DefaultVisit(LangElement node)
        {
            Console.WriteLine($"We are visit: {node}");

            var childs = node.Children.ToList();

            foreach (var child in childs)
            {
                Visit(child as LangElement);
            }

            base.DefaultVisit(node);
        }
    }
}