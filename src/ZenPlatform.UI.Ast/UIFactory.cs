namespace ZenPlatform.UI.Ast
{
    /// <summary>
    /// Фабрика элементов интерфейса
    /// </summary>
    public class UIFactory
    {
        private static UIFactory _instance = new UIFactory();

        /// <summary>
        ///  Получить фабрику
        /// </summary>
        /// <returns></returns>
        public static UIFactory Get()
        {
            return _instance;
        }

        public UITextBox TextBox()
        {
            return new UITextBox();
        }

        public UILabel Label(string text = "")
        {
            return new UILabel(text);
        }

        public UIGroup Group(UIGroupOrientation orientation = UIGroupOrientation.Vertical)
        {
            var g = new UIGroup();
            g.Orientation = orientation;

            return g;
        }

        internal UITab Tab()
        {
            return new UITab();
        }

        public UITabControl TabControl()
        {
            return new UITabControl();
        }

        public UIWindow Window()
        {
            return new UIWindow();
        }

        public UICheckBox CheckBox(string text = "")
        {
            return new UICheckBox(text);
        }

        public UIButton Button(string text = "")
        {
            return new UIButton(text);
        }
    }
}