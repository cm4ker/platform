using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Migrations;
using ZenPlatform.Migration;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.Component.Tests
{
    public class MigrationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MigrationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private XCRoot CreateConfiguration1()
        {
            XCRoot root = new XCRoot();

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var componentManager = (SingleEntityConfigurationManager)component.ComponentImpl.ComponentManager;


            var customType1 =
                componentManager.Create()
                    .SetName("customType")
                    .SetId(Guid.Parse("8f24498f-822f-4c0e-9b48-40f43aef3d62"))
                    .SetLinkId(Guid.Parse("205cf1b0-967b-4f65-acd3-86fdc0e06ecf"))
                    .SetDescription("customType")
                    .SetRealTableName("Obj_0000");

            var entity1 =
                componentManager.Create()
                    .SetName("store")
                    .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
                    .SetLinkId(Guid.Parse("0777428f-963a-4e6b-9b99-b60db19cad8a"))
                    .SetDescription("This is a store entity")
                    .SetRealTableName("Obj_0001");


            entity1.CreateProperty()
                .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
                .SetName("Property1")
                .AddType(new XCBinary())
                .SetDatabaseColumnName("Fld_0001");

            entity1.CreateProperty()
                .SetGuid(Guid.Parse("247b9ff2-636a-456b-bc4d-3150eb8ab4ea"))
                .SetName("Property2")
                .AddType(customType1.Link)
                .SetDatabaseColumnName("Fld_0002");


            return root;
        }

        private XCRoot CreateConfiguration2()
        {
            XCRoot root = new XCRoot();

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            var componentManager = (SingleEntityConfigurationManager)component.ComponentImpl.ComponentManager;


            var customType1 =
                componentManager.Create()
                    .SetName("customType")
                    .SetId(Guid.Parse("8f24498f-822f-4c0e-9b48-40f43aef3d62"))
                    .SetLinkId(Guid.Parse("205cf1b0-967b-4f65-acd3-86fdc0e06ecf"))
                    .SetDescription("customType")
                    .SetRealTableName("Obj_0000");

            var entity1 =
                componentManager.Create()
                    .SetName("store")
                    .SetId(Guid.Parse("42b828fe-1a33-4ad5-86d1-aaf6131a77d5"))
                    .SetLinkId(Guid.Parse("0777428f-963a-4e6b-9b99-b60db19cad8a"))
                    .SetDescription("This is a store entity")
                    .SetRealTableName("Obj_0001");


            entity1.CreateProperty()
                .SetGuid(Guid.Parse("252e804b-8c16-407a-8d3c-3c0e5bf461df"))
                .SetName("Property1")
                .AddType(new XCBinary())
                .AddType(new XCInt())
                .SetDatabaseColumnName("Fld_0001");

            entity1.CreateProperty()
                .SetGuid(Guid.Parse("247b9ff2-636a-456b-bc4d-3150eb8ab4ea"))
                .SetName("Property2")
                .AddType(customType1.Link)
                .SetDatabaseColumnName("Fld_0002");


            return root;
        }

        private XCRoot CreateEmptyConfiguration()
        {
            XCRoot root = new XCRoot();

            root.ProjectId = Guid.Parse("8d33de57-1971-405d-a7f3-a6c30d6b086a");
            root.ProjectName = "Library";
            root.ProjectVersion = "0.0.0.1";

            var component = new XCComponent()
            {
                ComponentAssembly = typeof(XCSingleEntity).Assembly,
            };

            root.Data.Components.Add(component);

            return root;
        }

        [Fact]
        public void CreateTest()
        {
            var empty = CreateEmptyConfiguration();
            var comfig = CreateConfiguration1();



            var components = empty.Data.Components.Join(comfig.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var tasks = new List<IMigrationTask>();
            foreach (var component in components)
            {
                var list = component.actual.ComponentImpl.Migrator.GetMigration(component.old, component.actual);
                tasks.AddRange(list);
            }

            Assert.Equal(2, tasks.Count);

            Assert.Contains("Create", tasks.First().Name);

        }

        [Fact]
        public void DeleteTest()
        {
            var empty = CreateEmptyConfiguration();
            var comfig = CreateConfiguration1();



            var components = comfig.Data.Components.Join(empty.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var tasks = new List<IMigrationTask>();
            foreach (var component in components)
            {
                var list = component.actual.ComponentImpl.Migrator.GetMigration(component.old, component.actual);
                tasks.AddRange(list);
            }

            Assert.Equal(2, tasks.Count);

            Assert.Contains("Delete", tasks.First().Name);

        }

        [Fact]
        public void ChangeTest()
        {
            var comfig1 = CreateConfiguration1();
            var comfig2 = CreateConfiguration2();



            var components = comfig1.Data.Components.Join(comfig2.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var tasks = new List<IMigrationTask>();
            foreach (var component in components)
            {
                var list = component.actual.ComponentImpl.Migrator.GetMigration(component.old, component.actual);
                tasks.AddRange(list);
            }

            Assert.Equal(4, tasks.Count);

            Assert.Contains("Copy", tasks.First().Name);

        }

        [Fact]
        public void IntegrationTest()
        {
            var comfig1 = CreateConfiguration1();
            var comfig2 = CreateConfiguration2();



            var components = comfig1.Data.Components.Join(comfig2.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var tasks = new List<IMigrationTask>();
            foreach (var component in components)
            {
                var list = component.actual.ComponentImpl.Migrator.GetMigration(component.old, component.actual);
                tasks.AddRange(list);
            }

            Assert.Equal(4, tasks.Count);

            DDLQuery q = DDLQuery.New();
            tasks.First(t => t.Name.Contains("Change")).Run(q);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(q.Expression);



        }


    }
}