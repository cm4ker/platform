using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Environment;

namespace ZenPlatform.Cli.Commands.Db
{
    [Command("list")]
    public class CommandDBList
    {
        private IConsole _console;
        private IPlatformEnvironmentManager _environmentManager;
        public CommandDBList(IConsole console, IPlatformEnvironmentManager environmentManager)
        {
            _console = console;
            _environmentManager = environmentManager;
        }
        public void OnExecute()
        {
            _environmentManager.GetEnvironmentList().ForEach(e => _console.WriteLine(e.Name));
        }
    }
}
