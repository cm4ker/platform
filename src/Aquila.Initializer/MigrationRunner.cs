using System;
using System.Threading;
using System.Transactions;
using Aquila.Data;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Aquila.QueryBuilder;
using Microsoft.Data.SqlClient;


namespace Aquila.Initializer
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

    public class DatabaseWorking
    {
        // Login failed is thrown when database does not exist (See Issue #776)
        // Unable to attach database file is thrown when file does not exist (See Issue #2810)
        // Unable to open the physical file is thrown when file does not exist (See Issue #2810)
        private static bool IsDoesNotExist(SqlException exception)
            => exception.Number == 4060 || exception.Number == 1832 || exception.Number == 5120;

        private bool ExistsSqlServer(string cString)
        {
            while (true)
            {
                var opened = false;
                var _connection = DatabaseFactory.Get(SqlDatabaseType.SqlServer, cString);
                try
                {
                    using var _ = new TransactionScope(TransactionScopeOption.Suppress);

                    _connection.Open();
                    opened = true;

                    var cmd = _connection.CreateCommand();
                    cmd.CommandText = "SELECT 1";
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException e)
                {
                    if (IsDoesNotExist(e))
                    {
                        return false;
                    }

                    throw;
                }
                finally
                {
                    if (opened)
                    {
                        _connection.Close();
                    }
                }
            }
        }
    }
}