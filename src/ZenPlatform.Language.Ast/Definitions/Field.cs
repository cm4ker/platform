using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Field : ITypedNode
    {
        public Field(ILineInfo li, string name, TypeSyntax type) : this(li, name)
        {
            Type = type;
        }

        public SymbolType SymbolType => SymbolType.Variable;
        public SymbolScopeBySecurity SymbolScope { get; set; }
        public TypeSyntax Type { get; set; }
    }
}