using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using YamlDotNet.Serialization;

namespace Aquila.CodeAnalysis.CommandLine
{
    /// <summary>
    /// Implementation of <c>aqc.exe</c>.
    /// </summary>
    internal class AquilaCompiler : CommonCompiler
    {
        readonly DiagnosticFormatter _diagnosticFormatter;
        readonly string _tempDirectory;

        protected internal new AquilaCommandLineArguments Arguments
        {
            get { return (AquilaCommandLineArguments)base.Arguments; }
        }

        public AquilaCompiler(CommandLineParser parser, string responseFile, string[] args, BuildPaths buildPaths,
            string additionalReferenceDirectories, IAnalyzerAssemblyLoader analyzerLoader)
            : base(parser, responseFile, args, buildPaths, additionalReferenceDirectories, analyzerLoader,
                new GeneratorDriverCache())
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
            /// </summary>
            public AquilaSyntaxTree SyntaxTree;

            /// <summary>
            /// Syntax trees for compilation
            /// </summary>
            public IEnumerable<AquilaSyntaxTree> Trees;

            /// <summary>Additional resources.</summary>
            public ResourceDescription Resources;
        }


        public override Compilation CreateCompilation(TextWriter consoleOutput, TouchedFileLogger touchedFilesLogger,
            ErrorLogger errorLogger, ImmutableArray<AnalyzerConfigOptionsResult> analyzerConfigOptions,
            AnalyzerConfigOptionsResult globalConfigOptions)
        {
            bool hadErrors = false;
            var sourceFiles = Arguments.SourceFiles;
            var metadataFiles = Arguments.MetadataFiles;

            IEnumerable<AquilaSyntaxTree> sourceTrees;

            IEnumerable<EntityMetadata> metadata;

            var resources = Enumerable.Empty<ResourceDescription>();

            using (Arguments.CompilationOptions.EventSources.StartMetric("parse"))
            {
                // PARSE

                var parseOptions = Arguments.ParseOptions;

                // NOTE: order of trees is important!!
                var trees = new AquilaSyntaxTree[sourceFiles.Length];

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

                sourceTrees = trees;
                // END PARSE
            }

            using (Arguments.CompilationOptions.EventSources.StartMetric("parse-md"))
            {
                var mdList = new EntityMetadata[metadataFiles.Length];

                var d = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .Build();


                if (Arguments.CompilationOptions.ConcurrentBuild)
                {
                    Parallel.For(0, metadataFiles.Length,
                        i =>
                        {
                            mdList[i] = ParseMetadataFile(metadataFiles[i], consoleOutput, ref hadErrors, errorLogger,
                                d);
                        });
                }
                else
                {
                    for (int i = 0; i < sourceFiles.Length; i++)
                    {
                        mdList[i] = ParseMetadataFile(metadataFiles[i], consoleOutput, ref hadErrors, errorLogger, d);
                    }
                }


                metadata = mdList;
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
            if (ReportDiagnostics(diagnostics, consoleOutput, errorLogger, null))
            {
                return null;
            }

            //
            var referenceResolver = GetCommandLineMetadataReferenceResolver(touchedFilesLogger);
            var loggingFileSystem = new LoggingStrongNameFileSystem(touchedFilesLogger, _tempDirectory);
            var strongNameProvider = Arguments.GetStrongNameProvider(loggingFileSystem);

            var compilation = AquilaCompilation.Create(
                Arguments.CompilationName,
                sourceTrees.WhereNotNull(),
                metadata,
                resolvedReferences,
                resources: resources,
                options: Arguments.CompilationOptions.WithMetadataReferenceResolver(referenceResolver)
                    .WithAssemblyIdentityComparer(assemblyIdentityComparer).WithStrongNameProvider(strongNameProvider)
                    .WithXmlReferenceResolver(xmlFileResolver).WithSourceReferenceResolver(sourceFileResolver)
            );

            return compilation;
        }

        private EntityMetadata ParseMetadataFile(string filePath, TextWriter consoleOutput, ref bool hadErrors,
            ErrorLogger errorLogger, IDeserializer d)
        {
            var diagnosticInfos = new List<DiagnosticInfo>();

            try
            {
                using TextReader tr = new StreamReader(filePath);
                return d.Deserialize<EntityMetadata>(tr);
            }
            catch (Exception ex)
            {
                diagnosticInfos.Add(ToFileReadDiagnostics(MessageProvider, ex, filePath));
            }

            if (diagnosticInfos.Count != 0)
            {
                ReportDiagnostics(diagnosticInfos, consoleOutput, errorLogger, null);
                hadErrors = true;
            }

            return null;
        }

        private ParsedSource ParseFile(
            TextWriter consoleOutput,
            AquilaParseOptions parseOptions,
            AquilaParseOptions scriptParseOptions,
            ref bool hadErrors,
            CommandLineSourceFile file,
            ErrorLogger errorLogger)
        {
            // single source file

            var diagnosticInfos = new List<DiagnosticInfo>();
            var content = TryReadFileContent(file, diagnosticInfos, out var normalizedFilePath);

            if (diagnosticInfos.Count != 0)
            {
                ReportDiagnostics(diagnosticInfos, consoleOutput, errorLogger, null);
                hadErrors = true;
            }

            SyntaxTree result = null;

            if (content != null)
            {
                result = AquilaSyntaxTree.ParseText(content, parseOptions, normalizedFilePath);
            }

            if (result != null)
            {
                var diag = result.GetDiagnostics().ToImmutableArray();

                if (diag.Any())
                {
                    ReportDiagnostics(diag, consoleOutput, errorLogger, null);
                    hadErrors = true;
                }

                return new ParsedSource { SyntaxTree = (AquilaSyntaxTree)result };
            }

            throw new Exception("Compilation error. IDK.");
        }

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
            // foreach (var version in AquilaSyntaxTree.SupportedLanguageVersions)
            // {
            //     consoleOutput.WriteLine(version.ToString(2));
            // }

            consoleOutput.WriteLine();
        }

        internal static string GetVersion() => typeof(AquilaCompiler).GetTypeInfo().Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        internal new string GetAssemblyFileVersion() => GetVersion();


        protected override void ResolveAnalyzersFromArguments(List<DiagnosticInfo> diagnostics,
            CommonMessageProvider messageProvider, bool skipAnalyzers, out ImmutableArray<DiagnosticAnalyzer> analyzers,
            out ImmutableArray<ISourceGenerator> generators)
        {
            Arguments.ResolveAnalyzersFromArguments(Constants.AquilaLanguageName, diagnostics, messageProvider,
                AssemblyLoader, skipAnalyzers, out analyzers, out generators);
        }

        protected override bool TryGetCompilerDiagnosticCode(string diagnosticId, out uint code)
        {
            return TryGetCompilerDiagnosticCode(diagnosticId, "AQ", out code);
        }

        internal override string GetToolName()
        {
            return AquilaResources.IDS_ToolName;
        }

        internal override bool SuppressDefaultResponseFile(IEnumerable<string> args)
        {
            return args.Any(arg => new[] { "/noconfig", "-noconfig" }.Contains(arg.ToLowerInvariant()));
        }

        protected override void ResolveEmbeddedFilesFromExternalSourceDirectives(SyntaxTree tree,
            SourceReferenceResolver resolver, OrderedSet<string> embeddedFiles, DiagnosticBag diagnostics)
        {
            // We don't use any source mapping directives in Aq
        }

        private protected override GeneratorDriver CreateGeneratorDriver(ParseOptions parseOptions,
            ImmutableArray<ISourceGenerator> generators,
            AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, ImmutableArray<AdditionalText> additionalTexts)
        {
            throw new NotImplementedException();
        }

        protected override string GetOutputFileName(Compilation compilation, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}