using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Sdk;
using Aquila.Configuration.Exceptions;
using Aquila.Configuration.Structure;
using Aquila.Configuration.Structure.Data;
using Aquila.Configuration.Structure.Data.Types.Complex;
using Aquila.ConfigurationExample;
using Aquila.EntityComponent.Configuration;
using Aquila.EntityComponent.Entity;
using Aquila.Shared.ParenChildCollection;


namespace Aquila.Tests.Component.EntityComponent
{
    public class EntityComponentTest
    {
        private const string filePath =
            "../../../../Build/Debug/ExampleConfiguration/Configuration/Data/Entity/TestEntity.xml";

        [Fact]
        public void TestEntityLoad()
        {
            XCSingleEntity conf;
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                conf = XCHelper.DeserializeFromStream<XCSingleEntity>(stream);
            }

            if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
            if (conf.Guid == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");

            Assert.Equal("ТестоваяСущность", conf.Name);
            Assert.True(conf.Properties.Any());
        }
    }
}