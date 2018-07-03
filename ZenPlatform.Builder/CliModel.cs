using System;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using ZenPlatform.Configuration.ConfigurationLoader.Structure;

namespace ZenPlatform.Builder
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

            var optionBuild = app.Option("-b|--build", "Build project xml file", CommandOptionType.SingleValue);

            Func<int> handler = () =>
            {
                if (optionVersion.HasValue())
                {
                    Console.WriteLine(
                        $"{Assembly.GetExecutingAssembly().GetName().Name} version {Assembly.GetExecutingAssembly().GetName().Version}");
                }

                if (optionBuild.HasValue())
                {
                    var projectFilePath = optionBuild.Value();

                    if (!File.Exists(projectFilePath))
                        throw new FileNotFoundException($"File not found: {projectFilePath}");

                    var root = XCRoot.Load(projectFilePath);

                    Console.WriteLine($"Success load project {projectFilePath}");
                    Console.WriteLine($"Start building");
                }

                return 0;
            };

            app.OnExecute(handler);


            return app.Execute(args);
        }
    }
}