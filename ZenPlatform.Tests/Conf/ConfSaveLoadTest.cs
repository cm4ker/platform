using System.IO;
using Xunit;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Tests.Common;


namespace ZenPlatform.Tests.Conf
{
    public class ConfSaveLoadTest
    {
        [Fact]
        public void RootLoad()
        {
            var conf = Factory.GetExampleConfigutaion();

            //using (var tr = new StreamReader(Path.Combine(ConfigurationPath, "Project1.xml")))
            //{
            //    XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
            //    var result = (XmlConfRoot)serializer.Deserialize(tr);

            Assert.NotNull(conf);
            Assert.Equal(typeof(XCRoot), conf.GetType());

            Assert.Equal("Управление библиотекой", conf.ProjectName);
            Assert.Equal("0.0.0.1 Alpha", conf.ProjectVersion);

            Assert.NotNull(conf.Data);
        }

        [Fact]
        public void FullConfigurationLoad()
        {
            //            ConfigurationLoader cl = new ConfigurationLoader(Path.Combine(ConfigurationPath, "Project1.xml"));
            //            var root = cl.Load();
        }

        [Fact]
        public void RootSaveLoadTest()
        {
            var conf = XCRoot.Create("TestProject");

            var xml = conf.Serialize();

            using (var sw = new StreamWriter("xml.xml"))
            {
                sw.Write(xml);
            }

            var restoredConf = XCHelper.Deserialize<XCRoot>(xml);

            Assert.Equal("TestProject", restoredConf.ProjectName);
        }

        [Fact]
        public void ConfSaveTest()
        {
            var conf = Common.Factory.GetExampleConfigutaion();
            var storage = new XCFileSystemStorage("C:\\Test\\", "Proj1.xml");
            conf.Save(storage);
        }
    }
}