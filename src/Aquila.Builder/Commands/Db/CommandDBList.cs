using McMaster.Extensions.CommandLineUtils;
using Aquila.Core.Contracts.Environment;

namespace Aquila.Cli.Commands.Db
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
