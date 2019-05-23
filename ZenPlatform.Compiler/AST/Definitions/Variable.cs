using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Describes a variable.
    /// </summary>
    public class Variable : Statement, ITypedNode, IAstSymbol
    {
        /// <summary>
        /// Create a variable object.
        /// </summary>
        public Variable(ILineInfo li, object value, string name, IType type) : base(li)
        {
            Name = name;
            Type = type;
            Value = value;
        }


        public string Name { get; set; }

        public SymbolType SymbolType => SymbolType.Variable;

        /// <summary>
        /// Variable type.
        /// </summary>
        public IType Type { get; set; }

        /// <summary>
        /// Variable initial value;
        /// </summary>
        public object Value;
    }
}