using System;

namespace ZenPlatform.UIBuilder.Interface
{
    public class UITab : UINode
    {
        public UITab(string header = "")
        {
            Header = header;
        }

        public string Header { get; set; }

        public UITab With(UINode node)
        {
            Childs.Add(node);
            return this;
        }

        public UITab With(Func<UIFactory, UINode> factory)
        {
            return With(factory(UIFactory.Get()));
        }
    }
}