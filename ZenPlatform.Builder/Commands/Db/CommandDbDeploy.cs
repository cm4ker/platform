using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Environment;
using System.IO.Compression;
using ZenPlatform.Initializer;

namespace ZenPlatform.Cli.Commands.Db
{
    [Command("deploy")]
    class CommandDbDeploy
    {
        [Argument(0,Name = "name", Description = "Database name")]
        public string Name { get; }

        [Option("-z ", "if file 7Zip", CommandOptionType.NoValue)]
        public bool Zip { get; }

        private IConsole _console;
        private IPlatformEnvironmentManager _environmentManager;
        public CommandDbDeploy(IConsole console, IPlatformEnvironmentManager environmentManager)
        {
            _console = console;
            _environmentManager = environmentManager;
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
            
            var storage = new XCFileSystemStorage(pathTo, Name);
            var configuration = Project.Load(storage);

            var env = _environmentManager.GetEnvironment(Name);
            if (env is IPlatformEnvironment platform)
            {
                var databaseStorage = new XCDatabaseStorage(DatabaseConstantNames.SAVE_CONFIG_TABLE_NAME,
                platform.DataContextManager.GetContext(), platform.DataContextManager.SqlCompiler);
                configuration.Save(databaseStorage);
            }
            
        }
    }
}
