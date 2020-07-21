using Aquila.Language.Ast.Misc;
using Aquila.Language.Ast.Text;
using Aquila.Shared.Tree;

namespace Aquila.Language.Ast
{
    public abstract class SyntaxNode : Node, IAstNode
    {
        protected SyntaxNode(ILineInfo lineInfo, SyntaxKind kind)
        {
            Kind = kind;
            if (lineInfo != null)
            {
                Line = lineInfo.Line;
                Position = lineInfo.Position;
            }
        }

        public SyntaxKind Kind { get; }

        public int Line { get; set; }

        public int Position { get; set; }

        public int Length { get; set; }

        public TextLocation Location { get; set; }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);

        public abstract void Accept(AstVisitorBase visitor);
    }
}