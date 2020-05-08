using Avalonia.Threading;
#if CLIENT
using Avalonia.Controls;

#endif

namespace Aquila.Avalonia.Wrapper
{
    public class UXTextBox : UXElement
    {
#if CLIENT
        private TextBox _c;
#endif
        public UXTextBox()
        {
#if CLIENT
            _c = new TextBox();
#endif
        }

        public override object GetUnderlyingControl()
        {
#if CLIENT
            return _c;
#else
            return null;
#endif
        }
    }
}