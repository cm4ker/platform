using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a parameter.
    /// </summary>
    public class Parameter : AstNode, ITypedNode, IAstSymbol
    {
        /// <summary>
        /// Parameter type.
        /// </summary>
        public IType Type { get; set; }

        /// <summary>
        /// Parameter pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create parameter object.
        /// </summary>
        public Parameter(ILineInfo li, string name, IType type, PassMethod passMethod) : base(li)
        {
            Name = name;
            Type = type;
            PassMethod = passMethod;
        }

        public string Name { get; set; }
        public SymbolType SymbolType => SymbolType.Variable;
    }

    public interface ITypedNode
    {
        IType Type { get; set; }
    }
}