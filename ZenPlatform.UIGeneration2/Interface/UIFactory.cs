namespace ZenPlatform.UIGeneration2
{
    public class UIFactory
    {
        public static UIFactory Get()
        {
            return new UIFactory();
        }

        public UITextBox TextBox()
        {
            return new UITextBox();
        }

        public UIGroup Group(UIGroupOrientation orientation = UIGroupOrientation.Vertical)
        {
            var g = new UIGroup();
            g.Orientation = orientation;

            return g;
        }

        public UIWindow Window()
        {
            return new UIWindow();
        }
    }
}