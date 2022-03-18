using McMaster.Extensions.CommandLineUtils;

namespace Aquila.Cli.Commands
{
    [Command("exit")]
    [HelpOption]
    public class CommandExit
    {
        private IConsole _console;
        private object _terminalSession;
        
        public CommandExit(IConsole console, object terminalSession)
        {
            _console = console;
            _terminalSession = terminalSession;
        }
        public void OnExecute()
        {
            _console.WriteLine("Bye!");
            
            //_terminalSession.Close();
        }
    }
}
