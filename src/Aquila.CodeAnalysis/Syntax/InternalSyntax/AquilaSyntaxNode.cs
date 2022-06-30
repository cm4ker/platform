// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal abstract class AquilaSyntaxNode : GreenNode
    {
        internal AquilaSyntaxNode(SyntaxKind kind)
            : base((ushort)kind)
        {
            GreenStats.NoteGreen(this);
        }

        internal AquilaSyntaxNode(SyntaxKind kind, int fullWidth)
            : base((ushort)kind, fullWidth)
        {
            GreenStats.NoteGreen(this);
        }

        internal AquilaSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics)
            : base((ushort)kind, diagnostics)
        {
            GreenStats.NoteGreen(this);
        }

        internal AquilaSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, int fullWidth)
            : base((ushort)kind, diagnostics, fullWidth)
        {
            GreenStats.NoteGreen(this);
        }

        internal AquilaSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations)
            : base((ushort)kind, diagnostics, annotations)
        {
            GreenStats.NoteGreen(this);
        }

        internal AquilaSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations,
            int fullWidth)
            : base((ushort)kind, diagnostics, annotations, fullWidth)
        {
            GreenStats.NoteGreen(this);
        }

        internal AquilaSyntaxNode(ObjectReader reader)
            : base(reader)
        {
        }

        public override string Language
        {
            get { return LanguageConstants.LanguageId; }
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
            get { return false; } //this is DirectiveTriviaSyntax; }
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

        public abstract TResult Accept<TResult>(AquilaSyntaxVisitor<TResult> visitor);

        public abstract void Accept(AquilaSyntaxVisitor visitor);

        internal virtual DirectiveStack ApplyDirectives(DirectiveStack stack)
        {
            return ApplyDirectives(this, stack);
        }

        internal static DirectiveStack ApplyDirectives(GreenNode node, DirectiveStack stack)
        {
            if (node.ContainsDirectives)
            {
                for (int i = 0, n = node.SlotCount; i < n; i++)
                {
                    var child = node.GetSlot(i);
                    if (child != null)
                    {
                        stack = ApplyDirectivesToListOrNode(child, stack);
                    }
                }
            }

            return stack;
        }

        internal static DirectiveStack ApplyDirectivesToListOrNode(GreenNode listOrNode, DirectiveStack stack)
        {
            // If we have a list of trivia, then that node is not actually a CSharpSyntaxNode.
            // Just defer to our standard ApplyDirectives helper as it will do the appropriate
            // walking of this list to ApplyDirectives to the children.
            if (listOrNode.RawKind == GreenNode.ListKind)
            {
                return ApplyDirectives(listOrNode, stack);
            }
            else
            {
                // Otherwise, we must have an actual piece of C# trivia.  Just apply the stack
                // to that node directly.
                return ((AquilaSyntaxNode)listOrNode).ApplyDirectives(stack);
            }
        }

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

        public override Microsoft.CodeAnalysis.SyntaxToken CreateSeparator<TNode>(SyntaxNode element)
        {
            return new Microsoft.CodeAnalysis.SyntaxToken();
        }

        public override bool IsTriviaWithEndOfLine()
        {
            return this.Kind == SyntaxKind.EndOfLineTrivia
                   || this.Kind == SyntaxKind.SingleLineCommentTrivia;
        }

        // Use conditional weak table so we always return same identity for structured trivia
        private static readonly
            ConditionalWeakTable<SyntaxNode, Dictionary<Microsoft.CodeAnalysis.SyntaxTrivia, WeakReference<SyntaxNode>>>
            s_structuresTable
                = new ConditionalWeakTable<SyntaxNode,
                    Dictionary<Microsoft.CodeAnalysis.SyntaxTrivia, WeakReference<SyntaxNode>>>();

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
                            structure = Aquila.CodeAnalysis.Syntax.StructuredTriviaSyntax.Create(trivia);
                            structsInParent.Add(trivia, new WeakReference<SyntaxNode>(structure));
                        }
                        else if (!weakStructure.TryGetTarget(out structure))
                        {
                            structure = Aquila.CodeAnalysis.Syntax.StructuredTriviaSyntax.Create(trivia);
                            weakStructure.SetTarget(structure);
                        }
                    }

                    return structure;
                }
                else
                {
                    return Aquila.CodeAnalysis.Syntax.StructuredTriviaSyntax.Create(trivia);
                }
            }

            return null;
        }
    }
}