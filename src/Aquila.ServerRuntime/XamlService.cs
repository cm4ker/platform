using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.CodeAnalysis.Operations;
using Portable.Xaml;

namespace Aquila.ServerRuntime
{
    public static class XamlService
    {
        public static string Save(object instance)
        {
            StringWriter stringWriter = new StringWriter();
            XamlServices.Save((TextWriter) stringWriter, instance);
            return stringWriter.ToString();
        }

        public static void Save(Stream stream, object instance)
        {
            Stream output = stream;
            using (XmlWriter xmlWriter = XmlWriter.Create(output, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            }))
                XamlServices.Save(xmlWriter, instance);
        }

        public static void Save(TextWriter textWriter, object instance)
        {
            TextWriter output = textWriter;
            using (XmlWriter xmlWriter = XmlWriter.Create(output, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            }))
                XamlServices.Save(xmlWriter, instance);
        }

        public static void Save(XmlWriter xmlWriter, object instance)
        {
            XamlServices.Save((XamlWriter) new XamlXmlWriter(xmlWriter, new XamlSchemaContext()), instance);
        }

        public static void Save(XamlWriter xamlWriter, object instance)
        {
            if (xamlWriter == null)
                throw new ArgumentNullException(nameof(xamlWriter));
            XamlServices.Transform((XamlReader) new XamlObjectReader(instance, xamlWriter.SchemaContext), xamlWriter);
        }

        public static bool Matches(this AssemblyName name, AssemblyName other)
        {
            if (name == other)
            {
                return true;
            }

            if (name.Name != other.Name)
            {
                return false;
            }

            byte[] publicKeyToken = name.GetPublicKeyToken();
            byte[] publicKeyToken2 = name.GetPublicKeyToken();
            return publicKeyToken?.SequenceEqual(publicKeyToken2) ?? (publicKeyToken2 == null);
        }

        public static object Parse(string xaml)
        {
            return Load(new StringReader(xaml));
        }

        public static object Load(TextReader textReader)
        {
            return Load(new XamlXmlReader(textReader, new MyXamlSchemaContext()));
        }

        public static object Load(XamlReader xamlReader)
        {
            if (xamlReader == null)
            {
                throw new ArgumentNullException("xamlReader");
            }

            XamlObjectWriter xamlObjectWriter = new XamlObjectWriter(xamlReader.SchemaContext);

            Transform(xamlReader, xamlObjectWriter);
            return xamlObjectWriter.Result;
        }

        public static void Transform(XamlReader xamlReader, XamlWriter xamlWriter)
        {
            Transform(xamlReader, xamlWriter, closeWriter: true);
        }

        public static void Transform(XamlReader xamlReader, XamlWriter xamlWriter, bool closeWriter)
        {
            if (xamlReader == null)
            {
                throw new ArgumentNullException("xamlReader");
            }

            if (xamlWriter == null)
            {
                throw new ArgumentNullException("xamlWriter");
            }

            if (xamlReader.NodeType == XamlNodeType.None)
            {
                xamlReader.Read();
            }

            IXamlLineInfo xamlLineInfo = xamlReader as IXamlLineInfo;
            IXamlLineInfoConsumer xamlLineInfoConsumer = xamlWriter as IXamlLineInfoConsumer;
            bool flag = xamlLineInfo != null && xamlLineInfoConsumer != null &&
                        xamlLineInfoConsumer.ShouldProvideLineInfo && xamlLineInfo.HasLineInfo;
            while (!xamlReader.IsEof)
            {
                if (flag)
                {
                    xamlLineInfoConsumer.SetLineInfo(xamlLineInfo.LineNumber, xamlLineInfo.LinePosition);
                }

                xamlWriter.WriteNode(xamlReader);
                xamlReader.Read();
            }

            if (closeWriter)
            {
                xamlWriter.Close();
            }
        }
    }
}