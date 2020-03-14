using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Avalonia;
using Avalonia.Markup.Xaml;
using Dock.Avalonia.Controls;
using Portable.Xaml;
using Portable.Xaml.Markup;

namespace ZenPlatform.ThinClient.Views
{
    public class MyXamlSchemaContext : XamlSchemaContext
    {
        Dictionary<string, string> prefixes = new Dictionary<string, string>();

        private void FillPrefixes(Assembly ass)
        {
            foreach (var attribute in ass.GetCustomAttributes(typeof(XmlnsPrefixAttribute)))
            {
                var customAttribute = (XmlnsPrefixAttribute) attribute;
                this.prefixes.Add(customAttribute.XmlNamespace, customAttribute.Prefix);
            }
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public override string GetPreferredPrefix(string xmlns)
        {
            var pref = base.GetPreferredPrefix(xmlns);
            if (pref == "p")
                return RandomString(5);
            else
                return pref;
        }
    }

    public class DockMainWindow : MetroWindow
    {
        public DockMainWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            // TextWriter output = new StringWriter();
            // using (XmlWriter xmlWriter = XmlWriter.Create(output, new XmlWriterSettings()
            // {
            //     OmitXmlDeclaration = true,
            //     Indent = true
            // }))
            //
            //     XamlServices.Save(
            //         (XamlWriter) new XamlXmlWriter(xmlWriter, new MyXamlSchemaContext()),
            //         this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}