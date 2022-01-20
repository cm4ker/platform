using System.Collections.Generic;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying
{
    public class QLangWalker : QLangVisitorBase
    {
        protected int Depth { get; set; } = 1;
        private readonly Stack<QLangElement> _stack = new Stack<QLangElement>();

        protected IEnumerable<QLangElement> CurrentStackTree => _stack;

        public override void DefaultVisit(QLangElement node)
        {
            if (node == null)
                return;

            _stack.Push(node);
            Depth++;
            foreach (var child in node.GetChildren())
            {
                _stack.Push(child);
                Visit(child);
                _stack.Pop();
            }

            Depth--;
            _stack.Pop();
            //
            // base.DefaultVisit(node);
        }
    }


    public class QLangWalker<T> : QLangVisitorBase<T>
    {
        protected int Depth { get; set; } = 1;
        private readonly Stack<QLangElement> _stack = new Stack<QLangElement>();

        protected IEnumerable<QLangElement> CurrentStackTree => _stack;

        
        
        public override T DefaultVisit(QLangElement node)
        {
            if (node == null)
                return default;

            _stack.Push(node);
            Depth++;
            foreach (var child in node.GetChildren())
            {
                _stack.Push(child);
                Visit(child);
                _stack.Pop();
            }

            Depth--;
            _stack.Pop();

            return base.DefaultVisit(node);
        }
    }
}