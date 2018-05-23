using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public static class XmlConfHelper
    {
        public static T Deserialize<T>(this string content)
            where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(content.Trim('"')))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static T DeserializeFromFile<T>(string fileName)
            where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (var sr = new StreamReader(fileName))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }

}
