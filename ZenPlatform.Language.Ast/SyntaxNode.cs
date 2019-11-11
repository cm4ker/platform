using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Language.Ast
{
    public abstract class SyntaxNode : Node, IAstNode
    {
        public SyntaxNode(ILineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                Line = lineInfo.Line;
                Position = lineInfo.Position;
            }

            //  Children = new AstNodeCollection();
        }

        public int Line { get; set; }

        public int Position { get; set; }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);
    }
}