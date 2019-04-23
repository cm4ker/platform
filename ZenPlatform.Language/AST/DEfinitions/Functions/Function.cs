using Mono.Cecil.Cil;
using ZenPlatform.Language.AST.Definitions.Statements;
using ZenPlatform.Language.AST.Infrastructure;

namespace ZenPlatform.Language.AST.Definitions.Functions
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
        public ILProcessor Builder;

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