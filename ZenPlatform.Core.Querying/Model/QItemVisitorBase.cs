using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
{
    public abstract partial class QLangVisitorBase<T>
    {
        private Stack<QItem> _visitStack;
        private bool _break;

        protected Stack<QItem> VisitStack => _visitStack;

        public QLangVisitorBase()
        {
            _visitStack = new Stack<QItem>();
        }

        public virtual T Visit(QItem visitable)
        {
            if (visitable is null) return default;

            return visitable.Accept(this);
        }

        public virtual T DefaultVisit(QItem node)
        {
            return default;
        }
    }


    public abstract partial class QLangVisitorBase
    {
        private Stack<QItem> _visitStack;
        private bool _break;

        protected Stack<QItem> VisitStack => _visitStack;

        public QLangVisitorBase()
        {
            _visitStack = new Stack<QItem>();
        }

        public virtual void Visit(QItem visitable)
        {
            visitable?.Accept(this);
        }

        public virtual void DefaultVisit(QItem node)
        {
        }
    }
}