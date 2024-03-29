﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.PE
{
    /// <summary>
    /// The class to represent all generic type parameters imported from a PE/module.
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PETypeParameterSymbol : TypeParameterSymbol
    {
        private readonly Symbol _containingSymbol; // Could be PENamedType or a PEMethod
        private readonly GenericParameterHandle _handle;

        #region Metadata
        private readonly string _name;
        private readonly ushort _ordinal; // 0 for first, 1 for second, ...
        private readonly GenericParameterAttributes _flags;
        #endregion

        private TypeParameterBounds _lazyBounds = TypeParameterBounds.Unset;

        ///// <summary>
        ///// First error calculating bounds.
        ///// </summary>
        //private DiagnosticInfo _lazyBoundsErrorInfo = CSDiagnosticInfo.EmptyErrorInfo; // Indicates unknown state.

        private ImmutableArray<AttributeData> _lazyCustomAttributes;

        internal PETypeParameterSymbol(
            PEModuleSymbol moduleSymbol,
            PENamedTypeSymbol definingNamedType,
            ushort ordinal,
            GenericParameterHandle handle)
            : this(moduleSymbol, (Symbol)definingNamedType, ordinal, handle)
        {
        }

        internal PETypeParameterSymbol(
            PEModuleSymbol moduleSymbol,
            PEMethodSymbol definingMethod,
            ushort ordinal,
            GenericParameterHandle handle)
            : this(moduleSymbol, (Symbol)definingMethod, ordinal, handle)
        {
        }

        private PETypeParameterSymbol(
            PEModuleSymbol moduleSymbol,
            Symbol definingSymbol,
            ushort ordinal,
            GenericParameterHandle handle)
        {
            Debug.Assert((object)moduleSymbol != null);
            Debug.Assert((object)definingSymbol != null);
            Debug.Assert(ordinal >= 0);
            Debug.Assert(!handle.IsNil);

            _containingSymbol = definingSymbol;

            GenericParameterAttributes flags = 0;

            try
            {
                moduleSymbol.Module.GetGenericParamPropsOrThrow(handle, out _name, out flags);
            }
            catch (BadImageFormatException)
            {
                if ((object)_name == null)
                {
                    _name = string.Empty;
                }

            }

            // Clear the '.ctor' flag if both '.ctor' and 'valuetype' are
            // set since '.ctor' is redundant in that case.
            _flags = ((flags & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0) ? flags : (flags & ~GenericParameterAttributes.DefaultConstructorConstraint);

            _ordinal = ordinal;
            _handle = handle;
        }

        public override TypeParameterKind TypeParameterKind
        {
            get
            {
                return this.ContainingSymbol.Kind == SymbolKind.Method
                    ? TypeParameterKind.Method
                    : TypeParameterKind.Type;
            }
        }

        public override int Ordinal
        {
            get { return _ordinal; }
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        internal GenericParameterHandle Handle
        {
            get
            {
                return _handle;
            }
        }

        public override Symbol ContainingSymbol
        {
            get { return _containingSymbol; }
        }

        public override AssemblySymbol ContainingAssembly
        {
            get
            {
                return _containingSymbol.ContainingAssembly;
            }
        }

        private ImmutableArray<TypeSymbol> GetDeclaredConstraintTypes()
        {
            PEMethodSymbol containingMethod = null;
            PENamedTypeSymbol containingType;

            if (_containingSymbol.Kind == SymbolKind.Method)
            {
                containingMethod = (PEMethodSymbol)_containingSymbol;
                containingType = (PENamedTypeSymbol)containingMethod.ContainingSymbol;
            }
            else
            {
                containingType = (PENamedTypeSymbol)_containingSymbol;
            }

            var moduleSymbol = containingType.ContainingPEModule;
            var metadataReader = moduleSymbol.Module.MetadataReader;
            GenericParameterConstraintHandleCollection constraints;

            try
            {
                constraints = metadataReader.GetGenericParameter(_handle).GetConstraints();
            }
            catch (BadImageFormatException)
            {
                constraints = default(GenericParameterConstraintHandleCollection);
            }

            if (constraints.Count > 0)
            {
                var symbolsBuilder = ArrayBuilder<TypeSymbol>.GetInstance();
                MetadataDecoder tokenDecoder;

                if ((object)containingMethod != null)
                {
                    tokenDecoder = new MetadataDecoder(moduleSymbol, containingMethod);
                }
                else
                {
                    tokenDecoder = new MetadataDecoder(moduleSymbol, containingType);
                }

                foreach (var constraintHandle in constraints)
                {
                    var constraint = metadataReader.GetGenericParameterConstraint(constraintHandle);
                    var typeSymbol = tokenDecoder.DecodeGenericParameterConstraint(constraint.Type, out _);

                    // Drop 'System.Object' constraint type.
                    if (typeSymbol.SpecialType == Microsoft.CodeAnalysis.SpecialType.System_Object)
                    {
                        continue;
                    }

                    // Drop 'System.ValueType' constraint type if the 'valuetype' constraint was also specified.
                    if (((_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0) &&
                        (typeSymbol.SpecialType == Microsoft.CodeAnalysis.SpecialType.System_ValueType))
                    {
                        continue;
                    }

                    symbolsBuilder.Add(typeSymbol);
                }

                return symbolsBuilder.ToImmutableAndFree();
            }
            else
            {
                return ImmutableArray<TypeSymbol>.Empty;
            }
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return _containingSymbol.Locations;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override bool HasConstructorConstraint
        {
            get
            {
                return (_flags & GenericParameterAttributes.DefaultConstructorConstraint) != 0;
            }
        }

        public override bool HasReferenceTypeConstraint
        {
            get
            {
                return (_flags & GenericParameterAttributes.ReferenceTypeConstraint) != 0;
            }
        }

        public override bool HasValueTypeConstraint
        {
            get
            {
                return (_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;
            }
        }

        public override VarianceKind Variance
        {
            get
            {
                return (VarianceKind)(_flags & GenericParameterAttributes.VarianceMask);
            }
        }

        internal override void EnsureAllConstraintsAreResolved()
        {
            if (ReferenceEquals(_lazyBounds, TypeParameterBounds.Unset))
            {
                var typeParameters = (_containingSymbol.Kind == SymbolKind.Method) ?
                    ((PEMethodSymbol)_containingSymbol).TypeParameters :
                    ((PENamedTypeSymbol)_containingSymbol).TypeParameters;
                EnsureAllConstraintsAreResolved(typeParameters);
            }
        }

        internal override ImmutableArray<TypeSymbol> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
        {
            var bounds = this.GetBounds(inProgress);
            return (bounds != null) ? bounds.ConstraintTypes : ImmutableArray<TypeSymbol>.Empty;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
        {
            var bounds = this.GetBounds(inProgress);
            return (bounds != null) ? bounds.Interfaces : ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
        {
            var bounds = this.GetBounds(inProgress);
            return (bounds != null) ? bounds.EffectiveBaseClass : this.GetDefaultBaseType();
        }

        internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
        {
            var bounds = this.GetBounds(inProgress);
            return (bounds != null) ? bounds.DeducedBaseType : this.GetDefaultBaseType();
        }

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            if (_lazyCustomAttributes.IsDefault)
            {
                var containingPEModuleSymbol = (PEModuleSymbol)this.ContainingModule;
                containingPEModuleSymbol.LoadCustomAttributes(this.Handle, ref _lazyCustomAttributes);
            }
            return _lazyCustomAttributes;
        }

        private TypeParameterBounds GetBounds(ConsList<TypeParameterSymbol> inProgress)
        {
            Debug.Assert(!inProgress.ContainsReference(this));
            Debug.Assert(!inProgress.Any() || ReferenceEquals(inProgress.Head.ContainingSymbol, this.ContainingSymbol));

            if (ReferenceEquals(_lazyBounds, TypeParameterBounds.Unset))
            {
                var constraintTypes = GetDeclaredConstraintTypes();
                Debug.Assert(!constraintTypes.IsDefault);

                bool inherited = (_containingSymbol.Kind == SymbolKind.Method) && ((MethodSymbol)_containingSymbol).IsOverride;
                var bounds = new TypeParameterBounds(ImmutableArray<TypeSymbol>.Empty, ImmutableArray<NamedTypeSymbol>.Empty, null, null);
                
                Interlocked.CompareExchange(ref _lazyBounds, bounds, TypeParameterBounds.Unset);
            }

            return _lazyBounds;
        }

        private NamedTypeSymbol GetDefaultBaseType()
        {
            return this.ContainingAssembly.GetSpecialType(SpecialType.System_Object);
        }

        internal sealed override AquilaCompilation DeclaringCompilation // perf, not correctness
        {
            get { return null; }
        }
    }
}
