using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Core;
using Aquila.Library.Scripting;
using Aquila.Metadata;
using Aquila.Test.Tools;
using ICSharpCode.Decompiler.Metadata;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;
using Location = Aquila.Core.Location;

namespace ScriptsTest
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // this class never created and provide database context for script tests
    }

    [Collection("Database collection")]
    public class ScriptsTest
    {
        /// <summary>
        /// If the test returns this string, skip it.
        /// </summary>
        const string SkippedTestReturn = "***SKIP***";

        static readonly AqContext.IScriptingProvider _provider = new ScriptingProvider();

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
            var hasIL = TryGetILExpect(path, out var expectedIL);
            Assert.True(File.Exists(expectedFile), "Please create expect file for comparing results");

            var expected = File.ReadAllText(expectedFile);

            // test script compilation and run it
            var (result, il) = CompileAndRun(path, _fixture.Context);

            Assert.Equal(expected, result);
            
            if (hasIL)
                Assert.Equal(expectedIL, il);


            // Skip if platform wants it to
            Skip.If(result == SkippedTestReturn);
        }

        private static bool TryGetILExpect(string path, out string ilContent)
        {
            var expectILFile = Path.Combine(Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + ".il");

            if (File.Exists(expectILFile))
            {
                ilContent = File.ReadAllText(expectILFile);
                return true;
            }

            ilContent = null;
            return false;
        }

        (string Result, string ILResult ) CompileAndRun(string path, AqContext ctx)
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
            }, File.ReadAllText(path), TestMetadata.GetTestMetadata());

            // run

            script.Evaluate(ctx);

            //
            outputStream.Position = 0;
            return (new StreamReader(outputStream, Encoding.UTF8).ReadToEnd(), GetIL(script.Image));
        }

        static string GetIL(ImmutableArray<byte> immutableArray)
        {
            var output = new ICSharpCode.Decompiler.PlainTextOutput();
            using (var moduleMetadata = ModuleMetadata.CreateFromImage(immutableArray))
            {
                var peFile = new PEFile("<test_file>", reader: new PEReader(immutableArray));
                var metadataReader = moduleMetadata.GetMetadataReader();

                bool found = false;
                foreach (var typeDefHandle in metadataReader.TypeDefinitions)
                {
                    var typeDef = metadataReader.GetTypeDefinition(typeDefHandle);
                    if (metadataReader.GetString(typeDef.Name) == WellKnownAquilaNames.MainModuleName)
                    {
                        var disassembler =
                            new ICSharpCode.Decompiler.Disassembler.ReflectionDisassembler(output, default);
                        disassembler.DisassembleType(peFile, typeDefHandle);
                        found = true;
                        break;
                    }
                }

                Assert.True(found, "Could not find type named " + WellKnownAquilaNames.MainModuleName);
            }

            return output.ToString();
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