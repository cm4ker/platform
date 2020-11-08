﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.CommandLine
{
    /// <summary>
    /// Implementation of <c>pchp.exe</c>.
    /// </summary>
    internal class AquilaCompiler : CommonCompiler
    {
        internal const string ResponseFileName = "php.rsp";

        readonly DiagnosticFormatter _diagnosticFormatter;
        readonly string _tempDirectory;

        protected internal new AquilaCommandLineArguments Arguments
        {
            get { return (AquilaCommandLineArguments) base.Arguments; }
        }

        public AquilaCompiler(CommandLineParser parser, string responseFile, string[] args, BuildPaths buildPaths,
            string additionalReferenceDirectories, IAnalyzerAssemblyLoader analyzerLoader)
            : base(parser, responseFile, args, buildPaths, additionalReferenceDirectories, analyzerLoader)
        {
            _tempDirectory = buildPaths.TempDirectory;
            _diagnosticFormatter =
                new CommandLineDiagnosticFormatter(buildPaths.WorkingDirectory, Arguments.PrintFullPaths);
        }

        public override DiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

        internal override Type Type => typeof(AquilaCompiler);

        /// <summary>
        /// Result of the <see cref="ParseFile"/> operation.
        /// </summary>
        struct ParsedSource
        {
            /// <summary>
            /// Resulting syntax tree of source file
            /// or a syntax tree of PHAR file stub.
            /// </summary>
            public AquilaSyntaxTree SyntaxTree;

            /// <summary>PHAR manifest in case the source file is a PHAR file.</summary>
            //public Devsense.PHP.Phar.Manifest Manifest;

            /// <summary>PHAR syntax trees in case the source file is a PHAR file.</summary>
            public IEnumerable<AquilaSyntaxTree> Trees;

            /// <summary>Additional resources.</summary>
            public ResourceDescription Resources;
        }

        public override Compilation CreateCompilation(TextWriter consoleOutput, TouchedFileLogger touchedFilesLogger,
            ErrorLogger errorLogger)
        {
            bool hadErrors = false;
            var sourceFiles = Arguments.SourceFiles;

            IEnumerable<AquilaSyntaxTree> sourceTrees;
            var resources = Enumerable.Empty<ResourceDescription>();

            using (Arguments.CompilationOptions.EventSources.StartMetric("parse"))
            {
                // PARSE

                var parseOptions = Arguments.ParseOptions;

                // NOTE: order of trees is important!!
                var trees = new AquilaSyntaxTree[sourceFiles.Length];
                var pharFiles = new List<(int index, ParsedSource phar)>();

                void ProcessParsedSource(int index, ParsedSource parsed)
                {
                    trees[index] = parsed.SyntaxTree;
                }

                // We compute script parse options once so we don't have to do it repeatedly in
                // case there are many script files.
                var scriptParseOptions = parseOptions.WithKind(SourceCodeKind.Script);

                if (Arguments.CompilationOptions.ConcurrentBuild)
                {
                    Parallel.For(0, sourceFiles.Length,
                        new Action<int>(i =>
                        {
                            ProcessParsedSource(i,
                                ParseFile(consoleOutput, parseOptions, scriptParseOptions, ref hadErrors,
                                    sourceFiles[i], errorLogger));
                        }));
                }
                else
                {
                    for (int i = 0; i < sourceFiles.Length; i++)
                    {
                        ProcessParsedSource(i,
                            ParseFile(consoleOutput, parseOptions, scriptParseOptions, ref hadErrors, sourceFiles[i],
                                errorLogger));
                    }
                }

                // flattern trees and pharFiles

                if (pharFiles == null || pharFiles.Count == 0)
                {
                    sourceTrees = trees;
                }
                else
                {
                    var treesList = new List<AquilaSyntaxTree>(trees);

                    // enlist phars from the end (index)
                    foreach (var f in pharFiles.OrderByDescending(x => x.index))
                    {
                        treesList[f.index] = f.phar.SyntaxTree; // phar stub, may be null
                        treesList.InsertRange(f.index + 1, f.phar.Trees);

                        // add content files
                        if (f.phar.Resources != null)
                        {
                            resources = resources.Concat(f.phar.Resources);
                        }
                    }

                    sourceTrees = treesList;
                }

                // END PARSE
            }

            // If errors had been reported in ParseFile, while trying to read files, then we should simply exit.
            if (hadErrors)
            {
                return null;
            }

            var diagnostics = new List<DiagnosticInfo>();

            var assemblyIdentityComparer = DesktopAssemblyIdentityComparer.Default;

            var xmlFileResolver = new LoggingXmlFileResolver(Arguments.BaseDirectory, touchedFilesLogger);
            var sourceFileResolver = new LoggingSourceFileResolver(ImmutableArray<string>.Empty,
                Arguments.BaseDirectory, Arguments.PathMap, touchedFilesLogger);

            MetadataReferenceResolver referenceDirectiveResolver;
            var resolvedReferences =
                ResolveMetadataReferences(diagnostics, touchedFilesLogger, out referenceDirectiveResolver);
            if (ReportErrors(diagnostics, consoleOutput, errorLogger))
            {
                return null;
            }

            //
            var referenceResolver = GetCommandLineMetadataReferenceResolver(touchedFilesLogger);
            var loggingFileSystem = new LoggingStrongNameFileSystem(touchedFilesLogger);
            var strongNameProvider = Arguments.GetStrongNameProvider(loggingFileSystem, _tempDirectory);

            var compilation = AquilaCompilation.Create(
                Arguments.CompilationName,
                sourceTrees.WhereNotNull(),
                resolvedReferences,
                resources: resources,
                options: Arguments.CompilationOptions.WithMetadataReferenceResolver(referenceResolver)
                    .WithAssemblyIdentityComparer(assemblyIdentityComparer).WithStrongNameProvider(strongNameProvider)
                    .WithXmlReferenceResolver(xmlFileResolver).WithSourceReferenceResolver(sourceFileResolver)
            );

            return compilation;
        }

        private ParsedSource ParseFile(
            TextWriter consoleOutput,
            AquilaParseOptions parseOptions,
            AquilaParseOptions scriptParseOptions,
            ref bool hadErrors,
            CommandLineSourceFile file,
            ErrorLogger errorLogger)
        {
            if (file.Path.IsPharFile())
            {
                // phar file archive
                //var phar = Devsense.PHP.Phar.PharFile.OpenPharFile(file.Path); // TODO: report exception

                // treat the stub as a regular source code:
                var stub = AquilaSyntaxTree.ParseCode(SourceText.From($"", Encoding.UTF8), parseOptions,
                    scriptParseOptions, file.Path);

                // TODO: ConcurrentBuild -> Parallel

                var prefix =
                    PhpFileUtilities.NormalizeSlashes(
                        PhpFileUtilities.GetRelativePath(file.Path, Arguments.BaseDirectory));
                var trees = new List<AquilaSyntaxTree>();
                //var content = new List<Devsense.PHP.Phar.Entry>();

                throw new NotImplementedException();

                // foreach (var entry in phar.Manifest.Entries.Values)
                // {
                //     var entryName = PhpFileUtilities.NormalizeSlashes(entry.Name);
                //
                //     if (entry.IsIgnoredEntry())
                //     {
                //         continue;
                //     }
                //
                //     if (entry.IsCompileEntry())
                //     {
                //         var tree = AquilaSyntaxTree.ParseCode(SourceText.From(entry.Code, Encoding.UTF8), parseOptions, scriptParseOptions, $"{prefix}/{entryName}");
                //         tree.PharStubFile = stub;
                //         trees.Add(tree);
                //     }
                //     else
                //     {
                //         content.Add(entry);
                //     }
                // }

                // // create resource file
                // var resources = new ResourceDescription($"phar://{prefix}.resources", () =>
                // {
                //     var stream = new MemoryStream();
                //     var writer = new System.Resources.ResourceWriter(stream);
                //
                //     foreach (var entry in content)
                //     {
                //         var entryName = PhpFileUtilities.NormalizeSlashes(entry.Name);
                //         writer.AddResource(entryName, entry.Code);
                //     }
                //
                //     //
                //     writer.Generate();
                //     stream.Position = 0;
                //     return stream;
                // }, isPublic: true);
                //
                // // TODO: report errors if any
                //
                // return new ParsedSource { SyntaxTree = stub, Manifest = phar.Manifest, Trees = trees, Resources = resources, };
            }
            else
            {
                // single source file

                var diagnosticInfos = new List<DiagnosticInfo>();
                var content = TryReadFileContent(file, diagnosticInfos, out var normalizedFilePath);

                if (diagnosticInfos.Count != 0)
                {
                    ReportErrors(diagnosticInfos, consoleOutput, errorLogger);
                    hadErrors = true;
                }

                AquilaSyntaxTree result = null;

                if (content != null)
                {
                    result = AquilaSyntaxTree.ParseCode(content, parseOptions, scriptParseOptions, normalizedFilePath);
                }

                if (result != null && result.Diagnostics.HasAnyErrors())
                {
                    ReportErrors(result.Diagnostics, consoleOutput, errorLogger);
                    hadErrors = true;
                }

                return new ParsedSource {SyntaxTree = result};
            }
        }

        // static string GetPharStub(Devsense.PHP.Phar.PharFile phar)
        // {
        //     var stub = phar.StubCode;
        //
        //     if (string.IsNullOrEmpty(stub))
        //     {
        //         return string.Empty;
        //     }
        //
        //     // ignore first line starting with #
        //     if (stub[0] == '#')
        //     {
        //         // ignore this the first line
        //         for (int i = 1; i < stub.Length; i++)
        //         {
        //             var nl = TextUtils.LengthOfLineBreak(stub, i);
        //             if (nl != 0)
        //             {
        //                 stub = stub.Substring(i + nl);
        //                 break;
        //             }
        //         }
        //     }
        //
        //     // ignore __HALT_COMPILER and following
        //     var halt = stub.LastIndexOf("__HALT_COMPILER", StringComparison.OrdinalIgnoreCase);
        //     if (halt >= 0)
        //     {
        //         stub = stub.Remove(halt);
        //     }
        //
        //     //
        //     return stub;
        // }

        public override void PrintHelp(TextWriter consoleOutput)
        {
            consoleOutput.WriteLine(AquilaResources.IDS_Help);
        }

        public override void PrintLogo(TextWriter consoleOutput)
        {
            // {ToolName} version {ProductVersion}
            consoleOutput.WriteLine(AquilaResources.IDS_Logo, GetToolName(), GetAssemblyFileVersion());
        }

        public override void PrintLangVersions(TextWriter consoleOutput)
        {
            consoleOutput.WriteLine(AquilaResources.IDS_LangVersions);
            foreach (var version in AquilaSyntaxTree.SupportedLanguageVersions)
            {
                consoleOutput.WriteLine(version.ToString(2));
            }

            consoleOutput.WriteLine();
        }

        internal static string GetVersion() => typeof(AquilaCompiler).GetTypeInfo().Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        internal new string GetAssemblyFileVersion() => GetVersion();

        protected override ImmutableArray<DiagnosticAnalyzer> ResolveAnalyzersFromArguments(
            List<DiagnosticInfo> diagnostics, CommonMessageProvider messageProvider)
        {
            return Arguments.ResolveAnalyzersFromArguments(Constants.AquilaLanguageName, diagnostics, messageProvider,
                AssemblyLoader);
        }

        protected override bool TryGetCompilerDiagnosticCode(string diagnosticId, out uint code)
        {
            return TryGetCompilerDiagnosticCode(diagnosticId, "PHP", out code);
        }

        internal override string GetToolName()
        {
            return AquilaResources.IDS_ToolName;
        }

        internal override bool SuppressDefaultResponseFile(IEnumerable<string> args)
        {
            return args.Any(arg => new[] {"/noconfig", "-noconfig"}.Contains(arg.ToLowerInvariant()));
        }

        protected override void ResolveEmbeddedFilesFromExternalSourceDirectives(SyntaxTree tree,
            SourceReferenceResolver resolver, OrderedSet<string> embeddedFiles, DiagnosticBag diagnostics)
        {
            // We don't use any source mapping directives in PHP
        }
    }
}