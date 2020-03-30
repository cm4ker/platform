using Avalonia.Controls;

namespace ZenPlatform.Avalonia.Wrapper
{
    public class UXTextBox : UXElement
    {
        private TextBox _c;

        public UXTextBox()
        {
            _c = new TextBox();
        }

        public override object GetUnderlyingControl()
        {
            return _c;
        }
    }
}