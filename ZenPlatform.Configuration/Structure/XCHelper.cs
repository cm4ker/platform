using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Configuration;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.XmlSerializer;

namespace ZenPlatform.Configuration.Structure
{
    public static class XCHelper
    {
        public static T Deserialize<T>(this string content)
            where T : class
        {
            Serializer ser = new Serializer();

            var xml = content.Trim('"');

            using (var sr = new StringReader(xml))
            {
                return (T) ser.Deserialize(sr);
            }
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
            Serializer ser = new Serializer();
            return (T) ser.Deserialize(stream, BuildDefaultConfiguration());
        }

        public static string BaseDirectory { get; private set; }

        public static string Serialize(this object obj)
        {
            using (var sw = new StringWriter())
            {
                Serializer xs = new Serializer();
                xs.Serialize(obj, sw, BuildDefaultConfiguration());

                return sw.ToString();
            }
        }

        public static Stream SerializeToStream(this object obj)
        {
            var ms = new MemoryStream();

            var serializer = new ConfigurationContainer().Create();
            serializer.Serialize(XmlWriter.Create(ms), obj);

            return ms;
        }

        private static SerializerConfiguration BuildDefaultConfiguration()
        {
            return SerializerConfiguration.Create().CustomRoot<XCRoot>("Root")
                                                   .CustomRoot<XCComponent>("Component")
                                                   .IgnoreRootInProperties<XCData>()
                                                   .IgnoreRootInProperties<XCBlob>();
        }

        public static IConfigurationContainer UseXmlPlatformConfiguration(this IConfigurationContainer c)
        {
            return c;
        }
    }
}