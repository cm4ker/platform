using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a function call.
    /// </summary>
    public class Call : Expression
    {
        /// <summary>
        /// Function to call.
        /// </summary>
        public string Name;

        /// <summary>
        /// Function arguments to pass.
        /// </summary>
        public ArgumentCollection Arguments;

        /// <summary>
        /// Creates a function call object.
        /// </summary>
        public Call(ILineInfo li, ArgumentCollection arguments, string name) : base(li)
        {
            Arguments = arguments;
            Name = name;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            foreach (var argument in Arguments)
                visitor.Visit(argument);
        }
    }
}