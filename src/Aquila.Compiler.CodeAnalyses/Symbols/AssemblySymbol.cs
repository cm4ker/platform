using System;
using System.Collections.Immutable;
using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class AssemblySymbol : Symbol
    {
        private AssemblySymbol _corLibrary;

        internal AssemblySymbol CorLibrary
        {
            get { return _corLibrary; }
        }

        public void SetCorLibrary(AssemblySymbol symbol)
        {
        }


        internal abstract NamedTypeSymbol GetDeclaredSpecialType(SpecialType type);

        internal NamedTypeSymbol GetSpecialType(SpecialType type)
        {
            return CorLibrary.GetDeclaredSpecialType(type);
        }


        internal virtual void RegisterDeclaredSpecialType(NamedTypeSymbol corType)
        {
            throw new Exception("unreacheable");
        }

        public override SymbolKind Kind => SymbolKind.Assembly;

        public virtual ImmutableArray<ModuleSymbol> Modules => ImmutableArray<ModuleSymbol>.Empty;
    }
}