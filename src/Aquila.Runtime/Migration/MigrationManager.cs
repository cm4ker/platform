using System;
using System.ComponentModel;
using System.Linq;
using Aquila.Data;
using Aquila.Logging;
using Aquila.Metadata;
using Aquila.QueryBuilder.Builders;
using Aquila.Runtime;

namespace Aquila.Migrations
{
    public class MigrationManager
    {
        private readonly DataContextManager _dataContextManager;

        //private readonly IAssemblyManager _assemblyManager;
        //private readonly IConfigurationManipulator _m;
        private readonly ILogger _logger;

        public MigrationManager(DataContextManager dataContextManager, ILogger<MigrationManager> logger)
        {
            _dataContextManager = dataContextManager;
            _logger = logger;
        }


        public Guid CreateMigration(DataConnectionContext context)
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

        public void CompliteMigration(DataConnectionContext context, Guid id)
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

            var drc = DatabaseRuntimeContext.CreateAndLoad(context);
            drc.ApplyPendingChanges(context);
        }

        /// <summary>
        /// Checks the current state of database for migrations
        /// </summary>
        /// <param name="current"></param>
        /// <param name="pending"></param>
        /// <returns></returns>
        public bool CheckMigration()
        {
            var context = _dataContextManager.GetContext();
            var drc = DatabaseRuntimeContext.CreateAndLoad(context);
            return drc.PendingMetadata.GetMetadata().Metadata.Any();
        }

        public void Migrate()
        {
            var context = _dataContextManager.GetContext();

            _logger.Info("Check last migration.");

            if (IfLastMigrationFail(out var last_fail_migration_id, context))
            {
                _logger.Info($"Migration '{last_fail_migration_id}' fail.");
                if (!TryContinueMigration(last_fail_migration_id, context))
                {
                    ClearMigrationStatus(last_fail_migration_id, context);
                    _logger.Info($"Restart migration '{last_fail_migration_id}'.");
                    RunMigration(last_fail_migration_id, context);

                    CompliteMigration(context, last_fail_migration_id);
                    _logger.Info($"Migration '{last_fail_migration_id}' complite.");
                }
            }
            else
            {
                var id = CreateMigration(context);
                _logger.Info($"Create migration '{id}'");

                _logger.Info($"Run migration '{id}'.");
                RunMigration(id, context);

                CompliteMigration(context, id);
                _logger.Info($"Migration '{id}' complite.");
            }
        }

        private void ClearMigrationStatus(Guid id, DataConnectionContext context)
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

        private void ExecPlan(EntityMigrationPlan plan, Guid id, DataConnectionContext context)
        {
            var query = DDLQuery.New();

            var builder = new EntityMigrationPlanBuilder(id);

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

        private void RunMigration(Guid id, DataConnectionContext context)
        {
            var migrator = new EntityMigratorContext();
            var plan = new EntityMigrationPlan();

            //load instance of DatabaseRuntimeContext
            var runtimeContext = DatabaseRuntimeContext.CreateAndLoad(context);

            var migrationContext = new EntityMigratorDataContext(runtimeContext, context);

            migrator.MigrationPlan(plan, migrationContext);

            if (plan.Any())
            {
                ExecPlan(plan, id, context);
            }
            else
            {
                _logger.Info("Migration plan is empty.");
            }
        }

        private bool TryContinueMigration(Guid id, DataConnectionContext context)
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

        private bool IfLastMigrationFail(out Guid migration_id, DataConnectionContext context)
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
    }
}