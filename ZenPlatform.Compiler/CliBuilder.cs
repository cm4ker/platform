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
using ZenPlatform.QueryBuilder.DDL.CreateDatabase;

namespace ZenPlatform.Compiler
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

            return app.Execute(args);
        }


        private static void RegisterCompileCommand(CommandLineApplication app)
        {
            app.Command("compile", () => { })
        }

    }
}