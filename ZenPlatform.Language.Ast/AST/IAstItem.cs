using ZenPlatform.Compiler.Contracts.Symbols;

using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Language.Ast.AST
{
    public class LineInfo : ILineInfo
    {
        public int Line { get; set; }
        public int Position { get; set; }
    }

    public class UnknownLineInfo : LineInfo
    {
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

        public IAstNode Parent { get; set; }

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