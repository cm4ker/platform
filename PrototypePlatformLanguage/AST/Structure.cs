namespace PrototypePlatformLanguage.AST
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