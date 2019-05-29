using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
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

        public override void Accept(IVisitor visitor)
        {
            Arguments.ForEach(visitor.Visit);
        }
    }
}