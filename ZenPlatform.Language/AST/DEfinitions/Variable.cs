using ZenPlatfrom.Language.AST.Definitions.Statements;

namespace ZenPlatfrom.Language.AST.Definitions
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
        public Type Type;

        /// <summary>
        /// Variable initial value;
        /// </summary>
        public object Value;

        /// <summary>
        /// Create a variable object.
        /// </summary>
        public Variable(object value, string name, Type type)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}