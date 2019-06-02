using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;

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
        public TypeNode Type;

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
            string name, TypeNode type) : base(li)
        {
            InstructionsBody = instructionsBody;
            Parameters = parameters ?? new ParameterCollection();
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public SymbolType SymbolType => SymbolType.Function;


        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Type);

            if (Parameters != null)
                foreach (var parameter in Parameters)
                {
                    visitor.Visit(parameter);
                }

            visitor.Visit(InstructionsBody);
        }
    }
}