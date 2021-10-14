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
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ScriptsTest
{
    public class ScriptsTest
    {
        /// <summary>
        /// Environment variable, if set to "0" then we do not compare results with current `php` installed on the machine.
        /// </summary>
        const string AQUILA_TEST_AQ = "AQUILA_TEST_AQ";

        /// <summary>
        /// If the test returns this string, skip it.
        /// </summary>
        const string SkippedTestReturn = "***SKIP***";

        static readonly AqContext.IScriptingProvider
            _provider = new ScriptingProvider(); // use IScriptingProvider singleton

        private readonly ITestOutputHelper _output;

        public ScriptsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [SkippableTheory]
        [ScriptsListData]
        public void ScriptRunTest(string fname, string subdir, string testsdir)
        {
            var path = Path.Combine(testsdir, subdir, fname);
            var isSkipTest =
                new Regex(@"^skip(\([^)]*\))?_.*$"); // matches either skip_<smth>.aq or skip(<reason>)_<smth>.aq
            Skip.If(isSkipTest.IsMatch(fname));

            var service = TestEnvSetup.GetServerService(_output);
            var manager = service.GetService<IAqInstanceManager>();
            var instance = manager.GetInstance("Library");

            _output.WriteLine("Testing {0} ...", fname);

            // test script compilation and run it
            var result = CompileAndRun(path, new AqContext(instance));

            // Skip if platform wants it to
            Skip.If(result == SkippedTestReturn);
        }

        string CompileAndRun(string path, AqContext ctx)
        {
            var outputStream = new MemoryStream();
            var expectedFile = Path.Combine(Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + ".expect");

            Assert.True(File.Exists(expectedFile), "Please create expect file for comparing results");

            var expected = File.ReadAllText(expectedFile);

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

            var actual = script.Evaluate(ctx);
            Assert.Equal(expected, actual.ToString());


            //
            outputStream.Position = 0;
            return new StreamReader(outputStream, Encoding.UTF8).ReadToEnd();
        }

        // static string Interpret(string path)
        // {
        //     // Run PHP hiding any errors from the output (we don't compare them with ours)
        //     return RunProcess("aq", $"-d display_errors=Off -d log_errors=Off {Path.GetFileName(path)}",
        //         Path.GetDirectoryName(path));
        // }

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