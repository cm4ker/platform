using McMaster.Extensions.CommandLineUtils;
using Aquila.Core.Contracts.Instance;
using MoreLinq.Extensions;

namespace Aquila.Cli.Commands.Db
{
    [Command("list")]
    public class CommandDBList
    {
        private IConsole _console;
        private IPlatformInstanceManager _instanceManager;
        public CommandDBList(IConsole console, IPlatformInstanceManager instanceManager)
        {
            _console = console;
            _instanceManager = instanceManager;
        }
        public void OnExecute()
        {
            _instanceManager.GetInstances().ForEach(e => _console.WriteLine(e.Name));
        }
    }
}
