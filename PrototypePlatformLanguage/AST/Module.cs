namespace PrototypePlatformLanguage.AST
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
        public MethodBody MethodBody;

        /// <summary>
        /// Create a module object.
        /// </summary>
        public Module(MethodBody methodBody, string name)
        {
            MethodBody = methodBody;
            Name = name;
        }
    }
}