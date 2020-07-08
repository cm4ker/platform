using Aquila.Compiler.Contracts;

namespace Aquila.Language.Ast.Symbols.PE
{
    public sealed class PENamedTypeSymbolNonGeneric : PENamedTypeSymbol
    {
        private readonly PEModuleSymbol _moduleSymbol;
        private readonly PENamespaceSymbol _containingNamespace;
        private readonly IType _typeDef;

        public PENamedTypeSymbolNonGeneric(PEModuleSymbol moduleSymbol,
            PENamespaceSymbol containingNamespace, IType typeDef)
        {
            _moduleSymbol = moduleSymbol;
            _containingNamespace = containingNamespace;
            _typeDef = typeDef;
        }
    }
}