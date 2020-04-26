using System;
using Portable.Xaml;
using ZenPlatform.Avalonia.Wrapper;

namespace ZenPlatform.Runtime
{
    public static class UX
    {
        public static UXElement Parse(string xaml)
        {
            return (UXElement) XamlServices.Parse(xaml);
        }

        public static string Serialize(this UXElement ux)
        {
            return XamlServices.Save(ux);
        }
    }
}