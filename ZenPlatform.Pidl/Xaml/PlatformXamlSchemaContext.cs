using System;
using Portable.Xaml;

namespace ZenPlatform.Pidl.Xaml
{
    /// <summary>
    /// Контекст схемы. При переопределении позволяет обрабатывать типы кастомно
    /// </summary>
    public class PlatformXamlSchemaContext : XamlSchemaContext
    {
        private readonly IPlatformExtensionsResolver _resolver;

        public PlatformXamlSchemaContext(IPlatformExtensionsResolver resolver)
        {
            _resolver = resolver;
        }

        protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
        {
            return GetXamlType(_resolver.Resolve(xamlNamespace, name));
        }

        public override XamlType GetXamlType(Type type)
        {
            return new XamlType(type, this);
        }
    }
}