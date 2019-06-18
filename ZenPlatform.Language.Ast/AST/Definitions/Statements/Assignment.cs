using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class Assignment : Statement
    {
        public string Name;
        public Expression Value;
        public Expression Index;

        public Assignment(ILineInfo lineInfo, Expression value, Expression index, string name) : base(lineInfo)
        {
            Value = value;
            Name = name;
            Index = index;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Value);
            visitor.Visit(Index);
        }
    }

    /// <summary>
    /// Выражение для постинкрементирования
    /// </summary>
    public class PostIncrementStatement : Statement
    {
        public Name Name { get; }

        public PostIncrementStatement(ILineInfo li, string name) : this(li, new Name(li, name))
        {
        }

        public PostIncrementStatement(ILineInfo lineInfo, Name name) : base(lineInfo)
        {
            Name = name;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Name);
        }
    }

    public class PostDecrementStatement : Statement
    {
        public Name Name { get; }

        public PostDecrementStatement(ILineInfo li, string name) : this(li, new Name(li, name))
        {
        }


        public PostDecrementStatement(ILineInfo lineInfo, Name name) : base(lineInfo)
        {
            Name = name;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Name);
        }
    }
}