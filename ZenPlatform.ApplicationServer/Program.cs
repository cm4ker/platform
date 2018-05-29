using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZenPlatform.ApplicationServer
{
    class Program
    {
        static void Main()
        {
            var s = new XmlSerializer(typeof(XmlData));

            using (var sw = new StringWriter())
            {
                s.Serialize(sw, new XmlData() {Content = "Test"});
                Console.WriteLine(sw.ToString());
            }

            Console.ReadKey();
        }
    }


    [XmlType(AnonymousType = true)]
    public class XmlData
    {
        public string Content { get; set; }
    }
}