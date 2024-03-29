﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Represents a type parameter in a generic type or generic method.
    /// </summary>
    internal abstract partial class TypeParameterSymbol : TypeSymbol, ITypeParameterSymbol
    {
        /// <summary>
        /// The original definition of this symbol. If this symbol is constructed from another
        /// symbol by type substitution then OriginalDefinition gets the original symbol as it was defined in
        /// source or metadata.
        /// </summary>
        public new virtual TypeParameterSymbol OriginalDefinition => this;

        protected override sealed TypeSymbol OriginalTypeSymbolDefinition => this.OriginalDefinition;

        /// <summary>
        /// If this is a type parameter of a reduced extension method, gets the type parameter definition that
        /// this type parameter was reduced from. Otherwise, returns Nothing.
        /// </summary>
        public virtual TypeParameterSymbol ReducedFrom
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The ordinal position of the type parameter in the parameter list which declares
        /// it. The first type parameter has ordinal zero.
        /// </summary>
        public abstract int Ordinal
        {
#pragma warning disable S125
            // This is needed to determine hiding in C#: 
            //
            // interface IB { void M<T>(C<T> x); }
            // interface ID : IB { new void M<U>(C<U> x); }
            //
            // ID.M<U> hides IB.M<T> even though their formal parameters have different
            // types. When comparing formal parameter types for hiding purposes we must
            // compare method type parameters by ordinal, not by identity.
#pragma warning restore S125
            get;
        }

        /// <summary>
        /// The types that were directly specified as constraints on the type parameter.
        /// Duplicates and cycles are removed, although the collection may include
        /// redundant constraints where one constraint is a base type of another.
        /// </summary>
        public ImmutableArray<TypeSymbol> ConstraintTypes
        {
            get
            {
                return this.ConstraintTypesNoUseSiteDiagnostics;
            }
        }

        internal ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics
        {
            get
            {
                this.EnsureAllConstraintsAreResolved();
                return this.GetConstraintTypes(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal ImmutableArray<TypeSymbol> ConstraintTypesWithDefinitionUseSiteDiagnostics(ref HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            var result = ConstraintTypesNoUseSiteDiagnostics;

            return result;
        }

        /// <summary>
        /// True if the parameterless constructor constraint was specified for the type parameter.
        /// </summary>
        public abstract bool HasConstructorConstraint { get; }

        public virtual bool HasUnmanagedTypeConstraint => false;

        /// <summary>
        /// The type parameter kind of this type parameter.
        /// </summary>
        public abstract TypeParameterKind TypeParameterKind { get; }

        /// <summary>
        /// The method that declared this type parameter, or null.
        /// </summary>
        public MethodSymbol DeclaringMethod
        {
            get
            {
                return this.ContainingSymbol as MethodSymbol;
            }
        }

        /// <summary>
        /// The type that declared this type parameter, or null.
        /// </summary>
        public NamedTypeSymbol DeclaringType
        {
            get
            {
                return this.ContainingSymbol as NamedTypeSymbol;
            }
        }

        // Type parameters do not have members
        public sealed override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        // Type parameters do not have members
        public sealed override ImmutableArray<Symbol> GetMembers(string name) => ImmutableArray<Symbol>.Empty;

        
        // Type parameters do not have members
        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        // Type parameters do not have members
        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        // Type parameters do not have members
        public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public sealed override SymbolKind Kind
        {
            get
            {
                return SymbolKind.TypeParameter;
            }
        }

        public sealed override TypeKind TypeKind
        {
            get
            {
                return TypeKind.TypeParameter;
            }
        }

        // Only the compiler can create TypeParameterSymbols.
        internal TypeParameterSymbol()
        {
        }

        public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public sealed override bool IsStatic => false;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsSealed => false;

        public override ImmutableArray<NamedTypeSymbol> Interfaces => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> AllInterfaces => ImmutableArray<NamedTypeSymbol>.Empty;

        /// <summary>
        /// The effective base class of the type parameter (spec 10.1.5). If the deduced
        /// base type is a reference type, the effective base type will be the same as
        /// the deduced base type. Otherwise if the deduced base type is a value type,
        /// the effective base type will be the most derived reference type from which
        /// deduced base type is derived.
        /// </summary>
        internal NamedTypeSymbol EffectiveBaseClassNoUseSiteDiagnostics
        {
            get
            {
                this.EnsureAllConstraintsAreResolved();
                return this.GetEffectiveBaseClass(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal NamedTypeSymbol EffectiveBaseClass(ref HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            var result = EffectiveBaseClassNoUseSiteDiagnostics;

            return result;
        }

        /// <summary>
        /// The effective interface set (spec 10.1.5).
        /// </summary>
        internal ImmutableArray<NamedTypeSymbol> EffectiveInterfacesNoUseSiteDiagnostics
        {
            get
            {
                this.EnsureAllConstraintsAreResolved();
                return this.GetInterfaces(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        /// <summary>
        /// The most encompassed type (spec 6.4.2) from the constraints.
        /// </summary>
        internal TypeSymbol DeducedBaseTypeNoUseSiteDiagnostics
        {
            get
            {
                this.EnsureAllConstraintsAreResolved();
                return this.GetDeducedBaseType(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal TypeSymbol DeducedBaseType(ref HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            var result = DeducedBaseTypeNoUseSiteDiagnostics;
            return result;
        }

        /// <summary>
        /// The effective interface set and any base interfaces of those
        /// interfaces. This is AllInterfaces excluding interfaces that are
        /// only implemented by the effective base type.
        /// </summary>
        internal ImmutableArray<NamedTypeSymbol> AllEffectiveInterfacesNoUseSiteDiagnostics => AllInterfaces;

        internal ImmutableArray<NamedTypeSymbol> AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            var result = AllEffectiveInterfacesNoUseSiteDiagnostics;
            return result;
        }

        internal abstract void EnsureAllConstraintsAreResolved();

        /// <summary>
        /// Helper method to force type parameter constraints to be resolved.
        /// </summary>
        protected static void EnsureAllConstraintsAreResolved(ImmutableArray<TypeParameterSymbol> typeParameters)
        {
            foreach (var typeParameter in typeParameters)
            {
                // Invoke any method that forces constraints to be resolved.
                var unused = typeParameter.GetConstraintTypes(ConsList<TypeParameterSymbol>.Empty);
            }
        }

        internal abstract ImmutableArray<TypeSymbol> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress);

        internal abstract ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress);

        internal abstract NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress);

        internal abstract TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress);

        private static bool ConstraintImpliesReferenceType(TypeSymbol constraint)
        {
            if (constraint.TypeKind == TypeKind.TypeParameter)
            {
                return IsReferenceTypeFromConstraintTypes(((TypeParameterSymbol)constraint).ConstraintTypesNoUseSiteDiagnostics);
            }
            else if (!constraint.IsReferenceType)
            {
                return false;
            }
            else
            {
                switch (constraint.TypeKind)
                {
                    case TypeKind.Interface:
                        return false; // can be satisfied by value types
                    case TypeKind.Error:
                        return false;
                }

                switch (constraint.SpecialType)
                {
                    case SpecialType.System_Object:
                    case SpecialType.System_ValueType:
                    case SpecialType.System_Enum:
                        return false; // can be satisfied by value types
                }

                return true;
            }
        }

        // From typedesc.cpp :
        // > A recursive helper that helps determine whether this variable is constrained as ObjRef.
        // > Please note that we do not check the gpReferenceTypeConstraint special constraint here
        // > because this property does not propagate up the constraining hierarchy.
        // > (e.g. "class A<S, T> where S : T, where T : class" does not guarantee that S is ObjRef)
        internal static bool IsReferenceTypeFromConstraintTypes(ImmutableArray<TypeSymbol> constraintTypes)
        {
            foreach (var constraintType in constraintTypes)
            {
                if (ConstraintImpliesReferenceType(constraintType))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsValueTypeFromConstraintTypes(ImmutableArray<TypeSymbol> constraintTypes)
        {
            foreach (var constraintType in constraintTypes)
            {
                if (constraintType.IsValueType)
                {
                    return true;
                }
            }
            return false;
        }

        public sealed override bool IsReferenceType
        {
            get
            {
                return this.HasReferenceTypeConstraint || IsReferenceTypeFromConstraintTypes(this.ConstraintTypesNoUseSiteDiagnostics);
            }
        }

        public sealed override bool IsValueType
        {
            get
            {
                return this.HasValueTypeConstraint || IsValueTypeFromConstraintTypes(this.ConstraintTypesNoUseSiteDiagnostics);
            }
        }

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        public abstract bool HasReferenceTypeConstraint { get; }

        public abstract bool HasValueTypeConstraint { get; }

        public abstract VarianceKind Variance { get; }

        internal override bool Equals(TypeSymbol t2, bool ignoreCustomModifiersAndArraySizesAndLowerBounds, bool ignoreDynamic)
        {
            return this.Equals(t2 as TypeParameterSymbol, ignoreCustomModifiersAndArraySizesAndLowerBounds, ignoreDynamic);
        }

        internal bool Equals(TypeParameterSymbol other)
        {
            return Equals(other, false, false);
        }

        private bool Equals(TypeParameterSymbol other, bool ignoreCustomModifiersAndArraySizesAndLowerBounds, bool ignoreDynamic)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if ((object)other == null || !ReferenceEquals(other.OriginalDefinition, this.OriginalDefinition))
            {
                return false;
            }

            // Type parameters may be equal but not reference equal due to independent alpha renamings.
            return other.ContainingSymbol.ContainingType.Equals(this.ContainingSymbol.ContainingType, ignoreCustomModifiersAndArraySizesAndLowerBounds, ignoreDynamic);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ContainingSymbol, Ordinal);
        }

        #region ITypeParameterTypeSymbol Members

        TypeParameterKind ITypeParameterSymbol.TypeParameterKind
        {
            get
            {
                return (TypeParameterKind)this.TypeParameterKind;
            }
        }

        IMethodSymbol ITypeParameterSymbol.DeclaringMethod
        {
            get { return this.DeclaringMethod; }
        }

        INamedTypeSymbol ITypeParameterSymbol.DeclaringType
        {
            get { return this.DeclaringType; }
        }

        ImmutableArray<ITypeSymbol> ITypeParameterSymbol.ConstraintTypes
        {
            get
            {
                return StaticCast<ITypeSymbol>.From(this.ConstraintTypesNoUseSiteDiagnostics);
            }
        }

        ITypeParameterSymbol ITypeParameterSymbol.OriginalDefinition
        {
            get { return this.OriginalDefinition; }
        }

        ITypeParameterSymbol ITypeParameterSymbol.ReducedFrom
        {
            get { return this.ReducedFrom; }
        }
        
        NullableAnnotation ITypeParameterSymbol.ReferenceTypeConstraintNullableAnnotation => NullableAnnotation.None;

        bool ITypeParameterSymbol.HasNotNullConstraint => false;

        ImmutableArray<NullableAnnotation> ITypeParameterSymbol.ConstraintNullableAnnotations => ConstraintTypes.SelectAsArray(c => ((ITypeSymbol)c).NullableAnnotation);


        #endregion

        #region ISymbol Members

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitTypeParameter(this);
        }

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitTypeParameter(this);
        }

        #endregion
    }
}
