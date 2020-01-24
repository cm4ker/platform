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
using ZenPlatform.Configuration.Contracts.TypeSystem;

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

        private bool TryContinueMigration(Guid id, DataContext context)
        {
            using (var cmd = context.CreateCommand(qm =>
            {
                qm
                .bg_query()
                .m_from()
                    .ld_table("migration_status")
                .m_where()
                    .ld_const(true)
                    .ld_column("delete_table")
                    .eq()
                    .ld_const(false)
                    .ld_column("rename_table")
                    .eq()
                    .and()
                .m_select()
                    .ld_column("original_table")
                    .ld_column("temp_table")
                .st_query();
            }))
            {
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var plan = new EntityMigrationPlan();
                            while (reader.Read())
                            {
                                var original = reader.GetString(5);
                                var tmp = reader.GetString(6);

                                plan.AddScope(scope =>
                                {
                                    scope.RenameTable(tmp, original);
                                    scope.SetFlagRenameTable(tmp);
                                }, 40);
                            }

                            ExecPlan(plan, id, context);

                            CompliteMigration(context, id);

                            return true;
                        }
                        else
                        {
                            _logger.Error("Migration error, migration_status doesn't have rows to continue.");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Migration error, try continue");
                    throw ex;
                }
            }
        }

        private bool IfLastMigrationFail(out Guid migration_id, DataContext context)
        {
            migration_id = Guid.Empty;


            using (var cmd = context.CreateCommand(qm =>
            {
                qm
                .bg_query()
                .m_from()
                    .ld_table("migrations")
                .m_where()
                    .ld_const(false)

                    .ld_column("complited")
                    
                    .eq()
                .m_order_by()
                    .ld_column("datetime")
                    .desc()
                .m_select()
                    .ld_column("migration_id")
                .st_query();
            }))
            {
                try
                {
                    if (cmd.ExecuteScalar() is Guid id)
                    {
                        migration_id = id;
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Migration error, check complited");
                    throw ex;
                }
            }


            return false;
        }

        public Guid CreateMigration(DataContext context)
        {

            var id = Guid.NewGuid();

            using (var cmd = context.CreateCommand(qm =>
            {
                qm
                .bg_query()
                .m_values()
                    .ld_const(id)
                .m_insert()
                    .ld_table("migrations")
                    .ld_column("migration_id")
                .st_query();
            }))
            {
                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Migration error, create migration");
                    throw ex;
                }
            }

            return id;
        }

        public void CompliteMigration(DataContext context, Guid id)
        {


            using (var cmd = context.CreateCommand(qm =>
            {
                qm
                .bg_query()
                .m_where()
                    .ld_const(id)
                    .ld_column("migration_id")
                    .eq()
                .m_set()
                    .ld_column("complited")
                    .ld_const(true)  
                    .assign()
                .m_update()
                    .ld_table("migrations")
                .st_query();
            }))
            {
                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Migration error, complite migration");
                    throw ex;
                }
            }

        }

        private void ClearMigrationStatus(Guid id, DataContext context)
        {
            using (var cmd = context.CreateCommand(qm =>
            {
                qm
                .bg_query()
                .m_from()
                    .ld_table("migration_status")
                .m_where()
                    .ld_const(id)
                    .ld_column("migration_id")
                    .eq()
                .m_delete()
                .st_query();
            }))
            {
                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Migration error, clear migration status");
                    throw ex;
                }
            }
        }


        private void ExecPlan(EntityMigrationPlan plan, Guid id, DataContext context)
        {
            var query = DDLQuery.New();

            var builder = new EntityMigrationPlanSQLBuilder(id);

            builder.Build(plan, query);


            using (var cmd = context.CreateCommand(query.Expression))
            {
                try
                {
                    if (!string.IsNullOrEmpty(cmd.CommandText))
                    {
                        _logger.Debug("SQL Migration plan:\n{0}", cmd.CommandText);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Run migration plan error");
                    throw ex;
                }
            }
        }

        private void RunMigration(IXCRoot old, IXCRoot actual, Guid id, DataContext context)
        {
            var components = old.Data.Components.FullJoin(actual.Data.Components,
                c => c.Info.ComponentId,
                x => new { old = x, actual = default(IComponent) },
                x => new { old = default(IComponent), actual = x },
                (x, y) => new { old = x, actual = y });

            var plan = new EntityMigrationPlan();
            foreach (var component in components)
            {
                component.actual.ComponentImpl.Migrator.MigrationPlan(plan, component.old, component.actual);

            }
            if (plan.Count() > 0)
            {
                ExecPlan(plan, id, context);
            }
            else
            {
                _logger.Info("Migraion plan is empty.");
            }
        }

        public void Migrate(IXCRoot old, IXCRoot actual)
        {
            // Try 
            //_assemblyManager.BuildConfiguration(actual, _dataContextManager.DatabaseType);

            var context = _dataContextManager.GetContext();

            _logger.Info("Check last migration.");
            if (IfLastMigrationFail(out var last_fail_migration_id, context))
            {
                _logger.Info($"Migration '{last_fail_migration_id}' fail.");
                if (!TryContinueMigration(last_fail_migration_id, context))
                {
                    ClearMigrationStatus(last_fail_migration_id, context);
                    _logger.Info($"Restart migration '{last_fail_migration_id}'.");
                    RunMigration(old, actual, last_fail_migration_id, context);

                    CompliteMigration(context, last_fail_migration_id);
                    _logger.Info($"Migration '{last_fail_migration_id}' complite.");
                }
            } else
            {


                var id = CreateMigration(context);
                _logger.Info($"Create migration '{id}'");

                _logger.Info($"Run migration '{id}'.");
                RunMigration(old, actual, id, context);

                CompliteMigration(context, id);
                _logger.Info($"Migration '{id}' complite.");
            }
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