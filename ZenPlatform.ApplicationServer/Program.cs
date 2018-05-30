using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.ApplicationServer
{
    class Program
    {
        static void Main()
        {

        }
    }

    [XmlType(AnonymousType = true)]
    public class XmlData
    {
        public string Content { get; set; }
    }
}