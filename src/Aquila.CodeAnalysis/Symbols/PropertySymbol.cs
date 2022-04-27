﻿using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Represents a property or indexer.
    /// </summary>
    internal abstract partial class PropertySymbol : Symbol, IPropertySymbol
    {
        /// <summary>
        /// As a performance optimization, cache parameter types and refkinds - overload resolution uses them a lot.
        /// </summary>
        private ParameterSignature _lazyParameterSignature;

        internal PropertySymbol()
        {
        }

        /// <summary>
        /// The original definition of this symbol. If this symbol is constructed from another
        /// symbol by type substitution then OriginalDefinition gets the original symbol as it was defined in
        /// source or metadata.
        /// </summary>
        public new virtual PropertySymbol OriginalDefinition
        {
            get { return this; }
        }

        protected override sealed Symbol OriginalSymbolDefinition
        {
            get { return this.OriginalDefinition; }
        }

        /// <summary>
        /// The type of the property. 
        /// </summary>
        public abstract TypeSymbol Type { get; }

        /// <summary>
        /// The list of custom modifiers, if any, associated with the type of the property. 
        /// </summary>
        public abstract ImmutableArray<CustomModifier> TypeCustomModifiers { get; }

        /// <summary>
        /// The parameters of this property. If this property has no parameters, returns
        /// an empty list. Parameters are only present on indexers, or on some properties
        /// imported from a COM interface.
        /// </summary>
        public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

        /// <summary>
        /// Optimization: in many cases, the parameter count (fast) is sufficient and we
        /// don't need the actual parameter symbols (slow).
        /// </summary>
        internal int ParameterCount
        {
            get { return this.Parameters.Length; }
        }

        internal ImmutableArray<TypeSymbol> ParameterTypes
        {
            get
            {
                ParameterSignature.PopulateParameterSignature(this.Parameters, ref _lazyParameterSignature);
                return _lazyParameterSignature.parameterTypes;
            }
        }

        internal ImmutableArray<RefKind> ParameterRefKinds
        {
            get
            {
                ParameterSignature.PopulateParameterSignature(this.Parameters, ref _lazyParameterSignature);
                return _lazyParameterSignature.parameterRefKinds;
            }
        }

        /// <summary>
        /// Returns whether the property is really an indexer.
        /// </summary>
        /// <remarks>
        /// In source, we regard a property as an indexer if it is declared with an IndexerDeclarationSyntax.
        /// From metadata, we regard a property if it has parameters and is a default member of the containing
        /// type.
        /// CAVEAT: To ensure that this property (and indexer Names) roundtrip, source properties are not
        /// indexers if they are explicit interface implementations (since they will not be marked as default
        /// members in metadata).
        /// </remarks>
        public abstract bool IsIndexer { get; }

        /// <summary>
        /// True if this an indexed property; that is, a property with parameters
        /// within a [ComImport] type.
        /// </summary>
        public virtual bool IsIndexedProperty
        {
            get { return false; }
        }

        /// <summary>
        /// True if this is a read-only property; that is, a property with no set accessor.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                var property = this.GetLeastOverriddenProperty((NamedTypeSymbol)this.ContainingType);
                return (object)property.SetMethod == null;
            }
        }

        /// <summary>
        /// True if this is a write-only property; that is, a property with no get accessor.
        /// </summary>
        public bool IsWriteOnly
        {
            get
            {
                var property = this.GetLeastOverriddenProperty((NamedTypeSymbol)this.ContainingType);
                return (object)property.GetMethod == null;
            }
        }

        /// <summary>
        /// True if this symbol has a special name (metadata flag SpecialName is set).
        /// </summary>
        internal abstract bool HasSpecialName { get; }

        /// <summary>
        /// The 'get' accessor of the property, or null if the property is write-only.
        /// </summary>
        public abstract MethodSymbol GetMethod { get; }

        /// <summary>
        /// The 'set' accessor of the property, or null if the property is read-only.
        /// </summary>
        public abstract MethodSymbol SetMethod { get; }

        internal abstract Cci.CallingConvention CallingConvention { get; }

        internal abstract bool MustCallMethodsDirectly { get; }

        /// <summary>
        /// Returns the overridden property, or null.
        /// </summary>
        public PropertySymbol OverriddenProperty
        {
            get
            {
                if (this.IsOverride)
                {
                    if (IsDefinition)
                    {
                        //return (PropertySymbol)OverriddenOrHiddenMembers.GetOverriddenMember();
                        return this.ResolveOverridenMember();
                    }

                    return (PropertySymbol)OverriddenOrHiddenMembersResult.GetOverriddenMember(this,
                        OriginalDefinition.OverriddenProperty);
                }

                return null;
            }
        }

        //internal virtual OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers
        //{
        //    get
        //    {
        //        return this.MakeOverriddenOrHiddenMembers();
        //    }
        //}

        internal bool HidesBasePropertiesByName
        {
            get
            {
                // Dev10 gives preference to the getter.
                MethodSymbol accessor = GetMethod ?? SetMethod;

                // false is a reasonable default if there are no accessors (e.g. not done typing).
                return (object)accessor != null && accessor.HidesBaseMethodsByName;
            }
        }

        internal PropertySymbol GetLeastOverriddenProperty(NamedTypeSymbol accessingTypeOpt)
        {
            var accessingType = ((object)accessingTypeOpt == null ? this.ContainingType : accessingTypeOpt)
                .OriginalDefinition;

            PropertySymbol p = this;
            while (p.IsOverride && !p.HidesBasePropertiesByName)
            {
                // We might not be able to access the overridden method. For example,
                // 
                //   .assembly A
                //   {
                //      InternalsVisibleTo("B")
                //      public class A { internal virtual int P { get; } }
                //   }
                // 
                //   .assembly B
                //   {
                //      InternalsVisibleTo("C")
                //      public class B : A { internal override int P { get; } }
                //   }
                // 
                //   .assembly C
                //   {
                //      public class C : B { ... new B().P ... }       // A.P is not accessible from here
                //   }
                //
                // See InternalsVisibleToAndStrongNameTests: IvtVirtualCall1, IvtVirtualCall2, IvtVirtual_ParamsAndDynamic.
                PropertySymbol overridden = p.OverriddenProperty;
                //HashSet<DiagnosticInfo> useSiteDiagnostics = null;
                if ((object)overridden ==
                    null) // || !AccessCheck.IsSymbolAccessible(overridden, accessingType, ref useSiteDiagnostics))
                {
                    break;
                }

                p = overridden;
            }

            return p;
        }

        /// <summary>
        /// Source: Was the member name qualified with a type name?
        /// Metadata: Is the member an explicit implementation?
        /// </summary>
        /// <remarks>
        /// Will not always agree with ExplicitInterfaceImplementations.Any()
        /// (e.g. if binding of the type part of the name fails).
        /// </remarks>
        internal virtual bool IsExplicitInterfaceImplementation
        {
            get { return ExplicitInterfaceImplementations.Any(); }
        }

        /// <summary>
        /// Returns interface properties explicitly implemented by this property.
        /// </summary>
        /// <remarks>
        /// Properties imported from metadata can explicitly implement more than one property.
        /// </remarks>
        public abstract ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations { get; }

        /// <summary>
        /// Gets the kind of this symbol.
        /// </summary>
        public sealed override SymbolKind Kind
        {
            get { return SymbolKind.Property; }
        }

        /// <summary>
        /// Implements visitor pattern.
        /// </summary>
        internal override TResult Accept<TArgument, TResult>(AquilaSymbolVisitor<TArgument, TResult> visitor,
            TArgument argument)
        {
            return visitor.VisitProperty(this, argument);
        }

        public override void Accept(AquilaSymbolVisitor visitor)
        {
            visitor.VisitProperty(this);
        }

        public override TResult Accept<TResult>(AquilaSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitProperty(this);
        }

        public bool HasRefOrOutParameter()
        {
            foreach (ParameterSymbol param in this.Parameters)
            {
                if (param.RefKind != RefKind.None)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool ReturnsByRef => false;

        public virtual bool ReturnsByRefReadonly => false;

        public virtual RefKind RefKind => RefKind.None;

        internal virtual PropertySymbol AsMember(NamedTypeSymbol newOwner)
        {
            Debug.Assert(this.IsDefinition);
            Debug.Assert(ReferenceEquals(newOwner.OriginalDefinition, this.ContainingSymbol.OriginalDefinition));
            return (newOwner == this.ContainingSymbol)
                ? this
                : new SubstitutedPropertySymbol(newOwner as SubstitutedNamedTypeSymbol, this);
        }

        #region IPropertySymbol Members

        bool IPropertySymbol.IsIndexer
        {
            get { return this.IsIndexer; }
        }

        ITypeSymbol IPropertySymbol.Type
        {
            get { return this.Type; }
        }

        ImmutableArray<IParameterSymbol> IPropertySymbol.Parameters
        {
            get { return StaticCast<IParameterSymbol>.From(this.Parameters); }
        }

        IMethodSymbol IPropertySymbol.GetMethod
        {
            get { return this.GetMethod; }
        }

        IMethodSymbol IPropertySymbol.SetMethod
        {
            get { return this.SetMethod; }
        }

        IPropertySymbol IPropertySymbol.OriginalDefinition
        {
            get { return this.OriginalDefinition; }
        }

        IPropertySymbol IPropertySymbol.OverriddenProperty
        {
            get { return this.OverriddenProperty; }
        }

        ImmutableArray<IPropertySymbol> IPropertySymbol.ExplicitInterfaceImplementations
        {
            get { return this.ExplicitInterfaceImplementations.Cast<PropertySymbol, IPropertySymbol>(); }
        }

        bool IPropertySymbol.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        bool IPropertySymbol.IsWriteOnly
        {
            get { return this.IsWriteOnly; }
        }

        bool IPropertySymbol.IsWithEvents
        {
            get { return false; }
        }

        ImmutableArray<CustomModifier> IPropertySymbol.TypeCustomModifiers
        {
            get { return this.TypeCustomModifiers; }
        }

        ImmutableArray<CustomModifier> IPropertySymbol.RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        Microsoft.CodeAnalysis.NullableAnnotation IPropertySymbol.NullableAnnotation =>
            Microsoft.CodeAnalysis.NullableAnnotation.None;

        #endregion

        #region ISymbol Members

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitProperty(this);
        }

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitProperty(this);
        }

        #endregion

        #region Equality

        public override bool Equals(ISymbol other, Microsoft.CodeAnalysis.SymbolEqualityComparer equalityComparer)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // This checks if the property have the same definition and the type parameters on the containing types have been
            // substituted in the same way.
            return other is PropertySymbol p && this.ContainingType == p.ContainingType &&
                   ReferenceEquals(this.OriginalDefinition, p.OriginalDefinition);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = Hash.Combine(this.ContainingType, hash);
            hash = Hash.Combine(this.Name, hash);
            hash = Hash.Combine(hash, this.ParameterCount);
            return hash;
        }

        #endregion Equality
    }

    internal class MissingPropertySymbol : PropertySymbol
    {
        public string Name { get; }

        public MissingPropertySymbol(string name)
        {
            Name = name;
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData { get; }
        public override Symbol ContainingSymbol { get; }
        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }
        public override Accessibility DeclaredAccessibility { get; }
        public override bool IsStatic { get; }
        public override bool IsVirtual { get; }
        public override bool IsOverride { get; }
        public override bool IsAbstract { get; }
        public override bool IsSealed { get; }
        public override bool IsExtern { get; }
        public override TypeSymbol Type { get; }
        public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations { get; }
        public override ImmutableArray<CustomModifier> TypeCustomModifiers { get; }
        public override ImmutableArray<ParameterSymbol> Parameters { get; }
        public override MethodSymbol GetMethod { get; }
        public override MethodSymbol SetMethod { get; }
        internal override Cci.CallingConvention CallingConvention { get; }
        internal override bool MustCallMethodsDirectly { get; }
        public override bool IsIndexer { get; }
        internal override bool HasSpecialName { get; }
    }
}