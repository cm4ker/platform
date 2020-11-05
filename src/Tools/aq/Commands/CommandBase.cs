// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Aquila.Tools.CommandLineUtils;
using JetBrains.Annotations;

namespace Aquila.Tools.Commands
{
    internal abstract class CommandBase
    {
        public virtual void Configure(CommandLineApplication command)
        {
            var verbose = command.Option("-v|--verbose", "Resources.VerboseDescription");
            var noColor = command.Option("--no-color", "Resources.NoColorDescription");
            var prefixOutput = command.Option("--prefix-output", "Resources.PrefixDescription");

            command.HandleResponseFiles = true;

            command.OnExecute(
                (args) =>
                {
                    Reporter.IsVerbose = verbose.HasValue();
                    Reporter.NoColor = noColor.HasValue();
                    Reporter.PrefixOutput = prefixOutput.HasValue();

                    Validate();

                    return Execute(args);
                });
        }

        protected virtual void Validate()
        {
        }

        protected virtual int Execute([NotNull] string[] args)
            => 0;
    }
}
