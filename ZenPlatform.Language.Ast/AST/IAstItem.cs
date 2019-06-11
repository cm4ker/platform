using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST.Definitions;
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

    public interface IAstNode : ILineInfo, IChildItem<AstNode>, IVisitable
    {
        int Line { get; set; }
        int Position { get; set; }
        AstNode Parent { get; set; }
        T GetParent<T>() where T : IAstNode;
        void Accept(IVisitor visitor);
    }

    public abstract class AstNode : IAstNode
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

        public T GetParent<T>() where T : IAstNode
        {
            if (Parent is null) return default(T);

            if (Parent is T p)
                return p;

            return Parent.GetParent<T>();
        }


        public abstract void Accept(IVisitor visitor);
    }
}