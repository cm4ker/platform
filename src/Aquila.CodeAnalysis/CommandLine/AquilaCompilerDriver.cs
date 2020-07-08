using System;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.CommandLine
{
    public static class AquilaCompilerDriver
    {
        public static int Run(CommandLineParser parser, string responseFile, string[] args,
            string clientDirectory, string baseDirectory, string sdkDirectory,
            string additionalReferenceDirectories,
            IAnalyzerAssemblyLoader analyzerLoader,
            TextWriter output, CancellationToken cancellationToken)
        {
            var buildPaths = new BuildPaths(clientDirectory, baseDirectory, sdkDirectory, null);
            return
                new AquilaCompiler(parser, responseFile, args, buildPaths, additionalReferenceDirectories,
                        analyzerLoader)
                    .Run(output, cancellationToken);
        }

        public static int Run(AquilaCommandLineParser @default)
        {
            throw new NotImplementedException();
        }
    }
}