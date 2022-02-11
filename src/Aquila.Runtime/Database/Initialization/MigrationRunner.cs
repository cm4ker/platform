using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Transactions;
using Aquila.Data;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Aquila.QueryBuilder;
using Microsoft.Data.SqlClient;
using Npgsql;


namespace Aquila.Initializer
{
    /// <summary>
    /// Migration runner
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
        /// Update internal structures to the last version
        /// </summary>
        /// <param name="serviceProvider"></param>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }

    public class DatabaseHelper
    {
        // Login failed is thrown when database does not exist (See Issue #776)
        // Unable to attach database file is thrown when file does not exist (See Issue #2810)
        // Unable to open the physical file is thrown when file does not exist (See Issue #2810)
        private static bool IsDoesNotExist(SqlException exception)
            => exception.Number == 4060 || exception.Number == 1832 || exception.Number == 5120;

        // Login failed is thrown when database does not exist (See Issue #776)
        private static bool IsDoesNotExist(PostgresException exception) => exception.SqlState == "3D000";

        public static bool ExistsSqlServer(string connectionString)
        {
            while (true)
            {
                var opened = false;
                var connection = DatabaseFactory.Get(SqlDatabaseType.SqlServer, connectionString);
                try
                {
                    using var _ = new TransactionScope(TransactionScopeOption.Suppress);

                    connection.Open();
                    opened = true;

                    var cmd = connection.CreateCommand();
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
                        connection.Close();
                    }
                }
            }
        }

        public static bool ExistsPostgres(string connectionString)
        {
            // When checking whether a database exists, pooling must be off, otherwise we may
            // attempt to reuse a pooled connection, which may be broken (this happened in the tests).
            // If Pooling is off, but Multiplexing is on - NpgsqlConnectionStringBuilder.Validate will throw,
            // so we turn off Multiplexing as well.
            var unpooledCsb = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Pooling = false,
                Multiplexing = false
            };

            using var _ =
                new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var unpooledRelationalConnection = new NpgsqlConnection(unpooledCsb.ToString());
            try
            {
                unpooledRelationalConnection.Open();


                return true;
            }
            catch (PostgresException e)
            {
                if (IsDoesNotExist(e))
                {
                    return false;
                }

                throw;
            }
            catch (NpgsqlException e) when (
                e.InnerException is IOException &&
                e.InnerException.InnerException is SocketException socketException &&
                socketException.SocketErrorCode == SocketError.ConnectionReset
            )
            {
                // Pretty awful hack around #104
                return false;
            }
            finally
            {
                unpooledRelationalConnection.Close();
                unpooledRelationalConnection.Dispose();
            }
        }

        public static bool Exists(string connectionString, SqlDatabaseType dbType)
        {
            return dbType switch
            {
                SqlDatabaseType.Unknown => throw new Exception("Db is UNKNOWN"),
                SqlDatabaseType.SqlServer => ExistsSqlServer(connectionString),
                SqlDatabaseType.Postgres => ExistsPostgres(connectionString),
                _ => throw new NotImplementedException()
            };
        }
    }
}