using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using Avalonia.Markup.Xaml.Context;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.PortableXaml;
using Avalonia.Markup.Xaml.Styling;

namespace ZenPlatform.Pidl.Xaml
{
    public interface IPlatformExtensionsResolver
    {
        Type Resolve(string platformNamespace, string name);
        void Register(string platformNamespace, string name, Type type);
    }
}