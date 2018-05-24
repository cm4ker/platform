using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DocumentComponent.Configuration;

namespace ZenPlatform.Tests.Conf
{
    [TestClass]
    public class ConfLoadTest
    {
        private const string ConfigurationPath = "./Configuration";

        [TestMethod]
        public void RootLoad()
        {
            using (var tr = new StreamReader(Path.Combine(ConfigurationPath, "Project1.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
                var result = (XmlConfRoot)serializer.Deserialize(tr);

                Assert.AreNotEqual(null, result);
                Assert.AreEqual(typeof(XmlConfRoot), result.GetType());


                Assert.AreEqual("Управление библиотекой", result.ProjectName);
                Assert.AreEqual("0.0.0.1 Alpha", result.ProjectVersion);

                Assert.IsNotNull(result.Data);

                Assert.IsNotNull(result.Data.Components);

                Assert.AreEqual(5, result.Data.Components.Count);
                Assert.AreEqual(9, result.Data.IncludedFiles.Count);

                //var tableComponent = result.Data.Components.Find(x => x.Name == "Table");

                //Assert.AreEqual(2, tableComponent.Attaches.Count);

                //Assert.AreEqual(1, result.Modules.IncludedFiles.Count);

                //Assert.AreEqual(2, result.Languages.Count);
            }
        }

        [TestMethod]
        public void FullConfigurationLoad()
        {
            ConfigurationLoader cl = new ConfigurationLoader(Path.Combine(ConfigurationPath, "Project1.xml"));
            var root = cl.Load();
        }

    }
}
