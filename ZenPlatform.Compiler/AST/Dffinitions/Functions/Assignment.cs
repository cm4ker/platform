using ZenPlatform.Compiler.AST.Definitions.Statements;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
{
    public class Assignment : Statement
    {
        public string Name;
        public Infrastructure.Expression Value;
        public Infrastructure.Expression Index;

        public Assignment(Infrastructure.Expression value, Infrastructure.Expression index, string name)
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

        public PostIncrementStatement(string name)
        {
            Name = new Name(name);
        }

        public PostIncrementStatement(Name name)
        {
            Name = name;
        }
    }
}