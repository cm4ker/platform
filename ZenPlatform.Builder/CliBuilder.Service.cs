using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace ZenPlatform.Cli
{
    public static partial class CliBuilder
    {
        public static void BuildServiceCommands(CommandLineApplication app)
        {
            app.HelpOption("-h|--help");

            var optionVersion = app.Option("-v|--version", "Version of the program",
                CommandOptionType.NoValue);

            int OptionsHandler()
            {
                if (optionVersion.HasValue())
                {
                    Console.WriteLine(
                        $"{Assembly.GetExecutingAssembly().GetName().Name} version {Assembly.GetExecutingAssembly().GetName().Version}");
                }

                return 0;
            }

            app.OnExecute(OptionsHandler);
        }
    }
}