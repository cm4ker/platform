using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Sdk;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Tests.Common;

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

        [Fact]
        public void TestEntityDtoLoad()
        {
            var conf = Factory.GetExampleConfigutaion();

            var entity = conf.Data.PlatformTypes.FirstOrDefault(x => x.Name == "ТестоваяСущность") as XCObjectTypeBase;
            var prop = entity.GetProperties().First();

            Assert.NotNull(entity);

            SingleEntityGenerator gen = new SingleEntityGenerator(entity.Parent);

            var extension = gen.GenerateExtension();
            var dto = gen.GenerateDtoClass(entity);
            var mainClass = gen.GenerateEntityClass(entity);
            var intf = gen.GenerateInterface();
            var helpers = gen.GenerateHelpersForEntity();

            var multidataStorage = gen.GenerateMultiDataStorage(entity, prop);

            var expected = "";

            //Assert.Equal(expected, result.ToString());
        }
    }
}