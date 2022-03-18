using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.CommandLine
{
    public static class AquilaCompilerDriver
    {
        public static int Run(CommandLineParser parser, string responseFile, string[] args,
            string clientDirectory, string baseDirectory, string sdkDirectory,
            string additionalReferenceDirectories, IAnalyzerAssemblyLoader analyzerLoader,
            TextWriter output, CancellationToken cancellationToken)
        {
            var buildPaths = new BuildPaths(clientDirectory, baseDirectory, sdkDirectory, null);
            return
                new AquilaCompiler(parser, responseFile, args, buildPaths, additionalReferenceDirectories,
                        analyzerLoader)
                    .Run(output, cancellationToken);
        }

        public static AquilaCompilation Compile(CommandLineParser parser, string responseFile, string[] args,
            string clientDirectory, string baseDirectory, string sdkDirectory,
            string additionalReferenceDirectories, IAnalyzerAssemblyLoader analyzerLoader,
            TextWriter output, CancellationToken cancellationToken)
        {
            var buildPaths = new BuildPaths(clientDirectory, baseDirectory, sdkDirectory, null);
            return
                (AquilaCompilation)new AquilaCompiler(parser, responseFile, args, buildPaths,
                        additionalReferenceDirectories, analyzerLoader)
                    .CreateCompilation(output, null, null, new ImmutableArray<AnalyzerConfigOptionsResult>(),
                        new AnalyzerConfigOptionsResult());
        }

        public static int Run(AquilaCommandLineParser @default)
        {
            throw new NotImplementedException();
        }
    }
}