using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Cli
{
    public static partial class CliBuilder
    {
        public static void BuildDbCommands(CommandLineApplication app)
        {
            //Команда построения проекта
            app.Command("db", buildCmd =>
            {
                buildCmd.HelpOption(inherited: true);

                buildCmd.Command("create", c => { });
                buildCmd.Command("delete", c => { });
                buildCmd.Command("attach", c => { });
                buildCmd.Command("detach", c => { });
                buildCmd.Command("backup", c => { });
                buildCmd.Command("list", c => { });
            });
        }

        public static void BuildKillCommand(CommandLineApplication app)
        {
            //Команда построения проекта
            app.Command("kill", buildCmd =>
            {
                buildCmd.HelpOption(inherited: true);

                var pathArgument = buildCmd.Argument("dbId", "Identifier  of the database").IsRequired();
                var buildPathArgument = buildCmd.Argument("sessionId", "Identifier of the session").IsRequired();
            });
        }

        public static void BuildInstanceCommands(CommandLineApplication app)
        {
            //Команда построения проекта
            app.Command("db", buildCmd =>
            {
                buildCmd.HelpOption(inherited: true);

                buildCmd.Command("create", c => { });
                buildCmd.Command("delete", c => { });
                buildCmd.Command("attach", c => { });
                buildCmd.Command("detach", c => { });
                buildCmd.Command("backup", c => { });
                buildCmd.Command("list", c => { });
            });
        }
    }
}