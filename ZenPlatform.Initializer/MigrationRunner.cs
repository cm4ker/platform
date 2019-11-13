using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.QueryBuilder;


namespace ZenPlatform.Initializer
{
    /// <summary>
    /// Миграции. Используется стандартный пакет, котоырй теперь работает исключительно через DI контейнер
    /// </summary>
    public static class MigrationRunner
    {
        public static void Migrate(string connectionString, SqlDatabaseType dbType)
        {
            var serviceProvider = CreateServices(connectionString, dbType);

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }


        private static IServiceProvider CreateServices(string connectionString, SqlDatabaseType dbType)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    switch (dbType)
                    {
                        case SqlDatabaseType.SqlServer:
                            rb.AddSqlServer2012();
                            break;
                        case SqlDatabaseType.MySql:
                            rb.AddMySql5();
                            break;
                        case SqlDatabaseType.Oracle:
                            rb.AddOracle();
                            break;
                        case SqlDatabaseType.Postgres:
                            rb.AddPostgres();
                            break;
                    }

                    rb.WithGlobalConnectionString(connectionString);

                    rb.ScanIn(typeof(MigrationRunner).Assembly).For.Migrations();
                    rb.WithVersionTable(new VersionTable());
                })
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Обновить базу данных допоследней доступной миграции
        /// </summary>
        /// <param name="serviceProvider"></param>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();


            runner.MigrateUp();
        }
    }
}