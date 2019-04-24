using ZenPlatform.Language.AST.Definitions.Statements;

namespace ZenPlatform.Language.AST.Definitions.Functions
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
}