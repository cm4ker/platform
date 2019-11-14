using ZenPlatform.Core.Querying.Model;
using ZenPlatform.Core.Querying.Visitor;

namespace ZenPlatform.Core.Querying
{
    public class QLangWalker : QLangVisitorBase<object>
    {
        protected int Depth { get; set; } = 1;

        public override object DefaultVisit(QItem node)
        {
            Depth++;
            foreach (var child in node.Childs)
            {
                if (child is QItem qi)
                    Visit(qi);
            }

            Depth--;

            return base.DefaultVisit(node);
        }
    }
}