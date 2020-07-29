using Aquila.Compiler.Contracts;

namespace Aquila.Language.Ast.Symbols.PE
{
    public sealed class PENamedTypeSymbolGeneric : PENamedTypeSymbol
    {
        public PENamedTypeSymbolGeneric(IType typeDef) : base(null, null, typeDef.Namespace, typeDef.Name)
        {
        }
    }
}