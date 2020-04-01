#if CLIENT
using Avalonia.Controls;
#endif
using Avalonia.Threading;
using Portable.Xaml.Markup;

namespace ZenPlatform.Avalonia.Wrapper
{
    [ContentProperty(nameof(Childs))]
    public class UXGroup : UXElement
    {
#if CLIENT
        private Grid _g;
#endif
        public UXGroup()
        {
#if CLIENT
             _g = new Grid();
#endif
            Childs = new UXGroupCollection(this);
        }

        public UXGroupOrientation Orientation { get; set; }

        public UXGroupCollection Childs { get; }

        public override object GetUnderlyingControl()
        {
#if CLIENT
            return _g;
#else
            return null;
#endif
        }
    }
}