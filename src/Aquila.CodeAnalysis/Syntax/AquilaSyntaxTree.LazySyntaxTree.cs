﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using Aquila.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis
{
    public partial class AquilaSyntaxTree
    {
        private sealed class LazySyntaxTree : AquilaSyntaxTree
        {
            private readonly SourceText _text;
            private readonly AquilaParseOptions _options;
            private readonly string _path;
            private readonly ImmutableDictionary<string, ReportDiagnostic> _diagnosticOptions;
            private AquilaSyntaxNode? _lazyRoot;

            internal LazySyntaxTree(
                SourceText text,
                AquilaParseOptions options,
                string path,
                ImmutableDictionary<string, ReportDiagnostic>? diagnosticOptions)
            {
                Debug.Assert(options != null);

                _text = text;
                _options = options;
                _path = path ?? string.Empty;
                _diagnosticOptions = diagnosticOptions ?? EmptyDiagnosticOptions;
            }

            public override bool IsView => _options.Kind == SourceCodeKind.View;

            public override string FilePath
            {
                get { return _path; }
            }

            public override SourceText GetText(CancellationToken cancellationToken)
            {
                return _text;
            }

            public override bool TryGetText([NotNullWhen(true)] out SourceText? text)
            {
                text = _text;
                return true;
            }

            public override Encoding? Encoding
            {
                get { return _text.Encoding; }
            }

            public override int Length
            {
                get { return _text.Length; }
            }

            public override AquilaSyntaxNode GetRoot(CancellationToken cancellationToken)
            {
                if (_lazyRoot == null)
                {
                    // Parse the syntax tree
                    var tree = SyntaxFactory.ParseSyntaxTree(_text, _options, _path, cancellationToken);
                    var root = CloneNodeAsRoot((AquilaSyntaxNode)tree.GetRoot(cancellationToken));

                    Interlocked.CompareExchange(ref _lazyRoot, root, null);
                }

                return _lazyRoot;
            }

            public override bool TryGetRoot([NotNullWhen(true)] out AquilaSyntaxNode? root)
            {
                root = _lazyRoot;
                return root != null;
            }

            public override bool HasCompilationUnitRoot
            {
                get { return true; }
            }

            public override AquilaParseOptions Options
            {
                get { return _options; }
            }

            [Obsolete("Obsolete due to performance problems, use CompilationOptions.SyntaxTreeOptionsProvider instead",
                error: false)]
            public override ImmutableDictionary<string, ReportDiagnostic> DiagnosticOptions => _diagnosticOptions;

            public override SyntaxReference GetReference(SyntaxNode node)
            {
                return new SimpleSyntaxReference(node);
            }

            public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
            {
                if (ReferenceEquals(_lazyRoot, root) && ReferenceEquals(_options, options))
                {
                    return this;
                }

                return new ParsedSyntaxTree(
                    textOpt: null,
                    _text.Encoding,
                    _text.ChecksumAlgorithm,
                    _path,
                    (AquilaParseOptions)options,
                    (AquilaSyntaxNode)root,
                    null,
                    _diagnosticOptions,
                    cloneRoot: true);
            }

            public override SyntaxTree WithFilePath(string path)
            {
                if (_path == path)
                {
                    return this;
                }

                if (TryGetRoot(out var root))
                {
                    return new ParsedSyntaxTree(
                        _text,
                        _text.Encoding,
                        _text.ChecksumAlgorithm,
                        path,
                        _options,
                        root,
                        null,
                        _diagnosticOptions,
                        cloneRoot: true);
                }
                else
                {
                    return new LazySyntaxTree(_text, _options, path, _diagnosticOptions);
                }
            }

            [Obsolete("Obsolete due to performance problems, use CompilationOptions.SyntaxTreeOptionsProvider instead",
                error: false)]
            public override SyntaxTree WithDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> options)
            {
                if (options is null)
                {
                    options = EmptyDiagnosticOptions;
                }

                if (ReferenceEquals(_diagnosticOptions, options))
                {
                    return this;
                }

                if (TryGetRoot(out var root))
                {
                    return new ParsedSyntaxTree(
                        _text,
                        _text.Encoding,
                        _text.ChecksumAlgorithm,
                        _path,
                        _options,
                        root,
                        null,
                        options,
                        cloneRoot: true);
                }
                else
                {
                    return new LazySyntaxTree(_text, _options, _path, options);
                }
            }
        }
    }
}