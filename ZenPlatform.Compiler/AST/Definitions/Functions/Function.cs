using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
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
        public InstructionsBodyNode InstructionsBody;

        /// <summary>
        /// Function type.
        /// </summary>
        public ZType Type;

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
        public Function(InstructionsBodyNode instructionsBody, ParameterCollection parameters, string name, ZType type)
        {
            InstructionsBody = instructionsBody;
            Parameters = parameters;
            Name = name;
            Type = type;
        }
    }
}