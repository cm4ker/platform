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
using ZenPlatform.Configuration.Contracts.Migration;

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


            return false;
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

            var plan = new EntityMigrationPlan() ;
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);
             
            }

           

            var migrationId = Guid.NewGuid();
            var query = DDLQuery.New();

            var builder = new EntityMigrationPlanSQLBuilder(migrationId);

            builder.Build(plan, query);

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