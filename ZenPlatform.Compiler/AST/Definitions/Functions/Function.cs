using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a function.
    /// </summary>
    public class Function : Member, IAstSymbol
    {
        /// <summary>
        /// Function body.
        /// </summary>
        public InstructionsBodyNode InstructionsBody;

        /// <summary>
        /// Function type.
        /// </summary>
        public IType Type;

        /// <summary>
        /// Function parameters.
        /// </summary>
        public ParameterCollection Parameters;

        /// <summary>
        /// IL method builder.
        /// </summary>
        public IEmitter Builder;

        /// <summary>
        /// Creates a function object.
        /// </summary>
        public Function(ILineInfo li, InstructionsBodyNode instructionsBody, ParameterCollection parameters,
            string name, IType type) : base(li)
        {
            InstructionsBody = instructionsBody;
            Parameters = parameters;
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public SymbolType SymbolType => SymbolType.Function;
    }
}