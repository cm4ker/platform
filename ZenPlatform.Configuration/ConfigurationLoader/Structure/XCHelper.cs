using System.IO;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.Structure
{
    internal static class XCHelper
    {
        public static T Deserialize<T>(this string content)
            where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(content.Trim('"')))
            {
                return (T) ser.Deserialize(sr);
            }
        }

        public static T DeserializeFromFile<T>(string fileName)
            where T : class
        {
            BaseDirectory = Path.GetDirectoryName(fileName);

            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (var sr = new StreamReader(fileName))
            {
                return (T) ser.Deserialize(sr);
            }
        }

        public static string BaseDirectory { get; private set; }

        public static string Serialize(this object obj)
        {
            using (var sw = new StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(obj.GetType());
                xs.Serialize(sw, obj);

                return sw.ToString();
            }
        }
    }
}