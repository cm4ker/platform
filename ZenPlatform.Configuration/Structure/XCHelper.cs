﻿using System.IO;
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

            var xml = content.Trim('"');

            using (var sr = new StringReader(xml))
            {
                return (T)ser.Deserialize(sr);
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
            XmlSerializer ser = new XmlSerializer(typeof(T));
            return (T)ser.Deserialize(stream);
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
        public static Stream SerializeToStream(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(obj.GetType());
                xs.Serialize(ms, obj);

                return ms;
            }
        }
    }
}