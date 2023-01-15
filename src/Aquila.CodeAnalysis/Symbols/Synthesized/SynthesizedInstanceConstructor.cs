using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    internal class SynthesizedInstanceConstructor : SynthesizedInstanceMethodSymbol
    {
        private readonly NamedTypeSymbol _containingType;

        internal SynthesizedInstanceConstructor(NamedTypeSymbol containingType)
        {
            Debug.Assert((object)containingType != null);
            _containingType = containingType;
        }

        //
        // Consider overriding when implementing a synthesized subclass.
        //

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get { return ImmutableArray<ParameterSymbol>.Empty; }
        }

        public override Accessibility DeclaredAccessibility
        {
            get { return ContainingType.IsAbstract ? Accessibility.Protected : Accessibility.Public; }
        }

        internal override bool IsMetadataFinal
        {
            get
            {
                return false;
            }
        }

        #region Sealed

        public sealed override Symbol ContainingSymbol
        {
            get { return _containingType; }
        }

        public sealed override NamedTypeSymbol ContainingType
        {
            get
            {
                return _containingType;
            }
        }

        public sealed override string Name
        {
            get { return WellKnownMemberNames.InstanceConstructorName; }
        }

        internal sealed override bool HasSpecialName
        {
            get { return true; }
        }

        internal sealed override System.Reflection.MethodImplAttributes ImplementationAttributes
        {
            get
            {
                if (_containingType.TypeKind == TypeKind.Delegate)
                {
                    return System.Reflection.MethodImplAttributes.Runtime;
                }

                return default(System.Reflection.MethodImplAttributes);
            }
        }

        internal sealed override bool RequiresSecurityObject
        {
            get { return false; }
        }

        public sealed override DllImportData GetDllImportData()
        {
            return null;
        }

        public sealed override bool IsVararg
        {
            get { return false; }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return ImmutableArray<TypeParameterSymbol>.Empty; }
        }

        public sealed override ImmutableArray<Location> Locations
        {
            get { return ContainingType.Locations; }
        }

        public override RefKind RefKind => RefKind.None;

        public sealed override TypeSymbol ReturnType
        {
            get { return ContainingAssembly.GetSpecialType(SpecialType.System_Void); }
        }

        public sealed override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
        {
            get { return ImmutableArray<CustomModifier>.Empty; }
        }

        public sealed override ImmutableArray<TypeSymbol> TypeArguments
        {
            get { return ImmutableArray<TypeSymbol>.Empty; }
        }

        public sealed override ISymbol AssociatedSymbol
        {
            get { return null; }
        }

        public sealed override int Arity
        {
            get { return 0; }
        }

        public sealed override bool ReturnsVoid
        {
            get { return true; }
        }

        public sealed override MethodKind MethodKind
        {
            get { return MethodKind.Constructor; }
        }

        public sealed override bool IsExtern
        {
            get
            {
                // Synthesized constructors of ComImport type are extern
                NamedTypeSymbol containingType = this.ContainingType;
                return (object)containingType != null && false/*containingType.IsComImport*/;
            }
        }

        public sealed override bool IsSealed
        {
            get { return false; }
        }

        public sealed override bool IsAbstract
        {
            get { return false; }
        }

        public sealed override bool IsOverride
        {
            get { return false; }
        }

        public sealed override bool IsVirtual
        {
            get { return false; }
        }

        public sealed override bool IsStatic
        {
            get { return false; }
        }

        public sealed override bool IsAsync
        {
            get { return false; }
        }

        public sealed override bool HidesBaseMethodsByName
        {
            get { return false; }
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public sealed override bool IsExtensionMethod
        {
            get { return false; }
        }

        public sealed override Cci.CallingConvention CallingConvention
        {
            get { return Cci.CallingConvention.HasThis; }
        }

        internal sealed override bool IsExplicitInterfaceImplementation
        {
            get { return false; }
        }

        public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get { return ImmutableArray<MethodSymbol>.Empty; }
        }

        #endregion
    }
}
