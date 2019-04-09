using System.Reflection.Emit;

namespace PrototypePlatformLanguage.AST
{
    /// <summary>
    /// Describes a function.
    /// </summary>
    public class Function : Member
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Function body.
        /// </summary>
        public MethodBody MethodBody;

        /// <summary>
        /// Function type.
        /// </summary>
        public Type Type;

        /// <summary>
        /// Function parameters.
        /// </summary>
        public ParameterCollection Parameters;

        /// <summary>
        /// IL method builder.
        /// </summary>
        public MethodBuilder Builder;

        /// <summary>
        /// Creates a function object.
        /// </summary>
        public Function(MethodBody methodBody, ParameterCollection parameters, string name, Type type)
        {
            MethodBody = methodBody;
            Parameters = parameters;
            Name = name;
            Type = type;
        }
    }
}