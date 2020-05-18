using System;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using SharpFileSystem.FileSystems;
using Xunit;
using Xunit.Abstractions;
using Aquila.Configuration;
using Aquila.Configuration.Common;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Configuration.Storage;
using Aquila.Configuration.Structure;
using Aquila.Configuration.Structure.Data.Types.Primitive;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration.Migration;
using Aquila.Core.Contracts.Data;
using Aquila.EntityComponent.Configuration;
using Aquila.Migration;
using Aquila.QueryBuilder.Builders;
using Aquila.QueryBuilder.Visitor;
using Aquila.Test.Tools;

namespace Aquila.Component.Tests
{
    public class MigrationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly MemoryFileSystem _mfs;
        private readonly IProject _example;

        public MigrationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mfs = new MemoryFileSystem();

            var example = ConfigurationFactory.Create();
            example.Save(_mfs);
        }

        private Project CreateConfigurationChangeDataType()
        {
            var proj = Project.Load(new MDManager(new TypeManager(), new InMemoryUniqueCounter()), _mfs);

            var comEdit = (ComponentEditor) proj.Editors.First(x => x is ComponentEditor);
            var typeEdit = comEdit.Editors.First();
            var propEdit = typeEdit.PropertyEditors.First();
            propEdit.SetType(MDTypes.String(10));

            return proj;
        }

        private Project CreateConfigurationAddDataType()
        {
            return null;
        }

        private Project CreateConfigurationAddDeleteNewProperty()
        {
            return null;
        }

        private Project CreateConfigurationAddDeleteEntityType()
        {
            return null;
        }

        private Project CreateEmptyConfiguration()
        {
            return null;
        }

        [Fact]
        public void CreateTest()
        {
            var empty = CreateEmptyConfiguration();

            var components = empty.TypeManager.Components.Join(_example.TypeManager.Components, c => c.Info.ComponentId,
                c => c.Info.ComponentId,
                (x, y) => new {old = x, actual = y});

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.old, component.actual);
            }

            Assert.Equal(2, plan.Count());

            Assert.IsType<CreateTableItem>(plan.First());
        }

        [Fact]
        public void DeleteTest()
        {
            var empty = CreateEmptyConfiguration();


            var components = _example.TypeManager.Components.Join(empty.TypeManager.Components, c => c.Info.ComponentId,
                c => c.Info.ComponentId,
                (x, y) => new {old = x, actual = y});

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            Assert.Equal(2, plan.Count());

            Assert.IsType<DeleteTableItem>(plan.First());
        }

        [Fact]
        public void ChangeDataTypeTest()
        {
            var comfig1 = _example;
            var comfig2 = CreateConfigurationChangeDataType();

            var components = comfig1.TypeManager.Components.Join(comfig2.TypeManager.Components,
                c => c.Info.ComponentId,
                c => c.Info.ComponentId,
                (x, y) => new {old = x, actual = y});

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.old, component.actual);
            }

            //Assert.Equal(4, tasks.Count);

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);

            DDLQuery query = DDLQuery.New();
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);


            Assert.Equal(
                "IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp\n ALTER COLUMN Fld_0001 INT NOT NULL;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n",
                sql);
        }

        [Fact]
        public void AddDeleteNewPropertyTest()
        {
            var comfig1 = _example;
            var comfig2 = CreateConfigurationAddDeleteNewProperty();


            var components = comfig1.TypeManager.Components.Join(comfig2.TypeManager.Components,
                c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new {old = x, actual = y});
            //ADD test
            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            DDLQuery query = DDLQuery.New();

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);

            Assert.Equal(
                "IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp\n ADD Fld_0003 INT NOT NULL;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n",
                sql);

            //DELETE test
            plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.actual, component.old);
            }

            query = DDLQuery.New();

            builder.Build(plan, query);

            sql = visitor.Visit(query.Expression);

            Assert.Equal(
                "IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp DROP COLUMN Fld_0003;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n",
                sql);
        }

        [Fact]
        public void AddDeleteNewEntityTest()
        {
            var comfig1 = _example;
            var comfig2 = CreateConfigurationAddDeleteEntityType();


            var components = comfig1.TypeManager.Components.Join(comfig2.TypeManager.Components,
                c => c.Info.ComponentId,
                c => c.Info.ComponentId,
                (x, y) => new {old = x, actual = y});
            //ADD test
            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            DDLQuery query = DDLQuery.New();

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);

            Assert.Equal(
                "IF OBJECT_ID('Obj_0002', 'U') IS NOT NULL\nDROP TABLE Obj_0002;\nCREATE TABLE Obj_0002 \n(\nId UNIQUEIDENTIFIER NOT NULL,\nName VARCHAR(150) NOT NULL,\nFld_0001 UNIQUEIDENTIFIER NOT NULL\n)",
                sql);

            //DELETE test

            plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.actual, component.old);
            }

            query = DDLQuery.New();

            builder.Build(plan, query);

            sql = visitor.Visit(query.Expression);

            Assert.Equal("DROP TABLE Obj_0002", sql);
        }

        [Fact]
        public void SingleToMultiDataTypeTest()
        {
            var comfig1 = _example;
            var comfig2 = CreateConfigurationAddDataType();


            var components = comfig1.TypeManager.Components.Join(comfig2.TypeManager.Components,
                c => c.Info.ComponentId,
                c => c.Info.ComponentId,
                (x, y) => new {old = x, actual = y});
            //ADD test
            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                if (component.actual.TryGetFeature<IMigrateable>(out var m))
                    m.Migrator.MigrationPlan(plan, component.old, component.actual);
            }


            DDLQuery query = DDLQuery.New();

            var migration_id = Guid.Parse("ed46b940-1176-4c46-9522-b42c44d92861");
            var builder = new EntityMigrationPlanSQLBuilder(migration_id);
            builder.Build(plan, query);

            var visitor = new SQLVisitorBase();


            var sql = visitor.Visit(query.Expression);


            Assert.Equal(
                "IF OBJECT_ID('Obj_0001_tmp', 'U') IS NOT NULL\nDROP TABLE Obj_0001_tmp;\nSELECT * INTO Obj_0001_tmp FROM Obj_0001;\nINSERT INTO migration_status(migration_id, temp_table, original_table, copy_table)\nVALUES\n('ed46b940-1176-4c46-9522-b42c44d92861', 'Obj_0001_tmp', 'Obj_0001', 1)\n;\nALTER TABLE Obj_0001_tmp\n ADD Fld_0001_Type INT;\nALTER TABLE Obj_0001_tmp\n ADD Fld_0001_Int INT NOT NULL;\nEXEC sp_rename 'Obj_0001_tmp.Fld_0001', 'Fld_0001_Binary', 'COLUMN';\nUPDATE Obj_0001_tmp\nSET Fld_0001_Type = 1\n;\nUPDATE migration_status\nSET change_table = 1\nWHERE\n'Obj_0001' = temp_table\n;\nDROP TABLE Obj_0001;\nUPDATE migration_status\nSET delete_table = 1\nWHERE\n'Obj_0001' = original_table\n;\nEXEC sp_rename 'Obj_0001_tmp', 'Obj_0001';\nUPDATE migration_status\nSET rename_table = 1\nWHERE\n'Obj_0001_tmp' = temp_table\n",
                sql);
        }
    }
}