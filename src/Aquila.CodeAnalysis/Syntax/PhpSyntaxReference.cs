using System;
using System.Threading;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Syntax
{
    /// <summary>
    /// this is a basic do-nothing implementation of a syntax reference
    /// </summary>
    internal class PhpSyntaxReference : SyntaxReference
    {
        readonly AquilaSyntaxTree _tree;
        readonly LangElement _node;

        internal PhpSyntaxReference(AquilaSyntaxTree tree, LangElement node)
        {
            _tree = tree;
            _node = node;
        }

        public override SyntaxTree SyntaxTree
        {
            get
            {
                return _tree;
            }
        }

        public override TextSpan Span
        {
            get
            {
                return _node.Span.ToTextSpan();
            }
        }

        public override SyntaxNode GetSyntax(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
