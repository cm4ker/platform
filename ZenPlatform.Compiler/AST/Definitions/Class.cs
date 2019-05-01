namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Олицетворяет класс в платформе
    /// </summary>
    public class Class : TypeEntity
    {
        /// <summary>
        /// Create a module object.
        /// </summary>
        public Class(TypeBody typeBody, string name)
        {
            TypeBody = typeBody;
            Name = name;
        }
    }
}