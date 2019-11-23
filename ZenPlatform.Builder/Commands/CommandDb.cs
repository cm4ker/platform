﻿using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Cli.Commands.Db;

namespace ZenPlatform.Cli.Commands
{
    [Command("db")]
    [HelpOption]
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
}
