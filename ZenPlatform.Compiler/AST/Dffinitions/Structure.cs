using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Describes a structure.
    /// </summary>
    public class Structure : Statement
    {
        /// <summary>
        /// Structure name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Structure variables.
        /// </summary>
        public VariableCollection Variables = null;

        
        
        /// <summary>
        /// Create a structure object.
        /// </summary>
        public Structure(VariableCollection variables, string name)
        {
            Variables = variables;
            Name = name;
        }
    }
}