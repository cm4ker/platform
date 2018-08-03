using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Cli;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader;
using ZenPlatform.Configuration.Structure;


namespace ZenPlatform.Tests.Conf
{
    [TestClass]
    public class ConfLoadTest
    {
        private const string ConfigurationPath = "../../../../Build/Debug/ExampleConfiguration/Configuration";

        [TestMethod]
        public void RootLoad()
        {
            var conf = XCRoot.Load(new XCFileSystemStorage(ConfigurationPath, "Project.xml"));

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
    }
}