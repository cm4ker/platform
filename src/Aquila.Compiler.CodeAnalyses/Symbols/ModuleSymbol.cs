using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class ModuleSymbol : Symbol
    {
        public override SymbolKind Kind => SymbolKind.NetModule;

        public abstract NamespaceSymbol GlobalNamespace { get; }

        public abstract AssemblySymbol ContainingAssembly { get; }

        internal abstract NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName);
    }
}