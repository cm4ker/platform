using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{
    public static class XCHelper
    {
        public static T Deserialize<T>(this string content)
            where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            var xml = content.Trim('"');

            if (xml.StartsWith(_byteOrderMarkUtf8))
            {
                xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
            }

            using (var sr = new StringReader(xml))
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