using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Describes a variable.
    /// </summary>
    public class Variable : Statement, ITypedNode, IAstSymbol
    {
        /// <summary>
        /// Create a variable object.
        /// </summary>
        public Variable(ILineInfo li, AstNode value, string name, TypeNode type) : base(li)
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
        public AstNode Value;

        public override void Accept<T>(IVisitor<T> visitor)
        {
            if (Value is AstNode an)
                visitor.Visit(an);
            visitor.Visit(Type);
        }
    }
}