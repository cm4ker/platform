﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
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
            var conf = XCRoot.Load(Path.Combine(ConfigurationPath, "Project1.xml"));

            //using (var tr = new StreamReader(Path.Combine(ConfigurationPath, "Project1.xml")))
            //{
            //    XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
            //    var result = (XmlConfRoot)serializer.Deserialize(tr);

            Assert.AreNotEqual(null, conf);
            Assert.AreEqual(typeof(XCRoot), conf.GetType());

            Assert.AreEqual("Управление библиотекой", conf.ProjectName);
            Assert.AreEqual("0.0.0.1 Alpha", conf.ProjectVersion);

            Assert.IsNotNull(conf.Data);

            Assert.IsNotNull(conf.Data.Components);

//            Assert.AreEqual(5, conf.Data.Components.Count);
//            Assert.AreEqual(9, conf.Data.IncludedFiles.Count);

            //var tableComponent = result.Data.Components.Find(x => x.Name == "Table");

            //Assert.AreEqual(2, tableComponent.Attaches.Count);

            //Assert.AreEqual(1, result.Modules.IncludedFiles.Count);

            //Assert.AreEqual(2, result.Languages.Count);
            //}
        }

        [TestMethod]
        public void FullConfigurationLoad()
        {
//            ConfigurationLoader cl = new ConfigurationLoader(Path.Combine(ConfigurationPath, "Project1.xml"));
//            var root = cl.Load();
        }

    }
}
