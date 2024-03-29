﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols.PE
{
    internal sealed class SymbolFactory : SymbolFactory<PEModuleSymbol, TypeSymbol>
    {
        internal static readonly SymbolFactory Instance = new SymbolFactory();

        internal override TypeSymbol GetMDArrayTypeSymbol(PEModuleSymbol moduleSymbol, int rank, TypeSymbol elementType,
            ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers,
            ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
        {
            if (elementType is UnsupportedMetadataTypeSymbol)
            {
                return elementType;
            }

            return ArrayTypeSymbol.CreateMDArray(moduleSymbol.ContainingAssembly, elementType, rank, sizes, lowerBounds,
                CSharpCustomModifier.Convert(customModifiers));
        }

        internal override TypeSymbol GetSpecialType(PEModuleSymbol moduleSymbol, SpecialType specialType)
        {
            return moduleSymbol.ContainingAssembly.GetSpecialType(specialType);
        }

        internal override TypeSymbol GetSystemTypeSymbol(PEModuleSymbol moduleSymbol)
        {
            return moduleSymbol.SystemTypeSymbol;
        }

        internal override TypeSymbol MakePointerTypeSymbol(PEModuleSymbol moduleSymbol, TypeSymbol type,
            ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            if (type is UnsupportedMetadataTypeSymbol)
            {
                return type;
            }

            return new PointerTypeSymbol(type, CSharpCustomModifier.Convert(customModifiers));
        }

        internal override TypeSymbol MakeFunctionPointerTypeSymbol(Cci.CallingConvention callingConvention,
            ImmutableArray<ParamInfo<TypeSymbol>> returnAndParamTypes)
        {
            throw new NotImplementedException();
        }

        internal override TypeSymbol GetEnumUnderlyingType(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            return type.GetEnumUnderlyingType();
        }

        internal override Cci.PrimitiveTypeCode GetPrimitiveTypeCode(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            return type.PrimitiveTypeCode;
        }

        internal override TypeSymbol GetSZArrayTypeSymbol(PEModuleSymbol moduleSymbol, TypeSymbol elementType,
            ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            if (elementType is UnsupportedMetadataTypeSymbol)
            {
                return elementType;
            }

            return ArrayTypeSymbol.CreateSZArray(moduleSymbol.ContainingAssembly, elementType,
                CSharpCustomModifier.Convert(customModifiers));
        }

        internal override TypeSymbol GetUnsupportedMetadataTypeSymbol(PEModuleSymbol moduleSymbol,
            BadImageFormatException exception)
        {
            return new UnsupportedMetadataTypeSymbol(exception);
        }

        internal override TypeSymbol SubstituteTypeParameters(
            PEModuleSymbol moduleSymbol,
            TypeSymbol genericTypeDef,
            ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments,
            ImmutableArray<bool> refersToNoPiaLocalType)
        {
            if (genericTypeDef is UnsupportedMetadataTypeSymbol)
            {
                return genericTypeDef;
            }

            // Let's return unsupported metadata type if any argument is unsupported metadata type 
            foreach (var arg in arguments)
            {
                if (arg.Key.Kind == SymbolKind.ErrorType &&
                    arg.Key is UnsupportedMetadataTypeSymbol)
                {
                    return new UnsupportedMetadataTypeSymbol();
                }
            }

            NamedTypeSymbol genericType = (NamedTypeSymbol) genericTypeDef;

            // Collect generic parameters for the type and its containers in the order
            // that matches passed in arguments, i.e. sorted by the nesting.
            ImmutableArray<TypeParameterSymbol> typeParameters = genericType.GetAllTypeParameters();
            Debug.Assert(typeParameters.Length > 0);

            if (typeParameters.Length != arguments.Length)
            {
                return new UnsupportedMetadataTypeSymbol();
            }

            TypeMap substitution = new TypeMap(typeParameters,
                arguments.SelectAsArray(arg =>
                    new TypeWithModifiers(arg.Key, CSharpCustomModifier.Convert(arg.Value))));

            NamedTypeSymbol constructedType = substitution.SubstituteNamedType(genericType);

            return constructedType;
        }

        internal override TypeSymbol MakeUnboundIfGeneric(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            var namedType = type as NamedTypeSymbol;
            return ((object) namedType != null && namedType.IsGenericType) ? namedType.AsUnboundGenericType() : type;
        }
    }
}