using ZenPlatform.Core.Querying.Model;

namespace ZenPlatform.Core.Querying
{
    public class QLangWalker : QLangVisitorBase<object>
    {
        protected int Depth { get; set; } = 1;

        public override object DefaultVisit(QItem node)
        {
            if (node == null)
                return default;

            Depth++;
            foreach (var child in node.Children)
            {
                if (child is QItem qi)
                    Visit(qi);
            }

            Depth--;

            return base.DefaultVisit(node);
        }
    }
}