using System.Reflection.Emit;
using PrototypePlatformLanguage.AST.Definitions.Statements;
using PrototypePlatformLanguage.AST.Infrastructure;

namespace PrototypePlatformLanguage.AST.Definitions.Functions
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
        public InstructionsBody InstructionsBody;

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
        public Function(InstructionsBody instructionsBody, ParameterCollection parameters, string name, Type type)
        {
            InstructionsBody = instructionsBody;
            Parameters = parameters;
            Name = name;
            Type = type;
        }
    }
}