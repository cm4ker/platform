using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.DataComponent.Helpers;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Tests.Common;

namespace ZenPlatform.Tests.Component.BaseComponent
{
    public class BaseComponentTests
    {
        [Fact]
        public void TestXCPropertyHelper()
        {
            var conf = Factory.GetExampleConfigutaion();

            var componentType = conf.Data.ComponentTypes.First();

            var componentProperty = componentType.GetProperties().First();

            var actual = ColumnsHelper.GetColumnsFromProperty(componentProperty).Select(x => x.DatabaseColumnName)
                .ToList();
            var expected = new List<string> {"fld101_DateTime", "fld101_Binary", "fld101_Boolean", "fld101_Guid"};

            Assert.Equal(expected, actual);
        }
    }
}