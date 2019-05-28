using Antlr4.Runtime;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Compiler.AST
{
    public interface ILineInfo
    {
        int Line { get; set; }
        int Position { get; set; }
    }

    public class LineInfo : ILineInfo
    {
        public int Line { get; set; }
        public int Position { get; set; }
    }

    public class UnknownLineInfo : LineInfo
    {
    }

    public static class Helper
    {
        public static ILineInfo ToLineInfo(this IToken token)
        {
            return new LineInfo {Line = token.Line, Position = token.Column};
        }
    }

    public abstract class AstNode : ILineInfo, IChildItem<AstNode>, IAstNode, IVisitable
    {
        public AstNode(ILineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                Line = lineInfo.Line;
                Position = lineInfo.Position;
            }
        }

        public int Line { get; set; }

        public int Position { get; set; }

        public AstNode Parent { get; set; }

        public abstract void Accept(IVisitor visitor);
    }
}