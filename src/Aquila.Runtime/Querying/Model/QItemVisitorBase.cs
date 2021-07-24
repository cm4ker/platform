using System.Collections.Generic;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying
{
    public partial class QLangVisitorBase<T>
    {
        private Stack<QLangElement> _visitStack;
        private bool _break;

        protected Stack<QLangElement> VisitStack => _visitStack;

        public QLangVisitorBase()
        {
            _visitStack = new Stack<QLangElement>();
        }

        public virtual T Visit(QLangElement visitable)
        {
            if (visitable is null) return default;

            return visitable.Accept(this);
        }

        public virtual T DefaultVisit(QLangElement node)
        {
            return default;
        }
    }


    public partial class QLangVisitorBase
    {
        private Stack<QLangElement> _visitStack;
        private bool _break;

        protected Stack<QLangElement> VisitStack => _visitStack;

        public QLangVisitorBase()
        {
            _visitStack = new Stack<QLangElement>();
        }

        public virtual void Visit(QLangElement visitable)
        {
            visitable?.Accept(this);
        }

        public virtual void DefaultVisit(QLangElement node)
        {
        }
    }
}