using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Configuration.Contracts.Migration;
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
using ZenPlatform.Test.Tools;
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

        private XCRoot CreateConfiguration()
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

        private XCRoot CreateConfigurationChangeDataType()
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
                .AddType(new XCInt())
                .SetDatabaseColumnName("Fld_0001");

            entity1.CreateProperty()
                .SetGuid(Guid.Parse("247b9ff2-636a-456b-bc4d-3150eb8ab4ea"))
                .SetName("Property2")
                .AddType(customType1.Link)
                .SetDatabaseColumnName("Fld_0002");


            return root;
        }

        private XCRoot CreateConfigurationAddDataType()
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

        private XCRoot CreateConfigurationAddDeleteNewProperty()
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

            entity1.CreateProperty()
                .SetGuid(Guid.Parse("600bfe3a-2af8-4679-b068-8b270a07f5f3"))
                .SetName("Property3")
                .AddType(new XCInt())
                .SetDatabaseColumnName("Fld_0003");


            return root;
        }

        private XCRoot CreateConfigurationAddDeleteEntityType()
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


            var entity2 =
                componentManager.Create()
                    .SetName("invoice")
                    .SetId(Guid.Parse("134657f5-6d17-487d-a97d-7d5f71797f8f"))
                    .SetLinkId(Guid.Parse("969ba36c-6e91-4c1d-a39b-fa6a5f587201"))
                    .SetDescription("This is a invoice entity")
                    .SetRealTableName("Obj_0002");

            entity2.CreateProperty()
                .SetGuid(Guid.Parse("e70e6f1b-40f6-4bd1-b2f3-a8cde865d8ca"))
                .SetName("Property1")
                .AddType(entity1.Link)
                .SetDatabaseColumnName("Fld_0001");


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
            var comfig = CreateConfiguration();



            var components = empty.Data.Components.Join(comfig.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
            }

            Assert.Equal(2, plan.Count());

            Assert.IsType<CreateTableItem>(plan.First());
           
        }
        
        [Fact]
        public void DeleteTest()
        {
            var empty = CreateEmptyConfiguration();
            var comfig = CreateConfiguration();



            var components = comfig.Data.Components.Join(empty.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            Assert.Equal(2, plan.Count());

            Assert.IsType<DeleteTableItem>(plan.First());

        }
        
        [Fact]
        public void ChangeDataTypeTest()
        {
            var comfig1 = CreateConfiguration();
            var comfig2 = CreateConfigurationChangeDataType();



            var components = comfig1.Data.Components.Join(comfig2.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
            }

            //Assert.Equal(4, tasks.Count);

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);

            DDLQuery query = DDLQuery.New();
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);


            Assert.Equal("IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp\n ALTER COLUMN Fld_0001 INT NOT NULL;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n", sql);

        }
        
        [Fact]
        public void AddDeleteNewPropertyTest()
        {
            var comfig1 = CreateConfiguration();
            var comfig2 = CreateConfigurationAddDeleteNewProperty();



            var components = comfig1.Data.Components.Join(comfig2.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });
            //ADD test
            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            DDLQuery query = DDLQuery.New();

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);

            Assert.Equal("IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp\n ADD Fld_0003 INT NOT NULL;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n", sql);

            //DELETE test
            plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.actual, component.old);
            }

            query = DDLQuery.New();

            builder.Build(plan, query);

            sql = visitor.Visit(query.Expression);

            Assert.Equal("IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp DROP COLUMN Fld_0003;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n", sql);

        }
        
        [Fact]
        public void AddDeleteNewEntityTest()
        {
            var comfig1 = CreateConfiguration();
            var comfig2 = CreateConfigurationAddDeleteEntityType();



            var components = comfig1.Data.Components.Join(comfig2.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });
            //ADD test
            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            DDLQuery query = DDLQuery.New();

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);

            Assert.Equal("IF OBJECT_ID('Obj_0002', 'U') IS NOT NULL\nDROP TABLE Obj_0002;\nCREATE TABLE Obj_0002 \n(\nId UNIQUEIDENTIFIER NOT NULL,\nName VARCHAR(150) NOT NULL,\nFld_0001 UNIQUEIDENTIFIER NOT NULL\n)", sql);

            //DELETE test

            plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.actual, component.old);
            }

            query = DDLQuery.New();

            builder.Build(plan, query);

            sql = visitor.Visit(query.Expression);

            Assert.Equal("DROP TABLE Obj_0002", sql);




        }

        [Fact]
        public void SingleToMultiDataTypeTest()
        {
            var comfig1 = CreateConfiguration();
            var comfig2 = CreateConfigurationAddDataType();



            var components = comfig1.Data.Components.Join(comfig2.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });
            //ADD test
            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            DDLQuery query = DDLQuery.New();

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);


            Assert.Equal("IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp\n ADD Fld_0001_Type INT;\nALTER TABLE Obj_0001_tmp\n ADD Fld_0001_Int INT NOT NULL;\nEXEC sp_rename 'Obj_0001_tmp.Fld_0001', 'Fld_0001_Binary', 'COLUMN';\nUPDATE Obj_0001_tmp\nSET Fld_0001_Type = 1\n;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n", sql);





        }
    }
}