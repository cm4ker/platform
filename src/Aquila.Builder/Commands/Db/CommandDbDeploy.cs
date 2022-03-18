using McMaster.Extensions.CommandLineUtils;
using System.IO;
using System.IO.Compression;
using Aquila.Core.Instance;

namespace Aquila.Cli.Commands.Db
{
    [Command("deploy")]
    class CommandDbDeploy
    {
        [Argument(0, Name = "name", Description = "Database name")]
        public string Name { get; }

        [Option("-pkg ", "Package for deploy", CommandOptionType.NoValue)]
        public bool Package { get; }

        [Option("-s ", "Server", CommandOptionType.NoValue)]
        public string Server { get; }

        [Option("-i ", "Instance", CommandOptionType.NoValue)]
        public string Insatnce { get; }

        private IConsole _console;
        private AqInstanceManager _instanceManager;

        public CommandDbDeploy(IConsole console, AqInstanceManager instanceManager)
        {
            _console = console;
            _instanceManager = instanceManager;
        }

        public async void OnExecute()
        {
            var downloadFilePath = Path.GetTempFileName();

            using (var downloadFile = new StreamWriter(new FileStream(downloadFilePath, FileMode.OpenOrCreate)))
            {
                downloadFile.Write(await _console.In.ReadToEndAsync());
            }

            var pathTo = Path.GetTempFileName();
        }
    }
}