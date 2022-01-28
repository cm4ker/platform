using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Concurrent;
using Aquila.CodeAnalysis;

namespace Peachpie.LanguageServer
{
    class ProjectHandler : IDisposable
    {
        public class DocumentDiagnosticsEventArgs : EventArgs
        {
            public string DocumentPath { get; }

            public IEnumerable<Diagnostic> Diagnostics { get; }

            public DocumentDiagnosticsEventArgs(string documentPath, IEnumerable<Diagnostic> diagnostics)
            {
                this.DocumentPath = documentPath;
                this.Diagnostics = diagnostics;
            }
        }

        readonly CompilationDiagnosticBroker _diagnosticBroker;

        readonly ConcurrentDictionary<string, ImmutableArray<Diagnostic>> _parserDiagnostics
            = new ConcurrentDictionary<string, ImmutableArray<Diagnostic>>(StringComparer.InvariantCultureIgnoreCase);

        private HashSet<string> _filesWithSemanticDiagnostics
            = new HashSet<string>();

        public AquilaCompilation Compilation => _diagnosticBroker.Compilation;

        public ProjectInstance BuildInstance { get; }

        public Encoding SourceEncoding { get; }

        public string RootPath => PathUtils.NormalizePath(BuildInstance.Directory);

        public event EventHandler<DocumentDiagnosticsEventArgs> DocumentDiagnosticsChanged;

        public ProjectHandler(AquilaCompilation compilation, ProjectInstance buildInstance, Encoding encoding)
        {
            BuildInstance = buildInstance;
            _diagnosticBroker = new CompilationDiagnosticBroker(HandleCompilationDiagnostics);
            _diagnosticBroker.UpdateCompilation(compilation);
            SourceEncoding = encoding ?? Encoding.UTF8;
        }

        public void Initialize()
        {
            // Initially populate _parserDiagnostics and send the corresponding diagnostics
            // (_parserDiagnostics will be updated by _diagnosticBroker)

            var diagnostics = Compilation.GetParseDiagnostics();
            foreach (var fileDiagnostics in diagnostics.GroupBy(diag => diag.Location.SourceTree))
            {
                var path = fileDiagnostics.Key.FilePath;
                _parserDiagnostics[path] = fileDiagnostics.ToImmutableArray();

                OnDocumentDiagnosticsChanged(path, fileDiagnostics);
            }
        }

        public void UpdateFile(string path, string text)
        {
            _parserDiagnostics.TryGetValue(path, out var previousDiagnostics);

            var syntaxTree =
                AquilaSyntaxTree.ParseText(SourceText.From(text, SourceEncoding), AquilaParseOptions.Default, path);

            var diags = syntaxTree.GetDiagnostics();

            if (diags.Any())
            {
                _parserDiagnostics[path] = diags.ToImmutableArray();
            }
            else if (previousDiagnostics != null)
            {
                _parserDiagnostics.TryRemove(path, out _);
            }

            if (diags.Any() || previousDiagnostics != null)
            {
                // If there were any errors previously, send an empty set to remove them
                OnDocumentDiagnosticsChanged(path, syntaxTree.GetDiagnostics());
            }

            // Update the compilation
            if (diags.All(d => d.Severity != DiagnosticSeverity.Error) &&
                _diagnosticBroker.Compilation != null)
            {
                var currentTree =
                    _diagnosticBroker.Compilation.SyntaxTrees.FirstOrDefault(tree => tree.FilePath == path);

                var updatedCompilation = currentTree == null
                    ? (AquilaCompilation)_diagnosticBroker.Compilation.AddSyntaxTrees(syntaxTree)
                    : (AquilaCompilation)_diagnosticBroker.Compilation.ReplaceSyntaxTree(currentTree, syntaxTree);

                _diagnosticBroker.UpdateCompilation(updatedCompilation);
            }
        }

        // internal IEnumerable<Protocol.Location> ObtainDefinition(string filepath, int line, int character)
        // {
        //     var compilation = _diagnosticBroker.LastAnalysedCompilation;
        //
        //     // We have to work with already fully analyzed and bound compilation that is up-to-date with the client's code
        //     if (compilation == null)
        //     {
        //         return Array.Empty<Protocol.Location>();
        //     }
        //
        //     // Find the symbols gathered from the given source code
        //     return ToolTipUtils.ObtainDefinition(compilation, filepath, line, character);
        // }
        //
        // public ToolTipInfo ObtainToolTip(string filepath, int line, int character)
        // {
        //     var compilation = _diagnosticBroker.LastAnalysedCompilation;
        //
        //     // We have to work with already fully analyzed and bound compilation that is up-to-date with the client's code
        //     if (compilation == null)
        //     {
        //         return null;
        //     }
        //
        //     // Find the symbols gathered from the given source code
        //     return ToolTipUtils.ObtainToolTip(compilation, filepath, line, character);
        // }

        bool IsHidden(Diagnostic d)
        {
            var options = Compilation.Options.SpecificDiagnosticOptions;

            return
                d.IsSuppressed ||
                d.Severity == DiagnosticSeverity.Hidden ||
                (options != null && options.TryGetValue(d.Id, out var report) &&
                 (report == ReportDiagnostic.Suppress || report == ReportDiagnostic.Hidden));
        }

        private void HandleCompilationDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            var errorFiles = new HashSet<string>();
            var options = Compilation.Options.SpecificDiagnosticOptions;

            var fileGroups = diagnostics
                .Where(d => !IsHidden(d))
                .GroupBy(diagnostic => diagnostic.Location.SourceTree);

            foreach (var fileDiagnostics in fileGroups)
            {
                var file = fileDiagnostics.Key.FilePath;

                errorFiles.Add(file);

                OnDocumentDiagnosticsChanged(
                    file,
                    _parserDiagnostics.TryGetValue(file, out var parsediag)
                        ? parsediag.Concat(fileDiagnostics)
                        : fileDiagnostics);
            }

            var cleared = _filesWithSemanticDiagnostics.Except(errorFiles);
            foreach (var file in cleared)
            {
                OnDocumentDiagnosticsChanged(
                    file,
                    _parserDiagnostics.TryGetValue(file, out var parsediag)
                        ? parsediag
                        : ImmutableArray<Diagnostic>.Empty);
            }

            _filesWithSemanticDiagnostics = errorFiles;
        }

        private void OnDocumentDiagnosticsChanged(string documentPath, IEnumerable<Diagnostic> diagnostics)
        {
            DocumentDiagnosticsChanged?.Invoke(this, new DocumentDiagnosticsEventArgs(documentPath, diagnostics));
        }

        /// <summary>
        /// Gets used PeachPie Sdk version.
        /// </summary>
        public bool TryGetSdkVersion(out string version)
        {
            version = this.BuildInstance.GetPropertyValue("PeachpieVersion");
            return version != null;
        }

        public void Dispose()
        {
            this.DocumentDiagnosticsChanged = null;
        }
    }
}