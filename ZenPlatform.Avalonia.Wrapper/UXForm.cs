using Avalonia.Controls;
using Portable.Xaml.Markup;

namespace ZenPlatform.Avalonia.Wrapper
{
    [ContentProperty(nameof(Content))]
    public class UXForm : UXElement
    {
        private UserControl _uc;
        private UXElement _content;

        public UXForm()
        {
            _uc = new UserControl();
        }


        public UXElement Content
        {
            get => _content;
            set
            {
                _uc.Content = value.GetUnderlyingControl();
                _content = value;
            }
        }

        public override Control GetUnderlyingControl()
        {
            return _uc;
        }
    }
}