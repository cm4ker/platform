using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZenPlatform.Shell.Contracts;

namespace ZenPlatform.Cli.Commands
{
    [Command("exit")]
    [HelpOption]
    public class CommandExit
    {
        private IConsole _console;
        private ITerminalSession _terminalSession;
        public CommandExit(IConsole console, ITerminalSession terminalSession)
        {
            _console = console;
            _terminalSession = terminalSession;
        }
        public void OnExecute()
        {
            _console.WriteLine("Bye!");
            
            _terminalSession.Close();
        }
    }
}
