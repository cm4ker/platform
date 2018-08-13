using System;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using ZenPlatform.UIBuilder.Compilers;
using ZenPlatform.UIBuilder.Interface;

namespace ZenPlatform.UIBuilder
{
    public class Program
    {
        public static void Main()
        {
            var appBuilder = BuildAvaloniaApp().SetupWithoutStarting();


            var window = new UIWindow().With(x =>
                x.Group(UIGroupOrientation.Horizontal)
                    .With(g => g.Group(UIGroupOrientation.Horizontal)
                        .With(gi => gi.TextBox())
                        .With(gi => gi.TextBox()))
                    .With(g => g.Group(UIGroupOrientation.Vertical).With(gi => gi.TextBox())));

            //window.With(x => x.Group(UIGroupOrientation.Vertical).With(g => g.TextBox()));

            window.Height = 100;
            window.Width = 100;

            AvaloniaXamlUICompiler c = new AvaloniaXamlUICompiler();

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var text = c.Compile(window, sw);


            Console.WriteLine(text);

            Window w = AvaloniaXamlLoader.Parse<Window>(text);

            appBuilder.Instance.Run(w);


            w.ShowDialog();

            //Console.Read();
        }


        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}