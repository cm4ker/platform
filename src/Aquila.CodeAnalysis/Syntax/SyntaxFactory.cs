// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using InternalSyntax = Aquila.CodeAnalysis.Syntax.InternalSyntax;

namespace Aquila.CodeAnalysis
{
    /// <summary>
    /// A class containing factory methods for constructing syntax nodes, tokens and trivia.
    /// </summary>
    public static partial class SyntaxFactory
    {
        /// <summary>
        /// A trivia with kind EndOfLineTrivia containing both the carriage return and line feed characters.
        /// </summary>
        public static SyntaxTrivia CarriageReturnLineFeed { get; } =
            Syntax.InternalSyntax.SyntaxFactory.CarriageReturnLineFeed;

        /// <summary>
        /// A trivia with kind EndOfLineTrivia containing a single line feed character.
        /// </summary>
        public static SyntaxTrivia LineFeed { get; } = Syntax.InternalSyntax.SyntaxFactory.LineFeed;

        /// <summary>
        /// A trivia with kind EndOfLineTrivia containing a single carriage return character.
        /// </summary>
        public static SyntaxTrivia CarriageReturn { get; } = Syntax.InternalSyntax.SyntaxFactory.CarriageReturn;

        /// <summary>
        ///  A trivia with kind WhitespaceTrivia containing a single space character.
        /// </summary>
        public static SyntaxTrivia Space { get; } = Syntax.InternalSyntax.SyntaxFactory.Space;

        /// <summary>
        /// A trivia with kind WhitespaceTrivia containing a single tab character.
        /// </summary>
        public static SyntaxTrivia Tab { get; } = Syntax.InternalSyntax.SyntaxFactory.Tab;

        /// <summary>
        /// An elastic trivia with kind EndOfLineTrivia containing both the carriage return and line feed characters.
        /// Elastic trivia are used to denote trivia that was not produced by parsing source text, and are usually not
        /// preserved during formatting.
        /// </summary>
        public static SyntaxTrivia ElasticCarriageReturnLineFeed { get; } =
            Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturnLineFeed;

        /// <summary>
        /// An elastic trivia with kind EndOfLineTrivia containing a single line feed character. Elastic trivia are used
        /// to denote trivia that was not produced by parsing source text, and are usually not preserved during
        /// formatting.
        /// </summary>
        public static SyntaxTrivia ElasticLineFeed { get; } = Syntax.InternalSyntax.SyntaxFactory.ElasticLineFeed;

        /// <summary>
        /// An elastic trivia with kind EndOfLineTrivia containing a single carriage return character. Elastic trivia
        /// are used to denote trivia that was not produced by parsing source text, and are usually not preserved during
        /// formatting.
        /// </summary>
        public static SyntaxTrivia ElasticCarriageReturn { get; } =
            Syntax.InternalSyntax.SyntaxFactory.ElasticCarriageReturn;

        /// <summary>
        /// An elastic trivia with kind WhitespaceTrivia containing a single space character. Elastic trivia are used to
        /// denote trivia that was not produced by parsing source text, and are usually not preserved during formatting.
        /// </summary>
        public static SyntaxTrivia ElasticSpace { get; } = Syntax.InternalSyntax.SyntaxFactory.ElasticSpace;

        /// <summary>
        /// An elastic trivia with kind WhitespaceTrivia containing a single tab character. Elastic trivia are used to
        /// denote trivia that was not produced by parsing source text, and are usually not preserved during formatting.
        /// </summary>
        public static SyntaxTrivia ElasticTab { get; } = Syntax.InternalSyntax.SyntaxFactory.ElasticTab;

        /// <summary>
        /// An elastic trivia with kind WhitespaceTrivia containing no characters. Elastic marker trivia are included
        /// automatically by factory methods when trivia is not specified. Syntax formatting will replace elastic
        /// markers with appropriate trivia.
        /// </summary>
        public static SyntaxTrivia ElasticMarker { get; } = Syntax.InternalSyntax.SyntaxFactory.ElasticZeroSpace;

        /// <summary>
        /// Creates a trivia with kind EndOfLineTrivia containing the specified text. 
        /// </summary>
        /// <param name="text">The text of the end of line. Any text can be specified here, however only carriage return and
        /// line feed characters are recognized by the parser as end of line.</param>
        public static SyntaxTrivia EndOfLine(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.EndOfLine(text, elastic: false);
        }

        /// <summary>
        /// Creates a trivia with kind EndOfLineTrivia containing the specified text. Elastic trivia are used to
        /// denote trivia that was not produced by parsing source text, and are usually not preserved during formatting.
        /// </summary>
        /// <param name="text">The text of the end of line. Any text can be specified here, however only carriage return and
        /// line feed characters are recognized by the parser as end of line.</param>
        public static SyntaxTrivia ElasticEndOfLine(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.EndOfLine(text, elastic: true);
        }

        [Obsolete("Use SyntaxFactory.EndOfLine or SyntaxFactory.ElasticEndOfLine")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SyntaxTrivia EndOfLine(string text, bool elastic)
        {
            return Syntax.InternalSyntax.SyntaxFactory.EndOfLine(text, elastic);
        }

        /// <summary>
        /// Creates a trivia with kind WhitespaceTrivia containing the specified text.
        /// </summary>
        /// <param name="text">The text of the whitespace. Any text can be specified here, however only specific
        /// whitespace characters are recognized by the parser.</param>
        public static SyntaxTrivia Whitespace(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.Whitespace(text, elastic: false);
        }

        /// <summary>
        /// Creates a trivia with kind WhitespaceTrivia containing the specified text. Elastic trivia are used to
        /// denote trivia that was not produced by parsing source text, and are usually not preserved during formatting.
        /// </summary>
        /// <param name="text">The text of the whitespace. Any text can be specified here, however only specific
        /// whitespace characters are recognized by the parser.</param>
        public static SyntaxTrivia ElasticWhitespace(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.Whitespace(text, elastic: false);
        }

        [Obsolete("Use SyntaxFactory.Whitespace or SyntaxFactory.ElasticWhitespace")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SyntaxTrivia Whitespace(string text, bool elastic)
        {
            return Syntax.InternalSyntax.SyntaxFactory.Whitespace(text, elastic);
        }

        /// <summary>
        /// Creates a trivia with kind either SingleLineCommentTrivia or MultiLineCommentTrivia containing the specified
        /// text.
        /// </summary>
        /// <param name="text">The entire text of the comment including the leading '//' token for single line comments
        /// or stop or start tokens for multiline comments.</param>
        public static SyntaxTrivia Comment(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.Comment(text);
        }

        /// <summary>
        /// Creates a trivia with kind DisabledTextTrivia. Disabled text corresponds to any text between directives that
        /// is not considered active.
        /// </summary>
        public static SyntaxTrivia DisabledText(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.DisabledText(text);
        }

        /// <summary>
        /// Creates a trivia with kind PreprocessingMessageTrivia.
        /// </summary>
        public static SyntaxTrivia PreprocessingMessage(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.PreprocessingMessage(text);
        }

        /// <summary>
        /// Trivia nodes represent parts of the program text that are not parts of the
        /// syntactic grammar, such as spaces, newlines, comments, preprocessor
        /// directives, and disabled code.
        /// </summary>
        /// <param name="kind">
        /// A <see cref="SyntaxKind"/> representing the specific kind of <see cref="SyntaxTrivia"/>. One of
        /// <see cref="SyntaxKind.WhitespaceTrivia"/>, <see cref="SyntaxKind.EndOfLineTrivia"/>,
        /// <see cref="SyntaxKind.SingleLineCommentTrivia"/>, <see cref="SyntaxKind.MultiLineCommentTrivia"/>,
        /// <see cref="SyntaxKind.DocumentationCommentExteriorTrivia"/>, <see cref="SyntaxKind.DisabledTextTrivia"/>
        /// </param>
        /// <param name="text">
        /// The actual text of this token.
        /// </param>
        public static SyntaxTrivia SyntaxTrivia(SyntaxKind kind, string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            switch (kind)
            {
                case SyntaxKind.DisabledTextTrivia:
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                case SyntaxKind.EndOfLineTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.WhitespaceTrivia:
                    return new SyntaxTrivia(default(SyntaxToken),
                        new Syntax.InternalSyntax.SyntaxTrivia(kind, text, null, null), 0, 0);
                default:
                    throw new ArgumentException("kind");
            }
        }

        /// <summary>
        /// Creates a token corresponding to a syntax kind. This method can be used for token syntax kinds whose text
        /// can be inferred by the kind alone.
        /// </summary>
        /// <param name="kind">A syntax kind value for a token. These have the suffix Token or Keyword.</param>
        /// <returns></returns>
        public static SyntaxToken Token(SyntaxKind kind)
        {
            return new SyntaxToken(Syntax.InternalSyntax.SyntaxFactory.Token(ElasticMarker.UnderlyingNode, kind,
                ElasticMarker.UnderlyingNode));
        }

        /// <summary>
        /// Creates a token corresponding to syntax kind. This method can be used for token syntax kinds whose text can
        /// be inferred by the kind alone.
        /// </summary>
        /// <param name="leading">A list of trivia immediately preceding the token.</param>
        /// <param name="kind">A syntax kind value for a token. These have the suffix Token or Keyword.</param>
        /// <param name="trailing">A list of trivia immediately following the token.</param>
        public static SyntaxToken Token(SyntaxTriviaList leading, SyntaxKind kind, SyntaxTriviaList trailing)
        {
            return new SyntaxToken(Syntax.InternalSyntax.SyntaxFactory.Token(leading.Node, kind, trailing.Node));
        }


        /// <summary>
        /// Create a new syntax tree from a syntax node.
        /// </summary>
        public static SyntaxTree SyntaxTree(SyntaxNode root, ParseOptions? options = null, string path = "",
            Encoding? encoding = null)
        {
            return AquilaSyntaxTree.Create((AquilaSyntaxNode)root, (AquilaParseOptions?)options, path, encoding);
        }

        /// <summary>
        /// Determines if two trees are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldTree">The original tree.</param>
        /// <param name="newTree">The new tree.</param>
        /// <param name="topLevel"> 
        /// If true then the trees are equivalent if the contained nodes and tokens declaring
        /// metadata visible symbolic information are equivalent, ignoring any differences of nodes inside method bodies
        /// or initializer expressions, otherwise all nodes and tokens must be equivalent. 
        /// </param>
        public static bool AreEquivalent(SyntaxTree? oldTree, SyntaxTree? newTree, bool topLevel)
        {
            if (oldTree == null && newTree == null)
            {
                return true;
            }

            if (oldTree == null || newTree == null)
            {
                return false;
            }

            return SyntaxEquivalence.AreEquivalent(oldTree, newTree, ignoreChildNode: null, topLevel: topLevel);
        }

        /// <summary>
        /// Determines if two syntax nodes are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldNode">The old node.</param>
        /// <param name="newNode">The new node.</param>
        /// <param name="topLevel"> 
        /// If true then the nodes are equivalent if the contained nodes and tokens declaring
        /// metadata visible symbolic information are equivalent, ignoring any differences of nodes inside method bodies
        /// or initializer expressions, otherwise all nodes and tokens must be equivalent. 
        /// </param>
        public static bool AreEquivalent(SyntaxNode? oldNode, SyntaxNode? newNode, bool topLevel)
        {
            return SyntaxEquivalence.AreEquivalent(oldNode, newNode, ignoreChildNode: null, topLevel: topLevel);
        }

        /// <summary>
        /// Determines if two syntax nodes are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldNode">The old node.</param>
        /// <param name="newNode">The new node.</param>
        /// <param name="ignoreChildNode">
        /// If specified called for every child syntax node (not token) that is visited during the comparison. 
        /// If it returns true the child is recursively visited, otherwise the child and its subtree is disregarded.
        /// </param>
        public static bool AreEquivalent(SyntaxNode? oldNode, SyntaxNode? newNode,
            Func<SyntaxKind, bool>? ignoreChildNode = null)
        {
            return SyntaxEquivalence.AreEquivalent(oldNode, newNode, ignoreChildNode: ignoreChildNode, topLevel: false);
        }

        /// <summary>
        /// Determines if two syntax tokens are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldToken">The old token.</param>
        /// <param name="newToken">The new token.</param>
        public static bool AreEquivalent(SyntaxToken oldToken, SyntaxToken newToken)
        {
            return SyntaxEquivalence.AreEquivalent(oldToken, newToken);
        }

        /// <summary>
        /// Determines if two lists of tokens are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldList">The old token list.</param>
        /// <param name="newList">The new token list.</param>
        public static bool AreEquivalent(SyntaxTokenList oldList, SyntaxTokenList newList)
        {
            return SyntaxEquivalence.AreEquivalent(oldList, newList);
        }

        /// <summary>
        /// Determines if two lists of syntax nodes are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="topLevel"> 
        /// If true then the nodes are equivalent if the contained nodes and tokens declaring
        /// metadata visible symbolic information are equivalent, ignoring any differences of nodes inside method bodies
        /// or initializer expressions, otherwise all nodes and tokens must be equivalent. 
        /// </param>
        public static bool AreEquivalent<TNode>(SyntaxList<TNode> oldList, SyntaxList<TNode> newList, bool topLevel)
            where TNode : AquilaSyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, null, topLevel);
        }

        /// <summary>
        /// Determines if two lists of syntax nodes are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="ignoreChildNode">
        /// If specified called for every child syntax node (not token) that is visited during the comparison. 
        /// If it returns true the child is recursively visited, otherwise the child and its subtree is disregarded.
        /// </param>
        public static bool AreEquivalent<TNode>(SyntaxList<TNode> oldList, SyntaxList<TNode> newList,
            Func<SyntaxKind, bool>? ignoreChildNode = null)
            where TNode : SyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, ignoreChildNode, topLevel: false);
        }

        /// <summary>
        /// Determines if two lists of syntax nodes are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="topLevel"> 
        /// If true then the nodes are equivalent if the contained nodes and tokens declaring
        /// metadata visible symbolic information are equivalent, ignoring any differences of nodes inside method bodies
        /// or initializer expressions, otherwise all nodes and tokens must be equivalent. 
        /// </param>
        public static bool AreEquivalent<TNode>(SeparatedSyntaxList<TNode> oldList, SeparatedSyntaxList<TNode> newList,
            bool topLevel)
            where TNode : SyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, null, topLevel);
        }

        /// <summary>
        /// Determines if two lists of syntax nodes are the same, disregarding trivia differences.
        /// </summary>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="ignoreChildNode">
        /// If specified called for every child syntax node (not token) that is visited during the comparison. 
        /// If it returns true the child is recursively visited, otherwise the child and its subtree is disregarded.
        /// </param>
        public static bool AreEquivalent<TNode>(SeparatedSyntaxList<TNode> oldList, SeparatedSyntaxList<TNode> newList,
            Func<SyntaxKind, bool>? ignoreChildNode = null)
            where TNode : SyntaxNode
        {
            return SyntaxEquivalence.AreEquivalent(oldList.Node, newList.Node, ignoreChildNode, topLevel: false);
        }

        /// <summary>
        /// Parse a CompilationUnitSyntax using the grammar rule for an entire compilation unit (file). To produce a
        /// SyntaxTree instance, use CSharpSyntaxTree.ParseText instead.
        /// </summary>
        /// <param name="text">The text of the compilation unit.</param>
        /// <param name="offset">Optional offset into text.</param>
        /// <param name="options">The optional parse options to use. If no options are specified default options are
        /// used.</param>
        public static CompilationUnitSyntax ParseCompilationUnit(string text, int offset = 0,
            AquilaParseOptions? options = null)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc cref="AquilaSyntaxTree.ParseText(string, AquilaParseOptions?, string, Encoding?, CancellationToken)"/>
        public static SyntaxTree ParseSyntaxTree(
            string text,
            ParseOptions? options = null,
            string path = "",
            Encoding? encoding = null,
            CancellationToken cancellationToken = default)
        {
            return AquilaSyntaxTree.ParseText(text, (AquilaParseOptions?)options, path, encoding, cancellationToken);
        }

        /// <inheritdoc cref="AquilaSyntaxTree.ParseText(SourceText, AquilaParseOptions?, string, CancellationToken)"/>
        public static SyntaxTree ParseSyntaxTree(
            SourceText text,
            ParseOptions? options = null,
            string path = "",
            CancellationToken cancellationToken = default)
        {
            return AquilaSyntaxTree.ParseText(text, (AquilaParseOptions?)options, path, cancellationToken);
        }
        
        /// <summary>
        /// Creates an empty list of trivia.
        /// </summary>
        public static SyntaxTriviaList TriviaList()
        {
            return default(SyntaxTriviaList);
        }

        /// <summary>
        /// Creates a singleton list of trivia.
        /// </summary>
        /// <param name="trivia">A single trivia.</param>
        public static SyntaxTriviaList TriviaList(SyntaxTrivia trivia)
        {
            return new SyntaxTriviaList(trivia);
        }

        /// <summary>
        /// Creates a list of trivia.
        /// </summary>
        /// <param name="trivias">An array of trivia.</param>
        public static SyntaxTriviaList TriviaList(params SyntaxTrivia[] trivias)
            => new SyntaxTriviaList(trivias);

        /// <summary>
        /// Creates a list of trivia.
        /// </summary>
        /// <param name="trivias">A sequence of trivia.</param>
        public static SyntaxTriviaList TriviaList(IEnumerable<SyntaxTrivia> trivias)
            => new SyntaxTriviaList(trivias);
        
        /// <summary>
        /// Creates a trivia with kind DocumentationCommentExteriorTrivia.
        /// </summary>
        /// <param name="text">The raw text of the literal.</param>
        public static SyntaxTrivia DocumentationCommentExterior(string text)
        {
            return Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentExteriorTrivia(text);
        }
        
        /// <summary>
        /// Creates a token with kind IdentifierToken containing the specified text.
        /// <param name="text">The raw text of the identifier name, including any escapes or leading '@'
        /// character.</param>
        /// </summary>
        public static SyntaxToken Identifier(string text)
        {
            return new SyntaxToken(Syntax.InternalSyntax.SyntaxFactory.Identifier(ElasticMarker.UnderlyingNode, text, ElasticMarker.UnderlyingNode));
        }
        
        /// <summary>
        /// Creates an IdentifierNameSyntax node.
        /// </summary>
        /// <param name="name">The identifier name.</param>
        public static IdentifierEx IdentifierName(string name)
        {
            return IdentifierEx(Identifier(name));
        }
    }
}