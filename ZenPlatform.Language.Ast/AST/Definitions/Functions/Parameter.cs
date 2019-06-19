using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a parameter.
    /// </summary>
    public class Parameter : AstNode, ITypedNode, IAstSymbol
    {
        /// <summary>
        /// Parameter type.
        /// </summary>
        public TypeNode Type { get; set; }

        /// <summary>
        /// Parameter pass method.
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// Create parameter object.
        /// </summary>
        public Parameter(ILineInfo li, string name, TypeNode type, PassMethod passMethod) : base(li)
        {
            Name = name;
            Type = type;
            PassMethod = passMethod;
        }

        public string Name { get; set; }
        public SymbolType SymbolType => SymbolType.Variable;

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Type);
        }
    }

    public interface ITypedNode
    {
        TypeNode Type { get; set; }
    }
}