using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using FluentMigrator.Runner.Initialization;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Cli.Commands;
using ZenPlatform.Cli.Commands.Db;
//using ZenPlatform.Cli.Builder;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Data;
using ZenPlatform.Data.Tools;
using ZenPlatform.Initializer;
using ZenPlatform.Initializer.InternalDatabaseStructureMigrations;
using ZenPlatform.QueryBuilder;


namespace ZenPlatform.Cli
{

    public interface ICommandLineInterface
    {
        int Execute(string[] args);
    }


    public class McMasterCommandLineInterface: ICommandLineInterface
    {
        private IConsole _console;
        IServiceProvider _serviceProvider;
        public McMasterCommandLineInterface(IConsole console, IServiceProvider serviceProvider)
        {
            _console = console;
            _serviceProvider = serviceProvider;
        }

        private int RunCommand<T>(string[] args) where  T: class
        {
            var cmd = new CommandLineApplication<T>(false);
            cmd.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(_serviceProvider);
            

            return cmd.Execute(args.Length > 1 ? args[1..] : new string[0]);
        }

        private int Unknown()
        {
            _console.WriteLine("Unknown command");
            return 0;
        }
        public int Execute(string[] args)
        {


            return args[0] switch
            {
            "exit" => RunCommand<CommandExit>(args),
            "db" => RunCommand<CommandDb>(args),
            _ => Unknown()
            };
            
        }
    }

    public static partial class CliBuilder
    {
        public static void BuildApplicationServerCommands(CommandLineApplication app)
        {
            //Команда для подключения к серверу приложения в режиме интерпретатора
            app.Command("connect", publishCmd => { });
        }
    }

    public static partial class CliBuilder
    {
        /*
         * Для тулзы будут доступны следующие режимы
         * 
         * zenbuilder build "project filename"
         *
         * zenbuilder deploy "project filename" server "address" port "port" user "userName" password "password"
         *
         * zenbuilder run "project filename"
         *
         * zenbuilder run server "address" port "port" user "userName" password "password"
         * 
         */



        public static int Build(params string[] args)
        {
            var app = new CommandLineApplication();

            BuildProjectCommands(app);
            BuildServiceCommands(app);

            return app.Execute(args);
        }

        private static void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType,
            UniversalConnectionStringBuilder stringBuilder, bool createIfNotExists)
        {
            var connectionString = stringBuilder.GetConnectionString();

            // Если базы данных нет - её необходимо создать
            if (createIfNotExists)
            {
                var sqlCompiller = SqlCompillerBase.FormEnum(databaseType);

                DataContext dc = new DataContext(databaseType, connectionString);

                //CreateDatabaseQueryNode cDatabase = new CreateDatabaseQueryNode();
            }


            //Мигрируем...
            MigrationRunner.Migrate(connectionString, databaseType);

            //Создаём пустой проект с именем Project Name

            var newProject = XCRoot.Create(projectName);

            // Необходимо создать контекст данных

            var dataContext = new DataContext(databaseType, connectionString);

            var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            var configSaveStorage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            //Сохраняем новоиспечённый проект в сохранённую и конфигураци базы данных
            newProject.Save(configStorage);
            newProject.Save(configSaveStorage);

            Console.WriteLine($"Done!");
        }

        private static void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string connectionString,
            bool createIfNotExists)
        {
            OnCreateDbCommand(projectName, databaseType,
                UniversalConnectionStringBuilder.FromConnectionString(databaseType, connectionString),
                createIfNotExists);
        }

        private static void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string server, int port,
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