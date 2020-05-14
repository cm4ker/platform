using System;

namespace Aquila.UI.Ast
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
            Attach(node);
            return this;
        }

        public UITab With(Func<UIFactory, UINode> factory)
        {
            return With(factory(UIFactory.Get()));
        }
    }
}