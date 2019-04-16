using ZenPlatfrom.Language.AST.Definitions.Statements;
using ZenPlatfrom.Language.AST.Infrastructure;

namespace ZenPlatfrom.Language.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a function call statement.
    /// </summary>
    public class CallStatement : Statement
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
        public CallStatement(ArgumentCollection arguments, string name)
        {
            Arguments = arguments;
            Name = name;
        }
    }
}