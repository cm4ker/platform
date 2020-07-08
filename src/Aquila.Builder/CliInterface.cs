using System;
using Aquila.Cli.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Aquila.Cli
{
    public class CliInterface : ICommandLineInterface
    {
        private IConsole _console;
        IServiceProvider _serviceProvider;

        public CliInterface(IConsole console, IServiceProvider serviceProvider)
        {
            _console = console;
            _serviceProvider = serviceProvider;
        }

        private int RunCommand<T>(string[] args) where T : class
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
                "deploy" => RunCommand<CommandDeploy>(args),
                _ => Unknown()
            };
        }
    }
}