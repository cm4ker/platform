using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast.Definitions.Functions
{
    /// <summary>
    /// Описывает параметр
    /// </summary>
    public partial class Parameter 
    {
        public Parameter(ILineInfo li, string name, Ast.TypeSyntax type, PassMethod pm) : this(li, name, pm)
        {
            Type = type;
        }

        public SymbolType SymbolType => SymbolType.Variable;

        public SymbolScopeBySecurity SymbolScope { get; set; }

        public Ast.TypeSyntax Type
        {
            get => (Ast.TypeSyntax) this.Children[0];
            set => this.Attach(0, (SyntaxNode) value.Clone());
        }
    }
}