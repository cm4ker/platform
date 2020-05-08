using System.Diagnostics;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    [DebuggerDisplay("{Name}")]
    public partial class Property : Member, IAstSymbol
    {
        private Block _getter;
        private Block _setter;

        public Property(ILineInfo lineInfo, string name, TypeSyntax type, bool hasGet, bool hasSet,
            string mapTo = null) : this(lineInfo,
            name, type, mapTo)
        {
            HasGetter = hasGet;
            HasSetter = hasSet;
        }

        public SymbolType SymbolType => SymbolType.Property;

        public SymbolScopeBySecurity SymbolScope { get; set; }

        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }

        public bool IsInterface { get; set; }

        public Block Getter
        {
            get { return _getter; }
            set
            {
                Detach(_getter);
                _getter = value;
                Attach(_getter);
            }
        }

        public Block Setter
        {
            get => _setter;
            set
            {
                Detach(_setter);
                _setter = value;
                Attach(_setter);
            }
        }
    }
}