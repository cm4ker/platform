using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class ModuleSymbol : Symbol
    {
        public override SymbolKind Kind => SymbolKind.NetModule;

        internal abstract NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName);
    }
}