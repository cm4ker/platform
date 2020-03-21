using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Именованоое поле (переменная). В последствии раскручивания дерева эта переменная запишется в таблицу символов
    /// </summary>
    public partial class Name : Expression, ICanBeAssigned
    {
        private TypeSyntax _type;

        public override TypeSyntax Type
        {
            get
            {
                if (_type == null)
                {
                    var v = FirstParent<IScoped>().SymbolTable
                        .Find(this.Value, SymbolType.Variable | SymbolType.Property, this.GetScope());

                    if (v is VariableSymbol vs) return ((ITypedNode) vs.SyntaxObject).Type;
                    if (v is PropertySymbol ps) return ps.Property.Type;
                }

                return _type;
            }

            set { _type = value; }
        }
    }

    public partial class PropertyLookupExpression : ICanBeAssigned
    {
    }


    public partial class AssignFieldExpression : ICanBeAssigned
    {
    }

    public interface ICanBeAssigned
    {
    }
}