using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.DataComponent.Helpers;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Tests.Common;

namespace ZenPlatform.Tests.Component.BaseComponent
{
    [TestClass]
    public class BaseComponentTests
    {
        [TestMethod]
        public void TestXCPropertyHelper()
        {
            var conf = ExampleConfiguration.GetExample();
            
            var componentType = conf.Data.ComponentTypes.First();

            var componentProperty = (componentType as XCSingleEntity).Properties.First();

            var actual = ColumnsHelper.GetColumnsFromProperty(componentProperty).Select(x => x.DatabaseColumnName).ToList();
            var expected = new List<string> { "Ref1", "Ref2", "Ref3" };

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
