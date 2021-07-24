using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Errors;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis.Passes;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Syntax.Parser;
using SyntaxToken = Microsoft.CodeAnalysis.SyntaxToken;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Aquila.CodeAnalysis.Syntax
{
    /// <summary>
    /// Adapter providing <see cref="SyntaxTree"/> from <see cref="SourceUnit"/> and storing parse diagnostics.
    /// </summary>
    public class AquilaSyntaxTree : SyntaxTree
    {
        readonly SourceUnit _source;

        private string _filePath;

        /// <summary>
        /// Gets constructed function declaration nodes.
        /// </summary>
        public ImmutableArray<MethodDecl> Functions { get; private set; }

        /// <summary>
        /// Gest declared component in tree
        /// </summary>
        public ImmutableArray<ComponentDecl> Components { get; private set; }

        /// <summary>
        /// Gets constructed yield extpressions.
        /// </summary>
        public ImmutableArray<LangElement> YieldNodes { get; private set; }

        /// <summary>
        /// Gets file path for the debug document and embedded text feature.
        /// </summary>
        public string GetDebugSourceDocumentPath() => FilePath;

        /// <summary>
        /// Map of supported language versions and corresponding <see cref="LanguageFeatures"/> understood by underlying parser.
        /// </summary>
        static readonly Dictionary<Version, LanguageFeatures> s_langversions =
            new Dictionary<Version, LanguageFeatures>()
            {
                { new Version(1, 0), LanguageFeatures.Basic },
            };

        public static Version LatestLanguageVersion => new Version(1, 0);

        public static Version DefaultLanguageVersion => LatestLanguageVersion;

        public static IReadOnlyCollection<Version> SupportedLanguageVersions => s_langversions.Keys;

        private AquilaSyntaxTree(SourceUnit source)
        {
            _source = source ?? throw ExceptionUtilities.ArgumentNull(nameof(source));
            _filePath = source.FilePath;
        }

        internal override bool SupportsLocations => true;

        internal static LanguageFeatures ParseLanguageVersion(ref Version languageVersion)
        {
            languageVersion ??= DefaultLanguageVersion;

            if (s_langversions.TryGetValue(languageVersion, out var features))
            {
                return features;
            }
            else
            {
                throw Roslyn.Utilities.ExceptionUtilities.UnexpectedValue(languageVersion);
            }
        }

        static LanguageFeatures GetLanguageFeatures(AquilaParseOptions options)
        {
            var version = options.LanguageVersion;
            var features = ParseLanguageVersion(ref version);

            //
            return features;
        }

        public static AquilaSyntaxTree ParseCode(
            SourceText sourceText,
            AquilaParseOptions parseOptions,
            AquilaParseOptions scriptParseOptions,
            string fname)
        {
            if (fname == null)
            {
                throw ExceptionUtilities.ArgumentNull(nameof(fname));
            }
            // TODO: new parser implementation based on Roslyn

            SourceUnit unit = ParserHelper.ParseUnit(sourceText.ToString(), fname);
            return Create(unit, fname);
        }

        public static AquilaSyntaxTree Create(
            SourceUnit unit,
            string fname)
        {
            if (fname == null)
            {
                throw ExceptionUtilities.ArgumentNull(nameof(fname));
            }

            // TODO: new parser implementation based on Roslyn

            var result = (AquilaSyntaxTree)new AquilaSyntaxTree(unit).WithFilePath(fname);

            unit.UpdateBelongSyntaxTree(result);

            var errorSink = new ErrorSink(result);
            result.Diagnostics = errorSink.Diagnostics;
            result.Functions = unit.Methods.Elements;
            result.Components = unit.Components.Elements;

            return result;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

        public override Encoding Encoding => Encoding.UTF8;

        public override string FilePath => _filePath;

        public override bool HasCompilationUnitRoot => true;

        public override int Length => 0; //_source.SourceText.Length;

        protected override ParseOptions OptionsCore
        {
            get { throw new NotImplementedException(); }
        }

        internal SourceUnit Source => _source;

        public override IList<TextSpan> GetChangedSpans(SyntaxTree syntaxTree)
        {
            throw new NotImplementedException();
        }

        public override IList<TextChange> GetChanges(SyntaxTree oldTree)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxTrivia trivia)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxNodeOrToken nodeOrToken)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Diagnostic> GetDiagnostics(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Diagnostics;
        }

        public override FileLinePositionSpan GetLineSpan(TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return new FileLinePositionSpan(_source.FilePath, new LinePosition(0, 0), new LinePosition(0, 0));
            //_source.LinePosition(span.Start), _source.LinePosition(span.End));
        }

        public override Location GetLocation(TextSpan span)
        {
            return new SourceLocation(this, span);
        }

        public override FileLinePositionSpan GetMappedLineSpan(TextSpan span,
            CancellationToken cancellationToken = default)
        {
            return GetLineSpan(span, cancellationToken);
        }

        public override SyntaxReference GetReference(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return SourceText.From("", Encoding.UTF8); //_source.SourceText;
        }

        public override bool HasHiddenRegions()
        {
            throw new NotImplementedException();
        }

        public override bool IsEquivalentTo(SyntaxTree tree, bool topLevel = false)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetText(out SourceText text)
        {
            throw new NotImplementedException();
            //text = _source.SourceText;
            return text != null;
        }

        public override SyntaxTree WithChangedText(SourceText newText)
        {
            throw new NotImplementedException();
        }

        public override SyntaxTree WithFilePath(string path)
        {
            _filePath = path;
            return this;
        }

        public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<SyntaxNode> GetRootAsyncCore(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode GetRootCore(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetRootCore(out SyntaxNode root)
        {
            throw new NotImplementedException();
        }
    }
}