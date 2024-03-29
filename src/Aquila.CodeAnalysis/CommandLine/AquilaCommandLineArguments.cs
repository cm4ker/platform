﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.CommandLine
{
    internal sealed class AquilaCommandLineArguments : CommandLineArguments
    {
        /// <summary>
        /// Gets the compilation options for the Aquila <see cref="Compilation"/>
        /// created from the <see cref="AquilaCompiler"/>.
        /// </summary>
        public new AquilaCompilationOptions CompilationOptions { get; internal set; }

        /// <summary>
        /// Gets the parse options for the Aquila <see cref="Compilation"/>.
        /// </summary>
        public new AquilaParseOptions ParseOptions { get; internal set; }

        protected override ParseOptions ParseOptionsCore => ParseOptions;

        protected override CompilationOptions CompilationOptionsCore => CompilationOptions;

        public ImmutableArray<string> MetadataFiles { get; set; }

        public ImmutableArray<CommandLineSourceFile> ViewFiles { get; set; }

        /// <value>
        /// Should the format of error messages include the line and column of
        /// the end of the offending text.
        /// </value>
        internal bool ShouldIncludeErrorEndLocation { get; set; }

        internal AquilaCommandLineArguments()
        {
        }
    }
}