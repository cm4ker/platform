using Aquila.Core.Instance;
using McMaster.Extensions.CommandLineUtils;
using MoreLinq.Extensions;

namespace Aquila.Cli.Commands.Db
{
    [Command("list")]
    public class CommandDBList
    {
        private IConsole _console;
        private AqInstanceManager _instanceManager;

        public CommandDBList(IConsole console, AqInstanceManager instanceManager)
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