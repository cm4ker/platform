using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Editors;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
using Root = ZenPlatform.Configuration.Structure.Root;

namespace ZenPlatform.Component.Tests
{
    public class TypeEqualsTest
    {

        private Root _root;
        private XCComponent _component;
        private SingleEntityConfigurationManager _componentManager;
        private ITypeEditor Entity1;
        private ITypeEditor Entity2;
        public TypeEqualsTest()
        {
            _root = new Root();

            _root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
            _root.ProjectName = "Library";
            _root.ProjectVersion = "0.0.0.1";

            _component = new XCComponent()
            {
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            _root.Data.Components.Add(_component);

            _componentManager  = (SingleEntityConfigurationManager)_component.ComponentImpl.ComponentManager;


            var customType1 =
                _componentManager.Create()
                    .SetName("customType")
                    .SetId(Guid.Parse("8f24498f-822f-4c0e-9b48-40f43aef3d62"))
                    .SetLinkId(Guid.Parse("205cf1b0-967b-4f65-acd3-86fdc0e06ecf"))
                    .SetDescription("customType")
                    .SetRealTableName("Obj_0000");

            Entity1 =
                _componentManager.Create()
                    .SetName("store")
                    .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
                    .SetLinkId(Guid.Parse("0777428f-963a-4e6b-9b99-b60db19cad8a"))
                    .SetDescription("This is a store entity")
                    .SetRealTableName("Obj_0001");


            Entity1.CreateProperty()
                .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
                .SetName("Property1")
                .AddType(new XCBinary())
                .SetDatabaseColumnName("Fld_0001");

            Entity1.CreateProperty()
                .SetGuid(Guid.Parse("247b9ff2-636a-456b-bc4d-3150eb8ab4ea"))
                .SetName("Property2")
                .AddType(customType1.Link)
                .SetDatabaseColumnName("Fld_0002");


            var customType2 =
                _componentManager.Create()
                    .SetName("customType")
                    .SetId(Guid.Parse("8f24498f-822f-4c0e-9b48-40f43aef3d62"))
                    .SetLinkId(Guid.Parse("205cf1b0-967b-4f65-acd3-86fdc0e06ecf"))
                    .SetDescription("customType")
                    .SetRealTableName("Obj_0000");

            Entity2 =
                _componentManager.Create()
                    .SetName("store")
                    .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
                    .SetLinkId(Guid.Parse("0777428f-963a-4e6b-9b99-b60db19cad8a"))
                    .SetDescription("This is a store entity")
                    .SetRealTableName("Obj_0001");


            Entity2.CreateProperty()
                .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
                .SetName("Property1")
                .AddType(new XCBinary())
                .SetDatabaseColumnName("Fld_0001");

            Entity2.CreateProperty()
                .SetGuid(Guid.Parse("247b9ff2-636a-456b-bc4d-3150eb8ab4ea"))
                .SetName("Property2")
                .AddType(customType2.Link)
                .SetDatabaseColumnName("Fld_0002");
        }

        [Fact]
        public void XCSingleEntityEquals()
        {
           
            Assert.Equal(Entity1.Type, Entity2.Type);
        }

        [Fact]
        public void XCSingleEntityLinkEquals()
        {
            Assert.Equal(Entity1.Link, Entity2.Link);
        }

        [Fact]
        public void XCSingleEntityPropertyEquals()
        {
            var Id1 = Entity1.Type.GetProperties().FirstOrDefault(x => x.Name == "Id");
            Assert.NotNull(Id1);
            var Id2 = Entity2.Type.GetProperties().FirstOrDefault(x => x.Name == "Id");
            Assert.NotNull(Id2);
            Assert.Equal(Id1, Id2);

            var name1 = Entity1.Type.GetProperties().FirstOrDefault(x => x.Name == "Name");
            Assert.NotNull(name1);
            var name2 = Entity2.Type.GetProperties().FirstOrDefault(x => x.Name == "Name");
            Assert.NotNull(name2);
            Assert.Equal(name1, name2);

            var link1 = Entity1.Type.GetProperties().FirstOrDefault(x => x.Name == "Link");
            Assert.NotNull(link1);
            var link2 = Entity2.Type.GetProperties().FirstOrDefault(x => x.Name == "Link");
            Assert.NotNull(link2);
            Assert.Equal(link1, link2);

            var Property1_1 = Entity1.Type.GetProperties().FirstOrDefault(x => x.Name == "Property1");
            Assert.NotNull(Property1_1);
            var Property1_2 = Entity2.Type.GetProperties().FirstOrDefault(x => x.Name == "Property1");
            Assert.NotNull(Property1_2);
            Assert.Equal(Property1_1, Property1_2);

            var Property2_1 = Entity1.Type.GetProperties().FirstOrDefault(x => x.Name == "Property2");
            Assert.NotNull(Property2_1);
            var Property2_2 = Entity2.Type.GetProperties().FirstOrDefault(x => x.Name == "Property2");
            Assert.NotNull(Property2_2);
            Assert.Equal(Property2_1, Property2_2);





        }
    }
}
