// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Shared.Extensions
{
    public static partial class SyntaxTreeExtensions
    {
        public static Task<SyntaxToken> GetTouchingTokenAsync(
            this SyntaxTree syntaxTree,
            int position,
            CancellationToken cancellationToken,
            bool findInsideTrivia = false)
        {
            return GetTouchingTokenAsync(syntaxTree, position, _ => true, cancellationToken, findInsideTrivia);
        }

        public static async Task<SyntaxToken> GetTouchingTokenAsync(
            this SyntaxTree syntaxTree,
            int position,
            Predicate<SyntaxToken> predicate,
            CancellationToken cancellationToken,
            bool findInsideTrivia = false)
        {
            Contract.ThrowIfNull(syntaxTree);

            if (position >= syntaxTree.Length)
            {
                return default;
            }

            var root = await syntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(position, findInsideTrivia);

            if ((token.Span.Contains(position) || token.Span.End == position) && predicate(token))
            {
                return token;
            }

            token = token.GetPreviousToken();

            if (token.Span.End == position && predicate(token))
            {
                return token;
            }

            // SyntaxKind = None
            return default;
        }

        public static bool IsEntirelyHidden(this SyntaxTree tree, TextSpan span, CancellationToken cancellationToken)
        {
            if (!tree.HasHiddenRegions())
            {
                return false;
            }

            var text = tree.GetText(cancellationToken);
            var startLineNumber = text.Lines.IndexOf(span.Start);
            var endLineNumber = text.Lines.IndexOf(span.End);

            for (var lineNumber = startLineNumber; lineNumber <= endLineNumber; lineNumber++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var linePosition = text.Lines[lineNumber].Start;
                if (!tree.IsHiddenPosition(linePosition, cancellationToken))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsBeforeFirstToken(this SyntaxTree syntaxTree, int position,
            CancellationToken cancellationToken)
        {
            var root = syntaxTree.GetRoot(cancellationToken);
            var firstToken = root.GetFirstToken(includeZeroWidth: true, includeSkipped: true);

            return position <= firstToken.SpanStart;
        }

        public static SyntaxToken FindTokenOrEndToken(
            this SyntaxTree syntaxTree, int position, CancellationToken cancellationToken)
        {
            Contract.ThrowIfNull(syntaxTree);

            var root = syntaxTree.GetRoot(cancellationToken);
            var result = root.FindToken(position, findInsideTrivia: true);
            if (result.RawKind != 0)
            {
                return result;
            }

            // Special cases.  See if we're actually at the end of a:
            // a) doc comment
            // b) pp directive
            // c) file

            var compilationUnit = (ICompilationUnitSyntax)root;
            var triviaList = compilationUnit.EndOfFileToken.LeadingTrivia;
            foreach (var trivia in triviaList.Reverse())
            {
                if (trivia.HasStructure)
                {
                    var token = trivia.GetStructure()!.GetLastToken(includeZeroWidth: true);
                    if (token.Span.End == position)
                    {
                        return token;
                    }
                }
            }

            if (position == root.FullSpan.End)
            {
                return compilationUnit.EndOfFileToken;
            }

            return default;
        }
    }
}