using Avalonia;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.IDE
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
