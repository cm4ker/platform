using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Peachpie.Library.Scripting;

namespace Aquila.Compiler.Tests
{
    public abstract class TestBaseCLR : IDisposable
    {
        private ZLanguageVisitor _zlv;

        public TestBaseCLR()
        {
            _zlv = new ZLanguageVisitor();
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
            var parser = ParserHelper.Parse(unit);
            var point = (SourceUnit) _zlv.VisitEntryPoint(parser.entryPoint());
            var tree = PhpSyntaxTree.ParseCode(SourceText.From(unit, Encoding.UTF8), PhpParseOptions.Default, PhpParseOptions.Default,
                "");

            var diagnostics = tree.Diagnostics;

            if (diagnostics.IsEmpty)
            {
                PhpCompilationFactory builder = new PhpCompilationFactory();
                var compilation = (PhpCompilation) builder.CoreCompilation
                    .WithAssemblyName("Test")
                    .AddSyntaxTrees(tree);

                compilation = compilation.WithPhpOptions(compilation.Options.WithConcurrentBuild(false));
                
                var emitOptions = new EmitOptions();
                
                var embeddedTexts = default(IEnumerable<EmbeddedText>);

                if (true)
                {
                    compilation = compilation.WithPhpOptions(compilation.Options
                        .WithOptimizationLevel(OptimizationLevel.Debug).WithDebugPlusMode(true));

                    // if (options.IsSubmission)
                    // {
                    //     emitOptions = emitOptions.WithDebugInformationFormat(DebugInformationFormat.PortablePdb);
                    //     embeddedTexts = new[] { EmbeddedText.FromSource(tree.FilePath, tree.GetText()) };
                    // }
                }
                else
                {
                    compilation =
                        compilation.WithPhpOptions(
                            compilation.Options.WithOptimizationLevel(OptimizationLevel.Release));
                }

                diagnostics = compilation.GetDeclarationDiagnostics();

                if (!diagnostics.IsEmpty)
                {
                    throw new Exception("compiler error");
                }
                else
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
                        }
                    }
                    else
                    {
                        diagnostics = result.Diagnostics;
                    }
                }
            }
        }

        public object CompileAndRun(string unit)
        {
            Compile(unit);
            return null;
        }

        public void Dispose()
        {
        }
    }
}