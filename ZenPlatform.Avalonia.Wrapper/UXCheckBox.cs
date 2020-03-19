using Avalonia.Controls;

namespace ZenPlatform.Avalonia.Wrapper
{
    public class UXCheckBox : UXElement
    {
        private CheckBox _cb;

        public UXCheckBox()
        {
            _cb = new CheckBox();
        }

        public override Control GetUnderlyingControl()
        {
            return _cb;
        }
    }

    public class UXButton : UXElement
    {
        private Button _b;

        public UXButton()
        {
            _b = new Button();
        }

        public override Control GetUnderlyingControl()
        {
            return _b;
        }
    }

    public class UXDatePicker : UXElement
    {
        private DatePicker _dp;

        public UXDatePicker()
        {
            _dp = new DatePicker();
        }

        public override Control GetUnderlyingControl()
        {
            return _dp;
        }
    }
}