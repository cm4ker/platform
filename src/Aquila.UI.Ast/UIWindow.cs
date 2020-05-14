using System;

namespace Aquila.UI.Ast
{
    public class UIWindow : UINode
    {
        public UIWindow With(UINode node)
        {
            Attach(node);
            return this;
        }

        public UIWindow With(Func<UIFactory, UINode> factory)
        {
            return With(factory(UIFactory.Get()));
        }

        public double Height { get; set; }
        public double Width { get; set; }
    }
}