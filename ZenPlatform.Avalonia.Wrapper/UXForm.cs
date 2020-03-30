#if CLIENT
using Avalonia.Controls;
#endif

using Portable.Xaml.Markup;
using ZenPlatform.Avalonia.Wrapper.ControlLayer;

namespace ZenPlatform.Avalonia.Wrapper
{
    [ContentProperty(nameof(Content))]
    public class UXForm : UXElement
    {
#if CLIENT
        private UserControl _uc;
#endif
        private UXElement _content;

        public UXForm()
        {
#if CLIENT
            _uc = new UserControl();
#endif
        }


        public UXElement Content
        {
            get => _content;
            set
            {
#if CLIENT
                _uc.Content = value.GetUnderlyingControl();
#endif
                _content = value;
            }
        }

        public override object GetUnderlyingControl()
        {
#if CLIENT
            return _uc;
#else
            return null;
#endif
        }

        public virtual void CreateOnServer()
        {
        }
    }
}