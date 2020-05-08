using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Aquila.Configuration.Structure;
using Aquila.Configuration.Structure.Data;
using Aquila.ConfigurationExample;
using Aquila.DataComponent.Helpers;
using Aquila.EntityComponent.Configuration;
using Aquila.Shared.ParenChildCollection;

namespace Aquila.Tests.Component.BaseComponent
{
    public class BaseComponentTests
    {
        [Fact]
        public void TestXCPropertyHelper()
        {
            var conf = Factory.CreateExampleConfiguration();

            var componentType = conf.Data.ObjectTypes.First();

            var componentProperty = componentType.GetProperties().First();

            var actual = ColumnsHelper.GetColumnsFromProperty(componentProperty).Select(x => x.DatabaseColumnName)
                .ToList();
            var expected = new List<string> {"fld101_DateTime", "fld101_Binary", "fld101_Boolean", "fld101_Guid"};

            Assert.Equal(expected, actual);
        }
    }
}