using ZenPlatfrom.Language.AST.Infrastructure;

namespace ZenPlatfrom.Language.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a parameter.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Parameter type.
        /// </summary>
        public Type Type;

        /// <summary>
        /// Parameter pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create parameter object.
        /// </summary>
        public Parameter(string name, Type type, PassMethod passMethod)
        {
            Name = name;
            Type = type;
            PassMethod = passMethod;
        }
    }
}