﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal abstract class CSharpSyntaxNode : GreenNode
    {
        internal CSharpSyntaxNode(SyntaxKind kind)
            : base((ushort)kind)
        {
            GreenStats.NoteGreen(this);
        }

        internal CSharpSyntaxNode(SyntaxKind kind, int fullWidth)
            : base((ushort)kind, fullWidth)
        {
            GreenStats.NoteGreen(this);
        }

        internal CSharpSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics)
            : base((ushort)kind, diagnostics)
        {
            GreenStats.NoteGreen(this);
        }

        internal CSharpSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, int fullWidth)
            : base((ushort)kind, diagnostics, fullWidth)
        {
            GreenStats.NoteGreen(this);
        }

        internal CSharpSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations)
            : base((ushort)kind, diagnostics, annotations)
        {
            GreenStats.NoteGreen(this);
        }

        internal CSharpSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations,
            int fullWidth)
            : base((ushort)kind, diagnostics, annotations, fullWidth)
        {
            GreenStats.NoteGreen(this);
        }

        internal CSharpSyntaxNode(ObjectReader reader)
            : base(reader)
        {
        }

        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }

        public SyntaxKind Kind
        {
            get { return (SyntaxKind)this.RawKind; }
        }

        public override string KindText => this.Kind.ToString();

        public override int RawContextualKind
        {
            get { return this.RawKind; }
        }

        public override bool IsStructuredTrivia
        {
            get { return this is StructuredTriviaSyntax; }
        }

        public override bool IsDirective
        {
            get { return this is DirectiveTriviaSyntax; }
        }

        public override bool IsSkippedTokensTrivia => this.Kind == SyntaxKind.SkippedTokensTrivia;
        public override bool IsDocumentationCommentTrivia => SyntaxFacts.IsDocumentationCommentTrivia(this.Kind);

        public override int GetSlotOffset(int index)
        {
            // This implementation should not support arbitrary
            // length lists since the implementation is O(n).
            System.Diagnostics.Debug.Assert(index < 11); // Max. slots 11 (TypeDeclarationSyntax)

            int offset = 0;
            for (int i = 0; i < index; i++)
            {
                var child = this.GetSlot(i);
                if (child != null)
                {
                    offset += child.FullWidth;
                }
            }

            return offset;
        }

        public SyntaxToken GetFirstToken()
        {
            return (SyntaxToken)this.GetFirstTerminal();
        }

        public SyntaxToken GetLastToken()
        {
            return (SyntaxToken)this.GetLastTerminal();
        }

        public SyntaxToken GetLastNonmissingToken()
        {
            return (SyntaxToken)this.GetLastNonmissingTerminal();
        }

        public virtual GreenNode GetLeadingTrivia()
        {
            return null;
        }

        public override GreenNode GetLeadingTriviaCore()
        {
            return this.GetLeadingTrivia();
        }

        public virtual GreenNode GetTrailingTrivia()
        {
            return null;
        }

        public override GreenNode GetTrailingTriviaCore()
        {
            return this.GetTrailingTrivia();
        }

        public abstract TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor);

        public abstract void Accept(CSharpSyntaxVisitor visitor);

        /// <summary>
        /// Should only be called during construction.
        /// </summary>
        /// <remarks>
        /// This should probably be an extra constructor parameter, but we don't need more constructor overloads.
        /// </remarks>
        protected void SetFactoryContext(SyntaxFactoryContext context)
        {
            if (context.IsInAsync)
            {
                this.flags |= NodeFlags.FactoryContextIsInAsync;
            }

            if (context.IsInQuery)
            {
                this.flags |= NodeFlags.FactoryContextIsInQuery;
            }
        }

        internal static NodeFlags SetFactoryContext(NodeFlags flags, SyntaxFactoryContext context)
        {
            if (context.IsInAsync)
            {
                flags |= NodeFlags.FactoryContextIsInAsync;
            }

            if (context.IsInQuery)
            {
                flags |= NodeFlags.FactoryContextIsInQuery;
            }

            return flags;
        }

        public override CodeAnalysis.SyntaxToken CreateSeparator<TNode>(SyntaxNode element)
        {
            return new CodeAnalysis.SyntaxToken();
        }

        public override bool IsTriviaWithEndOfLine()
        {
            return this.Kind == SyntaxKind.EndOfLineTrivia
                   || this.Kind == SyntaxKind.SingleLineCommentTrivia;
        }

        // Use conditional weak table so we always return same identity for structured trivia
        private static readonly
            ConditionalWeakTable<SyntaxNode, Dictionary<CodeAnalysis.SyntaxTrivia, WeakReference<SyntaxNode>>>
            s_structuresTable
                = new ConditionalWeakTable<SyntaxNode,
                    Dictionary<CodeAnalysis.SyntaxTrivia, WeakReference<SyntaxNode>>>();

        /// <summary>
        /// Gets the syntax node represented the structure of this trivia, if any. The HasStructure property can be used to 
        /// determine if this trivia has structure.
        /// </summary>
        /// <returns>
        /// A CSharpSyntaxNode derived from StructuredTriviaSyntax, with the structured view of this trivia node. 
        /// If this trivia node does not have structure, returns null.
        /// </returns>
        /// <remarks>
        /// Some types of trivia have structure that can be accessed as additional syntax nodes.
        /// These forms of trivia include: 
        ///   directives, where the structure describes the structure of the directive.
        ///   documentation comments, where the structure describes the XML structure of the comment.
        ///   skipped tokens, where the structure describes the tokens that were skipped by the parser.
        /// </remarks>
        public override SyntaxNode GetStructure(Microsoft.CodeAnalysis.SyntaxTrivia trivia)
        {
            if (trivia.HasStructure)
            {
                var parent = trivia.Token.Parent;
                if (parent != null)
                {
                    SyntaxNode structure;
                    var structsInParent = s_structuresTable.GetOrCreateValue(parent);
                    lock (structsInParent)
                    {
                        if (!structsInParent.TryGetValue(trivia, out var weakStructure))
                        {
                            structure = CSharp.Syntax.StructuredTriviaSyntax.Create(trivia);
                            structsInParent.Add(trivia, new WeakReference<SyntaxNode>(structure));
                        }
                        else if (!weakStructure.TryGetTarget(out structure))
                        {
                            structure = CSharp.Syntax.StructuredTriviaSyntax.Create(trivia);
                            weakStructure.SetTarget(structure);
                        }
                    }

                    return structure;
                }
                else
                {
                    return CSharp.Syntax.StructuredTriviaSyntax.Create(trivia);
                }
            }

            return null;
        }
    }
}