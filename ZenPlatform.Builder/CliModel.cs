using System;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Cli
{
    public class CliBuilder
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

                var path = buildCmd.Argument("path", "path to the project file").IsRequired();

                buildCmd.OnExecute(() =>
                {
                    Console.WriteLine(path.Value);

                    var projectFilePath = path.Value;

                    if (!File.Exists(projectFilePath))
                        throw new FileNotFoundException($"File not found: {projectFilePath}");

                    XCFileSystemStorage ss = new XCFileSystemStorage(Path.GetDirectoryName(projectFilePath),
                        Path.GetFileName(projectFilePath));

                    var root = XCRoot.Load(ss);

                    Console.WriteLine($"Success load project {projectFilePath}");
                    Console.WriteLine($"Start building");
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
                    var serverOpt = dbCmd.Option("-s|--server", "database server", CommandOptionType.SingleValue);
                    var databaseOpt = dbCmd.Option("-d|--database", "database", CommandOptionType.SingleValue);
                    var userNameOpt = dbCmd.Option("-u|--username", "user name", CommandOptionType.SingleValue);
                    var passwordOpt = dbCmd.Option("-p|--password", "password", CommandOptionType.SingleValue);
                    var createOpt = dbCmd.Option("-c|--create", "Create database if not exists", CommandOptionType.NoValue);

                    dbCmd.OnExecute(() => { OnCreateDbCommand(projectNameArg.Value, serverOpt.Value(), databaseOpt.Value(), userNameOpt.Value(), passwordOpt.Value(), createOpt.HasValue()); });
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

        private static void OnCreateDbCommand(string projectName, string server, string database, string userName, string password, bool createIfNotExists)
        {
            Console.WriteLine($"Start creating new project {projectName}");
            Console.WriteLine($"Server: {server}\nDatabase {database}\nUsername {userName}\nPassword {password}");


            Console.WriteLine($"Done!");
        }
    }
}