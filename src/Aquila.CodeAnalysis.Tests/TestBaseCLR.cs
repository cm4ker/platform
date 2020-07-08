using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Metadata;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.Compiler.Tests
{
    public abstract class CompilerTestBase : IDisposable
    {
        private ZLanguageVisitor _zlv;

        public CompilerTestBase()
        {
            _zlv = new ZLanguageVisitor("unknown.cs");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        object ExecuteAndUnload(string assemblyPath, string methodName, out WeakReference alcWeakRef)
        {
            var alc = new HostAssemblyLoadContext(assemblyPath);
            alcWeakRef = new WeakReference(alc);

            Assembly a = alc.LoadFromAssemblyPath(assemblyPath);

            var cName = "Test";

            Type execClass = a.GetType($"{cName}");

            MethodInfo method = execClass.GetMethod(methodName);

            object result = method.Invoke(null, null);

            alc.Unload();

            return result;
        }


        public void Clear()
        {
        }

        // public void ImportRef()
        // {
        //     var asm = Ap.CreateAssembly("Debug", Version.Parse("1.0.0.0"));
        //     var asmName = $"test.bll";
        //
        //     if (File.Exists(asmName))
        //         File.Delete(asmName);
        //
        //     asm.Write(asmName);
        // }

        static MetadataReference CreateMetadataReference(string path) => MetadataReference.CreateFromFile(path);

        public void Compile(string unit)
        {
            var tree = AquilaSyntaxTree.ParseCode(SourceText.From(unit, Encoding.UTF8), AquilaParseOptions.Default,
                AquilaParseOptions.Default,
                "test_aq.cs");

            var diagnostics = tree.Diagnostics;

            if (diagnostics.IsEmpty)
            {
                AquilaCompilationFactory builder = new AquilaCompilationFactory();
                var compilation = (AquilaCompilation)builder.CoreCompilation
                        // .WithAssemblyName("Test")
                        .AddSyntaxTrees(tree)
                        .AddMetadata(TestMetadata.GetTestMetadata().Metadata)
                    ;

                compilation = compilation.WithAquilaOptions(compilation.Options.WithConcurrentBuild(false));

                var emitOptions = new EmitOptions();

                var embeddedTexts = default(IEnumerable<EmbeddedText>);

                if (true)
                {
                    compilation = compilation.WithAquilaOptions(compilation.Options
                        .WithOptimizationLevel(OptimizationLevel.Debug).WithDebugPlusMode(true));
                }
                else
                {
                    compilation =
                        compilation.WithAquilaOptions(
                            compilation.Options.WithOptimizationLevel(OptimizationLevel.Release));
                }

                diagnostics = compilation.GetDeclarationDiagnostics();

                if (!DiagnosticBagExtensions.HasErrors(diagnostics))
                {
                    var peStream = new MemoryStream();
                    var pdbStream = true ? new MemoryStream() : null;

                    var result = compilation.Emit(peStream,
                        pdbStream: pdbStream,
                        options: emitOptions,
                        embeddedTexts: embeddedTexts
                    );

                    if (result.Success)
                    {
                        if (pdbStream != null)
                        {
                            // DEBUG DUMP
                            var fname = @"C:\test\" +
                                        Path.GetFileNameWithoutExtension(tree.FilePath);
                            File.WriteAllBytes(fname + ".dll", peStream.ToArray());
                            File.WriteAllBytes(fname + ".pdb", pdbStream.ToArray());
                            return;
                        }
                    }
                    else
                    {
                        diagnostics = result.Diagnostics;
                    }
                }

                CreateInvalid(diagnostics);
            }
        }

        /// <summary>
        /// Initializes an invalid script that throws diagnostics upon invoking.
        /// </summary>
        private static void CreateInvalid(ImmutableArray<Diagnostic> diagnostics)
        {
            var errors = string.Join(Environment.NewLine,
                diagnostics.Select(d => $"{d.Severity} {d.Id}: {d.GetMessage()}"));

            throw new Exception($"The script cannot be compiled due to following errors:\n{errors}");
        }

        public object CompileAndRun(string unit)
        {
            Compile(unit);
            var asm = Assembly.LoadFile("C:\\Test\\test_aq.dll");
            var result = asm.GetType("<Constants>").GetMethod("Main").Invoke(null, null);
            return result;
        }

        public void Dispose()
        {
        }
    }
}