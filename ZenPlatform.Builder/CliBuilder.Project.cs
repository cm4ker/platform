using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Cli
{
    public static partial class CliBuilder
    {
        public static void BuildProjectCommands(CommandLineApplication app)
        {
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


                    XCCompiller compiller = new XCCompiller();
                    
                    throw new NotImplementedException();
                    //TODO : Сделать возможность выбора одного из 3х: Сервер, Клиент, КлиентСервер
                    compiller.Build(root, CompilationMode.Server);
                });
            });

            //Команда создания проекта
            app.Command("create", createCmd =>
            {
                createCmd.HelpOption(inherited: true);

                //var projectNameArg = createCmd.Option("--project_name", "Name of new project", CommandOptionType.SingleValue);
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

                    var connectionString =
                        dbCmd.Option("--connString", "Connection string", CommandOptionType.SingleValue);

                    var createOpt = dbCmd.Option("-c|--create", "Create database if not exists",
                        CommandOptionType.NoValue);

                    dbCmd.OnExecute(() =>
                    {
                        if (string.IsNullOrEmpty(connectionString.Value()))
                            OnCreateDbCommand(projectNameArg.Value, databaseTypeOpt.ParsedValue, serverOpt.Value(),
                                portOpt.ParsedValue, databaseOpt.Value(), userNameOpt.Value(), passwordOpt.Value(),
                                createOpt.HasValue());
                        else
                            OnCreateDbCommand(projectNameArg.Value, databaseTypeOpt.ParsedValue,
                                connectionString.Value(), createOpt.HasValue());
                    });
                });
            });

            //Команда загрузки скомпиллированого проекта
            app.Command("deploy", deployCmd => { });

            //Команда публикации проекта, по своей сути она равна build + deploy
            app.Command("publish", publishCmd => { });

            //Команда удаления проекта
            app.Command("remove", publishCmd => { });
        }
    }
}