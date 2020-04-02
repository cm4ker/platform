#if CLIENT
using Avalonia.Controls;
#endif

namespace ZenPlatform.Avalonia.Wrapper
{
    public class UXCheckBox : UXElement
    {
#if CLIENT
        private CheckBox _cb;
#endif
        public UXCheckBox()
        {
#if CLIENT
            _cb = new CheckBox();
#endif
        }

        public override object GetUnderlyingControl()
        {
#if CLIENT
            return _cb;
#else
            return null;
#endif
        }
    }

    public class UXButton : UXElement
    {
#if CLIENT
        private Button _b;
#endif
        public UXButton()
        {
#if CLIENT
            _b = new Button();
#endif
        }

        public override object GetUnderlyingControl()
        {
#if CLIENT
            return _b;
#else
            return null;
#endif
        }
    }

    public class UXDatePicker : UXElement
    {
#if CLIENT
        private DatePicker _dp;
#endif
        public UXDatePicker()
        {
#if CLIENT
            _dp = new DatePicker();
#endif
        }

        public override object GetUnderlyingControl()
        {
#if CLIENT
            return _dp;
#else
            return null;
#endif
        }
    }
}