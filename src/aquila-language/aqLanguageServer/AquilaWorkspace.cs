using System.IO;
using System.Threading;
using Aquila.CodeAnalysis.CommandLine;
using OmniSharp.Host.Services;

namespace Aquila.LanguageServer
{
    public class AquilaWorkspace
    {
        public void Update()
        {
            var compilation = AquilaCompilerDriver.Compile(AquilaCommandLineParser.Default, null, new string[] { }, "", "", "", "",
                new AnalyzerAssemblyLoader(), TextWriter.Null, CancellationToken.None);
            
            compilation.GetSemanticModel()
        }
    }
}