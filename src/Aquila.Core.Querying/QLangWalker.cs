using System.Collections.Generic;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying
{
    public class QLangWalker : QLangVisitorBase<object>
    {
        protected int Depth { get; set; } = 1;
        private readonly Stack<QItem> _stack = new Stack<QItem>();

        protected IEnumerable<QItem> CurrentStackTree => _stack;

        public override object DefaultVisit(QItem node)
        {
            if (node == null)
                return default;

            _stack.Push(node);
            Depth++;
            foreach (var child in node.Children)
            {
                if (child is QItem qi)
                {
                    _stack.Push(qi);
                    Visit(qi);
                    _stack.Pop();
                }
            }

            Depth--;
            _stack.Pop();

            return base.DefaultVisit(node);
        }
    }
}