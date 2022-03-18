using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Cli.Commands.Db;

namespace Aquila.Cli.Commands
{
    [HelpOption]
    [Command("db")]
    [Subcommand(typeof(CommandDbCreate))]
    [Subcommand(typeof(CommandDbDeploy))]
    [Subcommand(typeof(CommandDBList))]
    public class CommandDb
    {
        public int OnExecute()
        {
            return 0;
        }
    }

    [HelpOption]
    [Command("deploy")]
    public class CommandDeploy
    {
        public int OnExecute()
        {
            Console.WriteLine("Deploy!!!");
            return 0;
        }
    }
}