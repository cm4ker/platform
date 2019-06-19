using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Describes a structure.
    /// </summary>
    public class Structure : Statement, IAstSymbol
    {
        /// <summary>
        /// Structure variables.
        /// </summary>
        public VariableCollection Variables;


        /// <summary>
        /// Create a structure object.
        /// </summary>
        public Structure(ILineInfo li, VariableCollection variables, string name) : base(li)
        {
            Variables = variables;
            Name = name;
        }

        public string Name { get; set; }
        public SymbolType SymbolType { get; }


        public override void Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}