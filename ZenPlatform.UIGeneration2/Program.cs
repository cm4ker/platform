using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Context;
using Avalonia.Metadata;
using Portable.Xaml;
using ZenPlatform.UIBuilder.Compilers;
using ZenPlatform.UIBuilder.Compilers.Avalonia;
using ZenPlatform.UIBuilder.Interface;
using ZenPlatform.UIBuilder.Interface.DataGrid;

namespace ZenPlatform.UIBuilder
{
    public class Program
    {
        public static void Main()
        {
            DataGrid dg = new DataGrid();

            var appBuilder = BuildAvaloniaApp().SetupWithoutStarting();

            var window = new UIWindow().With(x =>
                x.Group()
                    .With(gi =>
                    {
                        var tb = gi.TextBox();
                        tb.DataSource = "Person";
                        return tb;
                    })
                    .With(l => l.Label("Label component"))
                        .With(f => f.CheckBox("Checkbox component"))
                        .With(f => f.Button("Button component"))
                        // .With(new UIObjectPicker())
                        .With(new UIDataGrid().WithColumn(f => f.Column())
                                              .WithColumn(f => f.Column())
                                              .WithColumn(f => f.Column()))
                        .With(tc => tc.TabControl().WithTab(t =>
                            {
                                t.Header = "Page 1";
                                t.With(f => f.Label("This is content on page 1"));
                            })
                            .WithTab(t =>
                            {
                                t.Header = "Page 2";
                                t.With(f => f.Label("This is content on page 2"));
                            })));

            window.Height = 400;
            window.Width = 300;

            AvaloniaXamlUICompiler c = new AvaloniaXamlUICompiler();

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var text = c.Compile(window, sw);

            Console.WriteLine(text);

            Window w = AvaloniaXamlLoader.Parse<Window>(text);
            w.DataContext = new { Person = "ФИО Человека" };

            appBuilder.Instance.Run(w);

            w.ShowDialog();

            //XamlWriterTest();
            //Console.Read();
        }


        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();

        public static void XamlWriterTest()
        {
            var tw = new StringWriter();
            var xw = XmlWriter.Create(tw);
            CustomXamlWriter x = new CustomXamlWriter(xw);

            //Scenario
            x.StartObject();

            x.StartProperty();
            x.Value();
            x.EndProperty();

            x.EndObject();

            xw.Close();
            //EndScenario

            Console.WriteLine(tw.GetStringBuilder().ToString());
        }
    }
}