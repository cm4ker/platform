// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    internal abstract partial class AquilaSyntaxVisitor<TResult>
    {
        public virtual TResult Visit(AquilaSyntaxNode node)
        {
            if (node == null)
            {
                return default(TResult);
            }

            return node.Accept(this);
        }

        public virtual TResult VisitToken(SyntaxToken token)
        {
            return this.DefaultVisit(token);
        }

        public virtual TResult VisitTrivia(SyntaxTrivia trivia)
        {
            return this.DefaultVisit(trivia);
        }

        protected virtual TResult DefaultVisit(AquilaSyntaxNode node)
        {
            return default(TResult);
        }
    }

    internal abstract partial class AquilaSyntaxVisitor
    {
        public virtual void Visit(AquilaSyntaxNode node)
        {
            if (node == null)
            {
                return;
            }

            node.Accept(this);
        }

        public virtual void VisitToken(SyntaxToken token)
        {
            this.DefaultVisit(token);
        }

        public virtual void VisitTrivia(SyntaxTrivia trivia)
        {
            this.DefaultVisit(trivia);
        }

        public virtual void DefaultVisit(AquilaSyntaxNode node)
        {
        }
    }
}
