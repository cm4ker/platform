namespace ZenPlatform.Language.AST.Definitions
{
    /// <summary>
    /// Describes a compilation unit.
    /// </summary>
    public class Module
    {
        /// <summary>
        /// Name of module.
        /// </summary>
        public string Name;

        /// <summary>
        /// Module body.
        /// </summary>
        public TypeBody TypeBody;

        /// <summary>
        /// Create a module object.
        /// </summary>
        public Module(TypeBody typeBody, string name)
        {
            TypeBody = typeBody;
            Name = name;
        }
    }
}