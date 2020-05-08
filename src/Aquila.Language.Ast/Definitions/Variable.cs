using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.AST;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Describes a variable.
    /// </summary>
    public partial class Variable : ITypedNode
    {
        public SymbolType SymbolType => SymbolType.Variable;

        public SymbolScopeBySecurity SymbolScope { get; set; }


        public Variable(ILineInfo li, Expression value, string name) : this(li, value, name, value.Type)
        {
        }

        public override TypeSyntax Type
        {
            get
            {
                if (VariableType.Kind == TypeNodeKind.Unknown && Value != null)
                    return Value.Type;

                return VariableType;
            }
        }
    }

  


    public partial class ContextVariable : ITypedNode
    {
        public SymbolType SymbolType => SymbolType.Variable;

        public SymbolScopeBySecurity SymbolScope
        {
            get => SymbolScopeBySecurity.User;
            set { }
        }
    }
}