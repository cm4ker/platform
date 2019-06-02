using ZenPlatform.Language.AST.Definitions.Statements;
using ZenPlatform.Language.AST.Infrastructure;

namespace ZenPlatform.Language.AST.Definitions
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