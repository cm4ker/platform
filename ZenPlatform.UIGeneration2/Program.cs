using System;
using System.Dynamic;
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
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;
using Portable.Xaml;
using Portable.Xaml.Markup;
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
                        .With(new UIObjectPicker())
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

            XamlWriterTest();

            var test = @"
            <Window Height=""400"" Width=""300"" xmlns=""https://github.com/avaloniaui"">
                <Window.Content>
                    <StackPanel Orientation=""Vertical"">
                        <StackPanel.Children>
                            <TextBox Height=""28"" Width=""100"" Text=""{Binding Path=Person, Mode=TwoWay}"" />
                            <TextBlock Text=""Label component"" />
                            <CheckBox Content=""Checkbox component"" />
                            <Button Content=""Button component"" />
                            <ObjectPicker xmlns=""clr-namespace:ZenPlatform.Controls.Avalonia;assembly=ZenPlatform.Controls.Avalonia"" />
                            <TabControl>
                                <TabControl.Items>
                                    <TabItem Header=""Page 1"">
                                        <TabItem.Content>
                                             <TextBlock Text=""This is content on page 1"" />
                                        </TabItem.Content>
                                    </TabItem>
                                    <TabItem Header=""Page 2"">
                                        <TabItem.Content>
                                            <TextBlock Text=""This is content on page 2"" />
                                        </TabItem.Content>
                                    </TabItem>
                                </TabControl.Items>
                            </TabControl>
                        </StackPanel.Children>
                    </StackPanel>
                </Window.Content>
            </Window>
            ";



            Window w = AvaloniaXamlLoader.Parse<Window>(test);
            TestObject obj = new TestObject();
            obj.Person = "123";
            w.DataContext = obj;

            appBuilder.Instance.Run(w);

            w.ShowDialog();

            //XamlWriterTest();
            Console.Write(obj.Person);
            Console.Read();
        }

        public class TestObject
        {
            public string Person { get; set; }
        }


        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();

        public static void XamlWriterTest()
        {
            TextBox tb = new TextBox();
            tb.Text = "Hello";

            BindingExtension

            var context = new AvaloniaCustomXamlSchemaContext(new AvaloniaRuntimeTypeProvider());

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            XamlWriter xw = new XamlXmlWriter(sw, context);
            
            xw.Close();

            Console.WriteLine(sb.ToString());
        }
    }
}