using System;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.DDL.CreateTable;

namespace ZenPlatform.Initializer.InternalDatabaseStructureMigrations
{
    [Migration(201808031249)]
    public class AddUserTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("user_id").AsGuid().PrimaryKey()
                .WithColumn("user_name").AsString(300)
                .WithColumn("is_active").AsBoolean();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }

    [Migration(201808031254)]
    public class AddConfigTableMigration : Migration
    {
        public override void Up()
        {
            Create.Table("saved_conf")
                .WithColumn("blob_name").AsString(200)
                .WithColumn("data").AsBinary();

            Create.Table("conf")
                .WithColumn("blob_name").AsString(200)
                .WithColumn("data").AsBinary();
        }

        public override void Down()
        {
            Delete.Table("saved_conf");
            Delete.Table("conf");
        }
    }


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
                        case SqlDatabaseType.SqlServer: rb.AddSqlServer2012(); break;
                        case SqlDatabaseType.MySql: rb.AddMySql5(); break;
                        case SqlDatabaseType.Oracle: rb.AddOracle(); break;
                        case SqlDatabaseType.Postgres: rb.AddPostgres(); break;
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


    [VersionTableMetaData]
    public class VersionTable : IVersionTableMetaData
    {
        public string ColumnName
        {
            get { return "Version"; }
        }

        public object ApplicationContext { get; set; }

        public bool OwnsSchema { get; }

        public string SchemaName
        {
            get { return ""; }
        }

        public string TableName
        {
            get { return "version"; }
        }

        public string UniqueIndexName
        {
            get { return "id"; }
        }

        public virtual string AppliedOnColumnName
        {
            get { return "applied_on"; }
        }

        public virtual string DescriptionColumnName
        {
            get { return "description"; }
        }
    }
}

