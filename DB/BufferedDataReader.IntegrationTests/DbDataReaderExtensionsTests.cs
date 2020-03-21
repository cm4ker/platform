using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.IntegrationTests.Extensions;
using Xunit;

namespace BufferedDataReaderDotNet.IntegrationTests
{
    public class DbDataReaderExtensionsTests
    {
        public DbDataReaderExtensionsTests()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "(LocalDb)\\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true,
                PacketSize = short.MaxValue,
                TypeSystemVersion = "SQL Server 2012"
            };

            _connectionString = connectionStringBuilder.ToString();
        }

        private readonly string _connectionString;

        private static async Task<IEnumerable<string>> GetTableNamesAsync(
            SqlConnection connection, CancellationToken cancellationToken)
        {
            var tableNames = new List<string>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM [INFORMATION_SCHEMA].[TABLES] WHERE [TABLE_TYPE] = @TableType";
                command.Parameters.AddWithValue("@TableType", "BASE TABLE");

                using (var dataReader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        var schemaName = (string) dataReader["TABLE_SCHEMA"];
                        var tableName = (string) dataReader["TABLE_NAME"];

                        tableNames.Add($"{schemaName.QuoteName()}.{tableName.QuoteName()}");
                    }
                }
            }

            return tableNames;
        }

        private static string GetCommandText(IEnumerable<string> tableNames)
        {
            var stringBuilder = new StringBuilder();
            var values = $"(VALUES {string.Join(",", Enumerable.Range(1, 1).Select(i => $"({i})"))}) AS _(__)";

            foreach (var tableName in tableNames.OrderBy(s => s))
                stringBuilder.Append($"SELECT {tableName}.* FROM {tableName}, {values};{Environment.NewLine}");

            return stringBuilder.ToString();
        }

        [Fact]
        public async Task GetBufferedDataAsync_GetsBufferedData()
        {
            var cancellationToken = CancellationToken.None;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                var tableNames = await GetTableNamesAsync(connection, cancellationToken);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetCommandText(tableNames);

                    BufferedData bufferedData;

                    const CommandBehavior commandBehavior = CommandBehavior.SequentialAccess;

                    using (var dataReader = await command.ExecuteReaderAsync(commandBehavior, cancellationToken))
                        bufferedData = await dataReader.GetBufferedDataAsync(cancellationToken);

                    var z = new Stopwatch();
                    z.Start();


                    var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "File.bdr");

                    if (File.Exists(file))
                        File.Delete(file);

                    using (var fileStream = new FileStream(file, FileMode.Create))
                        await bufferedData.WriteToAsync(fileStream, cancellationToken);

                    var bufferedDataOptions = new BufferedDataOptions();

                    using (var fileStream = new FileStream(file, FileMode.Open))
                    {
                        using var x =
                            await BufferedData.ReadFromAsync(fileStream, bufferedDataOptions, cancellationToken);

                        var dataReader = x.GetDataReader();
                        while (dataReader.Read())
                        {
                        }
                    }

                    File.Delete(file);
                }
            }
        }
    }
}