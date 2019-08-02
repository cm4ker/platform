using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Property : Member, IAstSymbol
    {
        public Property(ILineInfo lineInfo, string name, TypeSyntax type, bool hasGet, bool hasSet) : this(lineInfo,
            name, type)
        {
            HasGetter = hasGet;
            HasSetter = hasSet;
        }

        public SymbolType SymbolType => SymbolType.Variable;

        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }

        public Block Getter { get; set; }
        public Block Setter { get; set; }
    }
}