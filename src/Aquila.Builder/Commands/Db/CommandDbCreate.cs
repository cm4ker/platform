using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SharpFileSystem.Database;
using Aquila.Data;
using Aquila.Data.Tools;
using Aquila.Initializer;
using Aquila.QueryBuilder;

namespace Aquila.Cli.Commands.Db
{
    [Command("create")]
    public class CommandDbCreate
    {
        [Option("--project_name", "Name of new project", CommandOptionType.SingleValue)]
        [Required]
        public string ProjectName { get; }

        [Option("-s|--server", "Database server", CommandOptionType.SingleValue)]
        public string Server { get; }

        [Option("-t|--type", "Type of database within will be create solution", CommandOptionType.SingleValue)]
        public SqlDatabaseType DatabaseType { get; }

        [Option("-d|--database", "Database", CommandOptionType.SingleValue)]
        public string Database { get; }

        [Option("-u|--username", "User name", CommandOptionType.SingleValue)]
        public string Username { get; }

        [Option("-p|--password", "Password", CommandOptionType.SingleValue)]
        public string Password { get; }

        [Option("--port ", "Database server port", CommandOptionType.SingleValue)]
        public int Port { get; }

        [Option("--connString", "Connection string", CommandOptionType.SingleValue)]
        public string ConnectionString { get; }

        [Option("-c|--create", "Create database if not exists", CommandOptionType.NoValue)]
        public bool Create { get; }

        public void OnExecute()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                OnCreateDbCommand(ProjectName, DatabaseType, Server,
                    Port, Database, Username, Password,
                    Create);
            else
                OnCreateDbCommand(ProjectName, DatabaseType, ConnectionString, Create);
        }

        private void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType,
            UniversalConnectionStringBuilder stringBuilder, bool createIfNotExists)
        {
            var connectionString = stringBuilder.GetConnectionString();

            //Мигрируем...
            MigrationRunner.Migrate(connectionString, databaseType);

            //Создаём пустой проект с именем Project Name

            //var newProject = new Project(null, null) {ProjectName = projectName};

            // Необходимо создать контекст данных

            var dataContext = new DataContext(databaseType, connectionString);

            var configStorage = new DatabaseFileSystem(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext);

            var configSaveStorage = new DatabaseFileSystem(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext);

            //Сохраняем новоиспечённый проект в сохранённую и конфигураци базы данных
            // newProject.Save(configStorage);
            // newProject.Save(configSaveStorage);

            Console.WriteLine($"Done!");
        }

        private void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string connectionString,
            bool createIfNotExists)
        {
            OnCreateDbCommand(projectName, databaseType,
                UniversalConnectionStringBuilder.FromConnectionString(databaseType, connectionString),
                createIfNotExists);
        }

        private void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string server, int port,
            string database, string userName, string password, bool createIfNotExists)
        {
            Console.WriteLine($"Start creating new project {projectName}");
            Console.WriteLine(
                $"DatabaseType: {databaseType}\nServer: {server}\nDatabase {database}\nUsername {userName}\nPassword {password}");
            var cb = new UniversalConnectionStringBuilder(databaseType);

            //После успешного созадания базы пробуем к ней подключиться, провести миграции и 
            //создать новую конфигурацию
            cb.Database = database;
            cb.Server = server;
            cb.Password = password;
            cb.Username = userName;
            cb.Port = port;

            Console.WriteLine(cb.GetConnectionString());

            OnCreateDbCommand(projectName, databaseType, cb, createIfNotExists);
        }
    }
}