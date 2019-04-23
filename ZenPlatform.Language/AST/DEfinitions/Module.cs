namespace ZenPlatform.Language.AST.Definitions
{
    /// <summary>
    /// Описывает модуль
    /// </summary>
    public class Module : TypeEntity
    {
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