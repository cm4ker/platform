using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
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
    }
}