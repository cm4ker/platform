using System.Diagnostics;
using System.Threading;

namespace Aquila.Language.Ast.Symbols
{
    public class MetadataAssemblySymbol : NonMissingAssemblySymbol
    {
        /// <summary>
        /// An array of cached Cor types defined in this assembly.
        /// Lazily filled by GetDeclaredSpecialType method.
        /// </summary>
        private NamedTypeSymbol[] _lazySpecialTypes;


        /// <summary>
        /// How many Cor types have we cached so far.
        /// </summary>
        private int _cachedSpecialTypes;

        internal NamedTypeSymbol GetSpecialType(SpecialType type)
        {
            return CorLibrary.GetDeclaredSpecialType(type);
        }

        internal override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
        {
            if (_lazySpecialTypes == null || (object) _lazySpecialTypes[(int) type] == null)
            {
                MetadataTypeName emittedName =
                    MetadataTypeName.FromFullName(type.GetMetadataName(), useCLSCompliantNameArityEncoding: true);
                ModuleSymbol module = this.Modules[0];
                NamedTypeSymbol result = module.LookupTopLevelMetadataType(ref emittedName);
                // if (result.Kind != SymbolKind.ErrorType && result.DeclaredAccessibility != Accessibility.Public)
                // {
                //     result = new MissingMetadataTypeSymbol.TopLevel(module, ref emittedName, type);
                // }
                RegisterDeclaredSpecialType(result);
            }

            return _lazySpecialTypes[(int) type];
        }

        /// <summary>
        /// Register declaration of predefined CorLib type in this Assembly.
        /// </summary>
        /// <param name="corType"></param>
        internal sealed override void RegisterDeclaredSpecialType(NamedTypeSymbol corType)
        {
            SpecialType typeId = corType.SpecialType;

            if (_lazySpecialTypes == null)
            {
                Interlocked.CompareExchange(ref _lazySpecialTypes,
                    new NamedTypeSymbol[(int) SpecialType.Count + 1], null);
            }

            if ((object) Interlocked.CompareExchange(ref _lazySpecialTypes[(int) typeId], corType, null) != null)
            {
                Debug.Assert(ReferenceEquals(corType, _lazySpecialTypes[(int) typeId]) ||
                             (corType.Kind == SymbolKind.ErrorType &&
                              _lazySpecialTypes[(int) typeId].Kind == SymbolKind.ErrorType));
            }
            else
            {
                Interlocked.Increment(ref _cachedSpecialTypes);
                Debug.Assert(_cachedSpecialTypes > 0 && _cachedSpecialTypes <= (int) SpecialType.Count);
            }
        }
    }
}