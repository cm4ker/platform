using Avalonia;
using Avalonia.Markup.Xaml;

namespace Aquila.ServerManagementTool
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
