using Avalonia;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.UIBuilder
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
