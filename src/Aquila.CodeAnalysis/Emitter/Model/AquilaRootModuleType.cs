﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Roslyn.Utilities;
using EmitContext = Microsoft.CodeAnalysis.Emit.EmitContext;
using TypeDefinitionHandle = System.Reflection.Metadata.TypeDefinitionHandle;
using Aquila.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis.Emit
{
    /// <summary>
    /// Special type &lt;Module&gt;.
    /// See https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/PEWriter/RootModuleType.cs for original implementation.
    /// </summary>
    internal class AquilaRootModuleType : INamespaceTypeDefinition
    {
        public TypeDefinitionHandle TypeDef
        {
            get { return default(TypeDefinitionHandle); }
        }

        public ITypeDefinition ResolvedType
        {
            get { return this; }
        }

        public IEnumerable<ICustomAttribute> GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        public bool MangleName
        {
            get { return false; }
        }

        public string Name
        {
            get { return "<Module>"; }
        }

        public ushort Alignment
        {
            get { return 0; }
        }

        public ITypeReference? GetBaseClass(EmitContext context)
        {
            return null;
        }

        public IEnumerable<IEventDefinition> GetEvents(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<IEventDefinition>();
        }

        public IEnumerable<MethodImplementation> GetExplicitImplementationOverrides(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<MethodImplementation>();
        }

        public IEnumerable<IFieldDefinition> GetFields(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<IFieldDefinition>();
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        public IEnumerable<TypeReferenceWithAttributes> Interfaces(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<TypeReferenceWithAttributes>();
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsWindowsRuntimeImport
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public IEnumerable<IMethodDefinition> GetMethods(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).GetSynthesizedMethods(this);
        }

        public IEnumerable<INestedTypeDefinition> GetNestedTypes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<INestedTypeDefinition>();
        }

        public IEnumerable<IPropertyDefinition> GetProperties(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<IPropertyDefinition>();
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public CharSet StringFormat
        {
            get { return CharSet.Ansi; }
        }

        public bool IsPublic
        {
            get { return false; }
        }

        public bool IsNested
        {
            get { return false; }
        }

        IEnumerable<IGenericTypeParameter> ITypeDefinition.GenericParameters
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        ushort ITypeDefinition.GenericParameterCount
        {
            get { return 0; }
        }

        IEnumerable<SecurityAttribute> ITypeDefinition.SecurityAttributes
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        bool ITypeReference.IsEnum
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        bool ITypeReference.IsValueType
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
        {
            return this;
        }

        PrimitiveTypeCode ITypeReference.TypeCode
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        ushort INamedTypeReference.GenericParameterCount
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        IUnitReference INamespaceTypeReference.GetUnit(EmitContext context)
        {
            throw ExceptionUtilities.Unreachable;
        }

        string INamespaceTypeReference.NamespaceName
        {
            get { return string.Empty; }
        }

        IGenericMethodParameterReference? ITypeReference.AsGenericMethodParameterReference
        {
            get { return null; }
        }

        IGenericTypeInstanceReference? ITypeReference.AsGenericTypeInstanceReference
        {
            get { return null; }
        }

        IGenericTypeParameterReference? ITypeReference.AsGenericTypeParameterReference
        {
            get { return null; }
        }

        INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
        {
            return this;
        }

        INamespaceTypeReference ITypeReference.AsNamespaceTypeReference
        {
            get { return this; }
        }

        INestedTypeDefinition? ITypeReference.AsNestedTypeDefinition(EmitContext context)
        {
            return null;
        }

        INestedTypeReference? ITypeReference.AsNestedTypeReference
        {
            get { return null; }
        }

        ISpecializedNestedTypeReference? ITypeReference.AsSpecializedNestedTypeReference
        {
            get { return null; }
        }

        ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
        {
            return this;
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return this;
        }

        public ISymbolInternal? GetInternalSymbol()
        {
            throw new NotImplementedException();
        }
    }
}