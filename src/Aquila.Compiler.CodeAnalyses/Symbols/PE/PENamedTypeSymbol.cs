using Aquila.Language.Ast.Extension;
using Microsoft.CodeAnalysis;
using SpecialType = Aquila.Language.Ast.Extension.SpecialType;

namespace Aquila.Language.Ast.Symbols.PE
{
    public abstract class PENamedTypeSymbol : NamedTypeSymbol
    {
        private SpecialType _corTypeId;
        private readonly NamespaceOrTypeSymbol _container;


        protected PENamedTypeSymbol(PEModuleSymbol moduleSymbol, NamespaceOrTypeSymbol container, string ns,
            string name)
        {
            _container = container;

            // check if this is one of the COR library types
            if (ns != null && moduleSymbol.ContainingAssembly.IsCorLib) // NB: this.flags was set above.
            {
                _corTypeId =
                    SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(ns, name));
            }
            else
            {
                _corTypeId = SpecialType.None;
            }
        }

        public override Symbol ContainingSymbol => _container;

        public override SpecialType SpecialType => _corTypeId;
    }
}