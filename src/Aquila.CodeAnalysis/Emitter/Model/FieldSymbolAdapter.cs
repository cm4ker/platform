﻿using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols
{
    internal partial class FieldSymbol :
        Cci.IFieldReference,
        Cci.IFieldDefinition,
        Cci.ITypeMemberReference,
        Cci.ITypeDefinitionMember,
        Cci.ISpecializedFieldReference
    {
        Cci.ITypeReference Cci.IFieldReference.GetType(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;

            var customModifiers = this.CustomModifiers;
            var isFixed = this.IsFixed;
            var implType = isFixed ? this.FixedImplementationType(moduleBeingBuilt) : this.Type;
            var type = moduleBeingBuilt.Translate(implType, context.SyntaxNode, context.Diagnostics);

            if (isFixed || customModifiers.Length == 0)
            {
                return type;
            }
            else
            {
                return new Cci.ModifiedTypeReference(type, customModifiers.As<Cci.ICustomModifier>());
            }
        }

        Cci.IFieldDefinition Cci.IFieldReference.GetResolvedField(EmitContext context)
        {
            return ResolvedFieldImpl((PEModuleBuilder)context.Module);
        }

        private Cci.IFieldDefinition ResolvedFieldImpl(PEModuleBuilder moduleBeingBuilt)
        {
            Debug.Assert(this.IsDefinitionOrDistinct());

            if (this.IsDefinition &&
                object.ReferenceEquals(this.ContainingModule, moduleBeingBuilt.SourceModule))
            {
                return this;
            }

            return null;
        }

        Cci.ISpecializedFieldReference Cci.IFieldReference.AsSpecializedFieldReference
        {
            get
            {
                Debug.Assert(this.IsDefinitionOrDistinct());

                if (!this.IsDefinition)
                {
                    return this;
                }

                return null;
            }
        }

        Cci.ITypeReference Cci.ITypeMemberReference.GetContainingType(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;

            Debug.Assert(this.IsDefinitionOrDistinct());

            if (!this.IsDefinition)
            {
                return moduleBeingBuilt.Translate(this.ContainingType, context.SyntaxNode, context.Diagnostics);
            }

            return (Cci.ITypeReference)this.ContainingType;
        }

        void Cci.IReference.Dispatch(Cci.MetadataVisitor visitor)
        {
            Debug.Assert(this.IsDefinitionOrDistinct());

            if (!this.IsDefinition)
            {
                visitor.Visit((Cci.ISpecializedFieldReference)this);
            }
            else if (object.ReferenceEquals(this.ContainingModule,
                ((PEModuleBuilder)visitor.Context.Module).SourceModule))
            {
                visitor.Visit((Cci.IFieldDefinition)this);
            }
            else
            {
                visitor.Visit((Cci.IFieldReference)this);
            }
        }

        Cci.IDefinition Cci.IReference.AsDefinition(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;

            return ResolvedFieldImpl(moduleBeingBuilt);
        }

        string Cci.INamedEntity.Name
        {
            get { return this.MetadataName; }
        }

        bool Cci.IFieldReference.IsContextualNamedEntity
        {
            get { return false; }
        }

        MetadataConstant Cci.IFieldDefinition.GetCompileTimeValue(EmitContext context)
        {
            CheckDefinitionInvariant();

            return GetMetadataConstantValue(context);
        }

        internal MetadataConstant GetMetadataConstantValue(EmitContext context)
        {
            // A constant field of type decimal is not treated as a compile time value in CLR,
            // so check if it is a metadata constant, not just a constant to exclude decimals.
            if (this.IsMetadataConstant)
            {
                // NOTE: We would like to be able to assert that the constant value of this field
                // is not bad (i.e. ConstantValue.Bad) if it is being consumed by CCI, but we can't
                // because this method is called by the ReferenceIndexer in the metadata-only case
                // (and we specifically don't want to prevent metadata-only emit because of a bad
                // constant).  If the constant value is bad, we'll end up exposing null to CCI.
                return ((PEModuleBuilder)context.Module).CreateConstant(this.Type, this.ConstantValue,
                    syntaxNodeOpt: context.SyntaxNode,
                    diagnostics: context.Diagnostics);
            }

            return null;
        }

        ImmutableArray<byte> Cci.IFieldDefinition.MappedData
        {
            get
            {
                CheckDefinitionInvariant();
                return default(ImmutableArray<byte>);
            }
        }

        bool Cci.IFieldDefinition.IsCompileTimeConstant
        {
            get
            {
                CheckDefinitionInvariant();
                // A constant field of type decimal is not treated as a compile time value in CLR,
                // so check if it is a metadata constant, not just a constant to exclude decimals.
                return this.IsMetadataConstant;
            }
        }

        bool Cci.IFieldDefinition.IsNotSerialized
        {
            get
            {
                CheckDefinitionInvariant();
                return this.IsNotSerialized;
            }
        }

        bool Cci.IFieldDefinition.IsReadOnly
        {
            get
            {
                CheckDefinitionInvariant();
                return this.IsReadOnly || (this.IsConst && !this.IsMetadataConstant);
            }
        }

        bool Cci.IFieldDefinition.IsRuntimeSpecial
        {
            get
            {
                CheckDefinitionInvariant();
                return this.HasRuntimeSpecialName;
            }
        }

        bool Cci.IFieldDefinition.IsSpecialName
        {
            get
            {
                CheckDefinitionInvariant();
                return this.HasSpecialName;
            }
        }

        bool Cci.IFieldDefinition.IsStatic
        {
            get
            {
                CheckDefinitionInvariant();
                return this.IsStatic;
            }
        }

        bool Cci.IFieldDefinition.IsMarshalledExplicitly
        {
            get
            {
                CheckDefinitionInvariant();
                return this.IsMarshalledExplicitly;
            }
        }

        internal virtual bool IsMarshalledExplicitly
        {
            get
            {
                CheckDefinitionInvariant();
                return this.MarshallingInformation != null;
            }
        }

        Cci.IMarshallingInformation Cci.IFieldDefinition.MarshallingInformation
        {
            get
            {
                CheckDefinitionInvariant();
                return this.MarshallingInformation;
            }
        }

        ImmutableArray<byte> Cci.IFieldDefinition.MarshallingDescriptor
        {
            get
            {
                CheckDefinitionInvariant();
                return this.MarshallingDescriptor;
            }
        }

        internal virtual ImmutableArray<byte> MarshallingDescriptor
        {
            get
            {
                CheckDefinitionInvariant();
                return default(ImmutableArray<byte>);
            }
        }

        int Cci.IFieldDefinition.Offset
        {
            get
            {
                CheckDefinitionInvariant();
                var offset = this.TypeLayoutOffset;
                return offset ?? 0;
            }
        }

        Cci.ITypeDefinition Cci.ITypeDefinitionMember.ContainingTypeDefinition
        {
            get
            {
                CheckDefinitionInvariant();
                return (Cci.ITypeDefinition)this.ContainingType;
            }
        }

        Cci.TypeMemberVisibility Cci.ITypeDefinitionMember.Visibility
        {
            get
            {
                CheckDefinitionInvariant();
                return PEModuleBuilder.MemberVisibility(this);
            }
        }

        Cci.IFieldReference Cci.ISpecializedFieldReference.UnspecializedVersion
        {
            get
            {
                Debug.Assert(!this.IsDefinition);
                return this.OriginalDefinition;
            }
        }
    }
}