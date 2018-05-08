using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Core;
using ZenPlatform.DocumentComponent;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class ConfigurationTesting
    {
        private PRootConfiguration CreateConfiguration()
        {
            var conf = new PRootConfiguration();
            conf.RegisterDataComponent(ConfigurationFactory.CreateDocumentComponent());
            conf.ConfigurationName = "SimpleTestConfiguration";

            return conf;
        }

        [TestMethod]
        public void ConfigurationUnloadLoadTesting()
        {
            var confManager = new ConfigurationManager();
            confManager.Unload(CreateConfiguration(), AppDomain.CurrentDomain.BaseDirectory + "\\SimpleConf");
            var conf = confManager.Load(AppDomain.CurrentDomain.BaseDirectory +
                                        "\\SimpleConf\\SimpleTestConfiguration.json");
        }

        [TestMethod]
        public void SimpleTest()
        {
//            PlatformEnvironment e = new PlatformEnvironment();
//
//            var pcomponent = ConfigurationFactory.CreateDocumentComponent();
//
//            e.RegisterEntity(new EntityDefinition(pcomponent.Objects.ToArray()[1], typeof(ContractorEntity), typeof(ContractorDto)));
//            e.RegisterEntity(new EntityDefinition(pcomponent.Objects.ToArray()[0], typeof(InvoiceEntity), typeof(InvoiceDto)));
//
//            e.RegisterManager(typeof(ContractorEntity), new DocumentManager());
//            e.RegisterManager(typeof(InvoiceEntity), new DocumentManager());
//
//            var session = e.CreateSession();
//
//            var invoice = session.Document().Invoice.Load("someKey");
        }
    }
}