using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Tests.Component.EntityComponent
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