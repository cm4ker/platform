using McMaster.Extensions.CommandLineUtils;
using System.IO;
using System.IO.Compression;
using Aquila.Core.Contracts.Instance;

namespace Aquila.Cli.Commands.Db
{
    [Command("deploy")]
    class CommandDbDeploy
    {
        [Argument(0, Name = "name", Description = "Database name")]
        public string Name { get; }

        [Option("-z ", "if file 7Zip", CommandOptionType.NoValue)]
        public bool Zip { get; }

        private IConsole _console;
        private IPlatformInstanceManager _instanceManager;

        public CommandDbDeploy(IConsole console, IPlatformInstanceManager instanceManager)
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

            if (Zip)
            {
                ZipFile.ExtractToDirectory(downloadFilePath, pathTo);
            }

            // var storage = new PhysicalFileSystem(pathTo);
            //
            //
            // var env = _environmentManager.GetEnvironment(Name);
            // if (env is IPlatformEnvironment platform)
            // {
            //     var databaseStorage = new DatabaseFileSystem(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME,
            //         platform.DataContextManager.GetContext());
            //
            //     //TODO: Save the configuration
            // }
        }
    }
}