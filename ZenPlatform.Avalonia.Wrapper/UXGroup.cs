using Avalonia.Controls;
using Portable.Xaml.Markup;

namespace ZenPlatform.Avalonia.Wrapper
{
    [ContentProperty(nameof(Childs))]
    public class UXGroup : UXElement
    {
        private Grid _g;

        public UXGroup()
        {
            _g = new Grid();
            Childs = new UXGroupCollection(this);
        }

        public UXGroupOrientation Orientation { get; set; }

        public UXGroupCollection Childs { get; }

        public override object GetUnderlyingControl()
        {
            return _g;
        }
    }
}