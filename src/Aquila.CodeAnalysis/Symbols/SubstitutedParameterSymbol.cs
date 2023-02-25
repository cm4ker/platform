using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    internal sealed class SubstitutedParameterSymbol : WrappedParameterSymbol
    {
        // initially set to map which is only used to get the type, which is once computed is stored here.
        private object _mapOrType;

        internal SubstitutedParameterSymbol(MethodSymbol containingSymbol, TypeMap map, ParameterSymbol originalParameter) :
            this((Symbol)containingSymbol, map, originalParameter)
        {
        }

        internal SubstitutedParameterSymbol(PropertySymbol containingSymbol, TypeMap map, ParameterSymbol originalParameter) :
            this((Symbol)containingSymbol, map, originalParameter)
        {
        }

        private SubstitutedParameterSymbol(Symbol containingSymbol, TypeMap map, ParameterSymbol originalParameter) :
            base(originalParameter)
        {
            Debug.Assert(originalParameter.IsDefinition);
            ContainingSymbol = containingSymbol;
            _mapOrType = map;
        }

        protected override Symbol OriginalSymbolDefinition => underlyingParameter.OriginalDefinition;

        public override Symbol ContainingSymbol { get; }

        internal override TypeSymbol Type
        {
            get
            {
                var mapOrType = _mapOrType;
                var type = mapOrType as TypeSymbol;
                if (type != null)
                {
                    return type;
                }

                TypeWithModifiers substituted = ((TypeMap)mapOrType).SubstituteType(this.underlyingParameter.Type);

                type = substituted.Type;

                if (substituted.CustomModifiers.IsDefaultOrEmpty)
                {
                    _mapOrType = type;
                }

                return type;
            }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers =>
            _mapOrType is TypeMap map
                ? map.SubstituteCustomModifiers(this.underlyingParameter.Type,
                    this.underlyingParameter.CustomModifiers)
                : this.underlyingParameter.CustomModifiers;
    }
}
