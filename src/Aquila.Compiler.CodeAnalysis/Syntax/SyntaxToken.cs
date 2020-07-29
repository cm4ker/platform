using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Misc;
using Aquila.Language.Ast.Text;
using Microsoft.CodeAnalysis;
using SyntaxNode = Aquila.Language.Ast.SyntaxNode;
using SyntaxTree = Aquila.Language.Ast.SyntaxTree;

namespace Aquila.Compiler.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        internal SyntaxToken(ILineInfo lineInfo, SyntaxKind kind, string text)
            : base(lineInfo, kind)
        {
            Text = text;
        }

        public string Text { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitSyntaxToken(this);
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.VisitSyntaxToken(this);
        }
    }
}