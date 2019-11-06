using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Avalonia.Metadata;
using Portable.Xaml;

namespace ZenPlatform.UIBuilder
{
    /// <summary>
    /// Составитель XAML
    /// </summary>
    public class CustomXamlWriter
    {
        private readonly XmlWriter _writer;

        public CustomXamlWriter(XmlWriter writer)
        {
            _writer = writer;
        }

        public void StartObject()
        {
            _writer.WriteStartElement("someName");
        }

        public void EndObject()
        {
            _writer.WriteEndElement();
        }

        public void Value()
        {
            _writer.WriteValue(10);
        }

        public void StartProperty()
        {
            _writer.WriteStartAttribute("SomeAttributeName");
        }

        public void EndProperty()
        {
            _writer.WriteEndAttribute();
        }
    }

    public class MyXamlType : XamlType
    {
        private Type _underlyingType;

        public MyXamlType(Type underlyingType, XamlSchemaContext schemaContext) : base(NsResolver.Do(underlyingType), underlyingType.Name, null, schemaContext)
        {
            _underlyingType = underlyingType;
        }

        protected override Type LookupUnderlyingType()
        {
            return _underlyingType;
        }
    }

    public static class NsResolver
    {
        public static string Do(Type type)
        {
            var attr = type.Assembly.GetCustomAttributes<XmlnsDefinitionAttribute>().FirstOrDefault(x => x.ClrNamespace == type.Namespace);
            return attr?.XmlNamespace ?? "";
        }
    }
}
