using Avalonia.Controls;
using Portable.Xaml.Markup;
using ZenPlatform.Avalonia.Wrapper.ControlLayer;

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

        public override object GetUnderlyingControl()
        {
            return _uc;
        }

        public virtual void CreateOnServer()
        {
        }
    }
}