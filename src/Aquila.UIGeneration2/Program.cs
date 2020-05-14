using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Aquila.UIBuilder
{
    public class Program
    {

        public static void Main(string[] args )
        {
            var result = AppBuilder.Configure<App>()
                .UsePlatformDetect();

            result.StartWithClassicDesktopLifetime(args);
        }

//        public static void Main1()
//        {
//            var result = AppBuilder.Configure<App>()
//                .UsePlatformDetect()
//                .SetupWithoutStarting();
//
//
//            var window = new UIWindow().With(x =>
//                x.Group()
////                    .With(gi =>
////                    {
////                        var tb = gi.TextBox();
////                        tb.DataSource = "Person";
////                        return tb;
////                    })
//                    .With(l => l.Label("Label component"))
//                    .With(f => f.CheckBox("Checkbox component"))
////                        .With(f =>
////                        {
////                            var b = f.Button("Button component");
////                            b.OnClick = "Click";
////                            return b;
////                        })
////                        .With(new UIObjectPicker())
////                    .With(new UIDataGrid().WithColumn(g => g.TextColumn()))
//                    .With(tc => tc.TabControl().WithTab(t =>
//                        {
//                            t.Header = "Page 1";
//                            t.With(f => f.Label("This is content on page 1"));
//                        })
//                        .WithTab(t =>
//                        {
//                            t.Header = "Page 2";
//                            t.With(f => f.Label("This is content on page 2"));
//                        })));
//
//            window.Height = 400;
//            window.Width = 300;
//
//            AvaloniaXamlUICompiler c = new AvaloniaXamlUICompiler();
//
//            var sb = new StringBuilder();
//            var sw = new StringWriter(sb);
//            var text = c.Compile(window, sw);
//
//            Console.WriteLine(text);
//
//            XamlWriterTest();
//
//
//            Window w = AvaloniaXamlLoader.Parse<Window>(text);
////            TestObject obj = new TestObject();
////            obj.Person = "123";
////            w.DataContext = obj;
////
////            result.Instance.Run(w);
////
//            w.Show();
////
////            //XamlWriterTest();
////            //Console.Write(obj.Person);
////            Console.Read();
//        }

        public class TestObject
        {
            public string Person { get; set; }

            public void Click()
            {
                Console.WriteLine("Clicked");
            }
        }


//        public static AppBuilder BuildAvaloniaApp()
//            => AppBuilder.Configure<App>()
//                .UsePlatformDetect()
//                .LogToDebug();


        public static void XamlWriterTest()
        {
            //TextBox tb = new TextBox();
            //tb.Text = "Hello";

            //var context = new AvaloniaCustomXamlSchemaContext(new AvaloniaRuntimeTypeProvider());

            //var sb = new StringBuilder();
            //var sw = new StringWriter(sb);

            //XamlWriter xw = new XamlXmlWriter(sw, context);

            //xw.Close();

            //Console.WriteLine(sb.ToString());
        }
    }
}