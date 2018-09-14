using Avalonia;
using Avalonia.Markup.Xaml;

namespace ZenPlatform.ThinClient
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
