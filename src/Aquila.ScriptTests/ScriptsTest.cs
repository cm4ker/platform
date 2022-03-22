using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.Instance;
using Aquila.Core.Sessions;
using Aquila.Core.Test;
using Aquila.Library.Scripting;
using Aquila.Metadata;
using Aquila.Runtime.Tests.DB;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ScriptsTest
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("Database collection")]
    public class ScriptsTest
    {
        /// <summary>
        /// If the test returns this string, skip it.
        /// </summary>
        const string SkippedTestReturn = "***SKIP***";

        static readonly AqContext.IScriptingProvider
            _provider = new ScriptingProvider(); // use IScriptingProvider singleton

        private readonly ITestOutputHelper _output;
        private readonly DatabaseFixture _fixture;

        public ScriptsTest(ITestOutputHelper output, DatabaseFixture fixture)
        {
            _output = output;
            _fixture = fixture;
        }

        [SkippableTheory]
        [ScriptsListData]
        public void ScriptRunTest(string fname, string subdir, string testsdir)
        {
            var path = Path.Combine(testsdir, subdir, fname);
            var isSkipTest =
                new Regex(@"^skip(\([^)]*\))?_.*$"); // matches either skip_<smth>.aq or skip(<reason>)_<smth>.aq
            Skip.If(isSkipTest.IsMatch(fname));

            _output.WriteLine("Testing {0} ...", fname);

            var expectedFile = Path.Combine(Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + ".expect");

            Assert.True(File.Exists(expectedFile), "Please create expect file for comparing results");

            var expected = File.ReadAllText(expectedFile);

            // test script compilation and run it
            var result = CompileAndRun(path, _fixture.Context);

            Assert.Equal(expected, result);

            // Skip if platform wants it to
            Skip.If(result == SkippedTestReturn);
        }

        string CompileAndRun(string path, AqContext ctx)
        {
            var outputStream = new MemoryStream();

            ctx.Output = outputStream;

            // Compile and load 
            var script = _provider.CreateScript(new AqContext.ScriptOptions()
            {
                Context = ctx,
                IsSubmission = false,
                EmitDebugInformation = true,
                Location = new Location(path, 0, 0),
                //AdditionalReferences = AdditionalReferences,
            }, File.ReadAllText(path), TestMetadata.GetTestMetadata());

            // run

            script.Evaluate(ctx);

            //
            outputStream.Position = 0;
            return new StreamReader(outputStream, Encoding.UTF8).ReadToEnd();
        }

        static string RunProcess(string exe, string args, string cwd)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(exe, args)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
                WorkingDirectory = cwd
            };

            //
            process.Start();

            // To avoid deadlocks, always read the output stream first and then wait.
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            //
            if (!string.IsNullOrEmpty(error))
                return error;

            //
            return output;
        }
    }
}