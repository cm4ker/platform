using ZenPlatform.Compiler.AST.Definitions.Statements;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Describes a variable.
    /// </summary>
    public class Variable : Statement
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Variable type.
        /// </summary>
        public ZType Type;

        /// <summary>
        /// Variable initial value;
        /// </summary>
        public object Value;

        /// <summary>
        /// Create a variable object.
        /// </summary>
        public Variable(object value, string name, ZType type)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}