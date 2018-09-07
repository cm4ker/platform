using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Cli;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Tests.Common;


namespace ZenPlatform.Tests.Conf
{
    [TestClass]
    public class ConfLoadTest
    {


        [TestMethod]
        public void RootLoad()
        {
            var conf = ExampleConfiguration.GetExample();

            //using (var tr = new StreamReader(Path.Combine(ConfigurationPath, "Project1.xml")))
            //{
            //    XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
            //    var result = (XmlConfRoot)serializer.Deserialize(tr);

            Assert.AreNotEqual(null, conf);
            Assert.AreEqual(typeof(XCRoot), conf.GetType());

            Assert.AreEqual("Управление библиотекой", conf.ProjectName);
            Assert.AreEqual("0.0.0.1 Alpha", conf.ProjectVersion);

            Assert.IsNotNull(conf.Data);
        }

        [TestMethod]
        public void FullConfigurationLoad()
        {
            //            ConfigurationLoader cl = new ConfigurationLoader(Path.Combine(ConfigurationPath, "Project1.xml"));
            //            var root = cl.Load();
        }

        [TestMethod]
        public void RootSaveLoadTest()
        {
            var conf = XCRoot.Create("TestProject");

            var xml = conf.Serialize();

            using (var sw = new StreamWriter("xml.xml"))
            {
                sw.Write(xml);
            }

            var restoredConf = XCHelper.Deserialize<XCRoot>(xml);

            Assert.AreEqual("TestProject", restoredConf.ProjectName);
        }
    }
}