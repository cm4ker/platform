using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.Migration
{
    public class MigrationManager : IMigrationManager
    {
        private readonly IDataContextManager _dataContextManager;
        private readonly IAssemblyManager _assemblyManager;
        private readonly IConfigurationManipulator _m;
        private readonly ILogger _logger;

        public MigrationManager(IDataContextManager dataContextManager, IAssemblyManager assemblyManager,
            IConfigurationManipulator m, ILogger<MigrationManager> logger)
        {
            _assemblyManager = assemblyManager;
            _m = m;
            _dataContextManager = dataContextManager;
            _logger = logger;
        }

        private void RollBack(Guid migrationId)
        {

        }

        private bool IsNotComplitLastMigration(out Guid migration_id)
        {
            migration_id = Guid.Empty;
            var context = _dataContextManager.GetContext();

            using (var cmd = context.CreateCommand(m=>
            {
                m
                .bg_query()
                .m_from()
                .ld_table("migration")
                .m_where()
                .ld_column("complited")
                .ld_const(false)
                .eq()
                .m_select()
                .ld_column("id")
                .st_query();
            }))
            {
                try
                {
                   
                    var id = cmd.ExecuteScalar();
                    if (id == null)
                        return false;

                    return Guid.TryParse((string)id, out migration_id);

                    
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Check last migration error");
                    throw new Exception("Check last migration error");
                }

            }
        }

        public void Migrate(IXCRoot old, IXCRoot actual)
        {
            _assemblyManager.BuildConfiguration(actual, _dataContextManager.DatabaseType);


            if (IsNotComplitLastMigration(out var migration_id))
            {
                RollBack(migration_id);
            }

            _logger.Info("Start migration.");
            var components = old.Data.Components.Join(actual.Data.Components, c => c.Info.ComponentId, c => c.Info.ComponentId,
                (x, y) => new { old = x, actual = y });

            var tasks = new List<IMigrationTask>();
            foreach (var component in components)
            {
                var list = component.actual.ComponentImpl.Migrator.GetMigration(component.old, component.actual);
                tasks.AddRange(list);
            }

            var orderTasks = tasks.OrderBy(t => t.Step);

            var migrationId = Guid.NewGuid();
            var query = DDLQuery.New();

            foreach (var task in orderTasks)
            {
                var rollback = DDLQuery.New();
                task.RollBack(rollback);

                query.Add(m =>
                {
                    m
                    .bg_query()
                    .m_values()
                    .ld_const(migrationId)
                    .ld_const(task.Id)
                    .ld_const(task.Name)
                    .ld_const(false)
                    .ld_const(_dataContextManager.SqlCompiler.Compile(rollback.Expression))
                    
                    .m_insert()
                    .ld_table("migration_task")
                    .ld_column("migration_id")
                    .ld_column("id")
                    .ld_column("name")
                    .ld_const("complited")
                    .ld_column("rollback")
                    .st_query();

                });
                task.Run(query);

                query.Add(m =>
                {
                    m
                    .bg_query()
                    .m_where()
                        .ld_column("id")
                        .ld_const(task.Id)
                        .eq()
                        .ld_column("migration_id")
                        .ld_const(migrationId)
                        .eq()
                        .and()
                    .m_set()
                        .ld_column("complited")
                        .ld_const(true)
                        .assign()
                    .m_update()
                        .ld_table("migration_task")

                    .st_query();

                });
            }
            
            var context = _dataContextManager.GetContext();

            using (var cmd = context.CreateCommand(query.Expression))
            {
                try
                {
                    if (!string.IsNullOrEmpty(cmd.CommandText))
                        cmd.ExecuteNonQuery();
                } catch (Exception ex)
                {
                    _logger.Error(ex, "Migration error");
                }
            }

            _logger.Info("Migration complite.");
        }


        /// <summary>
        /// Проверяет нужно ли выполнять миграцию, если False - то миграцию выполнять не нужно.
        /// </summary>
        /// <param name="old"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public bool CheckMigration(IXCRoot old, IXCRoot actual)
        {
            return !_m.Equals(old, actual);
        }
    }
}