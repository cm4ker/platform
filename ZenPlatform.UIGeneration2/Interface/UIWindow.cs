using System;

namespace ZenPlatform.UIGeneration2
{
    public class UIWindow : UINode
    {
        public UIWindow With(UINode node)
        {
            Childs.Add(node);
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