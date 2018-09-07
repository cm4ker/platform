using System;
using System.IO;
using System.Reflection;
using FluentMigrator.Runner.Initialization;
using McMaster.Extensions.CommandLineUtils;
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
    public static class CliBuilder
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

            app.HelpOption("-h|--help");

            var optionVersion = app.Option("-v|--version", "Version of the program",
                CommandOptionType.NoValue);

            //Команда построения проекта
            app.Command("build", buildCmd =>
            {
                buildCmd.HelpOption(inherited: true);

                var pathArgument = buildCmd.Argument("path", "path to the project file").IsRequired();
                var buildPathArgument = buildCmd.Argument("buildPath", "path where will be copy compiled dlls");

                buildCmd.OnExecute(() =>
                {
                    Console.WriteLine(pathArgument.Value);

                    var projectFilePath = pathArgument.Value;

                    if (!File.Exists(projectFilePath))
                        throw new FileNotFoundException($"File not found: {projectFilePath}");

                    //Проверим каталог на существование
                    if (!Directory.Exists(buildPathArgument.Value))
                    {
                        Console.WriteLine("Build directory not found");
                        return;
                    }

                    XCFileSystemStorage ss = new XCFileSystemStorage(Path.GetDirectoryName(projectFilePath),
                        Path.GetFileName(projectFilePath));

                    var root = XCRoot.Load(ss);

                    Console.WriteLine($"Success load project {projectFilePath}");
                    Console.WriteLine($"Start building");

                    XCCompiller compiller = new XCCompiller(root, buildPathArgument.Value);
                    compiller.Build();

                });
            });

            //Команда создания проекта
            app.Command("create", createCmd =>
            {
                createCmd.HelpOption(inherited: true);

                var projectNameArg = createCmd.Argument("project_name", "Name of new project").IsRequired();

                createCmd.Command("fs", (fsCmd) =>
                {
                    //Если мы создаём проект для файловой системы, то мы обязаны  считать папку, в которой будет эта вся штука
                    fsCmd.Option("--path", "Path to the project location", CommandOptionType.SingleValue);
                });

                createCmd.Command("db", (dbCmd) =>
                {
                    var databaseTypeOpt = dbCmd.Option<SqlDatabaseType>("-t|--type",
                            "Type of database within will be create solution", CommandOptionType.SingleValue)
                        .Accepts(v => v.Enum<SqlDatabaseType>(true));

                    var serverOpt = dbCmd.Option("-s|--server", "database server", CommandOptionType.SingleValue);

                    var databaseOpt = dbCmd.Option("-d|--database", "database", CommandOptionType.SingleValue);

                    var userNameOpt = dbCmd.Option("-u|--username", "user name", CommandOptionType.SingleValue);

                    var passwordOpt = dbCmd.Option("-p|--password", "password", CommandOptionType.SingleValue);

                    var portOpt = dbCmd.Option<int>("--port ", "Database server port", CommandOptionType.SingleValue);

                    var createOpt = dbCmd.Option("-c|--create", "Create database if not exists",
                        CommandOptionType.NoValue);

                    dbCmd.OnExecute(() =>
                    {
                        OnCreateDbCommand(projectNameArg.Value, databaseTypeOpt.ParsedValue, serverOpt.Value(),
                            portOpt.ParsedValue, databaseOpt.Value(), userNameOpt.Value(), passwordOpt.Value(),
                            createOpt.HasValue());
                    });
                });
            });

            //Команда загрузки скомпиллированого проекта
            app.Command("deploy", deployCmd => { });

            //Команда публикации проекта, по своей сути она равна build + deploy
            app.Command("publish", publishCmd => { });

            Func<int> optionsHandler = () =>
            {
                if (optionVersion.HasValue())
                {
                    Console.WriteLine(
                        $"{Assembly.GetExecutingAssembly().GetName().Name} version {Assembly.GetExecutingAssembly().GetName().Version}");
                }

                return 0;
            };

            app.OnExecute(optionsHandler);


            return app.Execute(args);
        }

        private static void OnCreateDbCommand(string projectName, SqlDatabaseType databaseType, string server, int port,
            string database, string userName, string password, bool createIfNotExists)
        {
            Console.WriteLine($"Start creating new project {projectName}");
            Console.WriteLine(
                $"DatabaseType: {databaseType}\nServer: {server}\nDatabase {database}\nUsername {userName}\nPassword {password}");
            var cb = new UniversalConnectionStringBuilder(databaseType);

            // Если базы данных нет - её необходимо создать
            if (createIfNotExists)
            {
            }

            // После успешного созадания базы пробуем к ней подключиться, провести миграции и 
            //создать новую конфигурацию
            cb.Database = database;
            cb.Server = server;
            cb.Password = password;
            cb.Username = userName;
            cb.Port = port;

            Console.WriteLine(cb.GetConnectionString());

            //Мигрируем...
            MigrationRunner.Migrate(cb.GetConnectionString(), databaseType);

            //Создаём пустой проект с именем Project Name

            var newProject = XCRoot.Create(projectName);

            // Необходимо создать контекст данных

            var dataContext = new DataContext(databaseType, cb.GetConnectionString());

            var configStorage = new XCDatabaseStorage(DatabaseConstantNames.CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            var configSaveStorage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME, dataContext,
                SqlCompillerBase.FormEnum(databaseType));

            //Сохраняем новоиспечённый проект в сохранённую и конфигураци базы данных
            newProject.Save(configStorage);
            newProject.Save(configSaveStorage);

            Console.WriteLine($"Done!");
        }
    }
}