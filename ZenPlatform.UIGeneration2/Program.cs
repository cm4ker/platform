using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Avalonia;
using Avalonia.Logging.Serilog;
using ZenPlatform.Shared.ParenChildCollection;


namespace ZenPlatform.UIGeneration2
{
    public class Program
    {
        public static void Main()
        {
            var window = new UIWindow().With(x => x.Group(UIGroupOrientation.Horizontal).With(g => g.TextBox()));
            window.Height = 100;
            window.Width = 2000;

            XamlUICompiler c = new XamlUICompiler();

            var sw = new StringWriter();
            var text = c.Compile(window, sw);

            Console.WriteLine(text);
            Console.Read();
        }
    }
}