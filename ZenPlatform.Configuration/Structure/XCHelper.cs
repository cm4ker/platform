using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Portable.Xaml;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Core.Crypto;

namespace ZenPlatform.Configuration.Structure
{
    public static class XCHelper
    {
        public static T Deserialize<T>(this string content)
            where T : class
        {
            throw new Exception();
        }

        public static T DeserializeFromFile<T>(string fileName)
            where T : class
        {
            BaseDirectory = Path.GetDirectoryName(fileName);
            using (var sr = File.Open(fileName, FileMode.Open))
            {
                return DeserializeFromStream<T>(sr);
            }
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            XamlSchemaContext context = new XCXamlSchemaContext();

            XamlObjectWriter writer = new XamlObjectWriter(context);
            XamlXmlReader reader = new XamlXmlReader(stream, context);
            XamlServices.Transform(reader, writer);

            return (T) writer.Result;
        }

        public static string BaseDirectory { get; private set; }

        public static string Serialize(this object obj)
        {
            using (var sw = new StringWriter())
            {
                XamlSchemaContext context = new XCXamlSchemaContext();

                XamlObjectReader reader = new XamlObjectReader(obj, context);
                XamlXmlWriter writer = new XamlXmlWriter(sw, context);
                XamlServices.Transform(reader, writer);

                return sw.ToString();
            }
        }

        public static Stream SerializeToStream(this object obj)
        {
            MemoryStream ms = new MemoryStream();
            XamlSchemaContext context = new XCXamlSchemaContext();

            XamlObjectReader reader = new XamlObjectReader(obj, context);
            XamlXmlWriter writer = new XamlXmlWriter(ms, context);
            XamlServices.Transform(reader, writer);

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public static string GetHash(this IXCRoot root)
        {
            return HashHelper.HashMD5(root.SerializeToStream());
        }
    }
}