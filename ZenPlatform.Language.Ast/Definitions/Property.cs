using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Property : Member, IAstSymbol
    {
        private Block _getter;

        public Property(ILineInfo lineInfo, string name, TypeSyntax type, bool hasGet, bool hasSet) : this(lineInfo,
            name, type)
        {
            HasGetter = hasGet;
            HasSetter = hasSet;
        }

        public SymbolType SymbolType => SymbolType.Variable;

        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }


        public Block Getter
        {
            get { return _getter; }
            set
            {
                Childs.Remove(_getter);
                _getter = value;
                Childs.Add(_getter);
            }
        }

        public Block Setter { get; set; }
    }
}