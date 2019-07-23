using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Describes a variable.
    /// </summary>
    public class Variable : Expression, ITypedNode, IAstSymbol
    {
        /// <summary>
        /// Create a variable object.
        /// </summary>
        public Variable(ILineInfo li, SyntaxNode value, string name, TypeNode type) : base(li)
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
        public TypeNode Type { get; set; }

        /// <summary>
        /// Variable initial value;
        /// </summary>
        public SyntaxNode Value;

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}