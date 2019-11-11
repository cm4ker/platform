using System;

namespace ZenPlatform.UI.Ast
{
    public class UITabControl : UINode
    {
        public UITabControl WithTab(Action<UITab> tab)
        {
            var tabContorl = UIFactory.Get().Tab();
            tab(tabContorl);

            Add(tabContorl);

            return this;
        }
    }
}
