using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Language.Ast.AST
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

    public abstract class AstNode : ILineInfo, IChildItem<AstNode>, IVisitable
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

        public T GetParent<T>() where T : AstNode
        {
            if (Parent is null) return null;

            if (Parent is T p)
                return p;

            return Parent.GetParent<T>();
        }


        public abstract void Accept(IVisitor visitor);
    }
}