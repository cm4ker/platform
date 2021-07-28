﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    #region BoundPrimitiveTypeRef

    [DebuggerDisplay("BoundPrimitiveTypeRef ({_type})")]
    public sealed partial class BoundPrimitiveTypeRef : BoundTypeRef
    {
        public AquilaTypeCode TypeCode => _type;

        partial void OnCreateImpl(AquilaTypeCode type)
        {
            IsNullable = type == AquilaTypeCode.Null || type == AquilaTypeCode.Mixed;
        }

        /// <summary>
        /// Gets value indicating the type is <c>long</c> or <c>double</c>.
        /// </summary>
        public bool IsNumber => _type == AquilaTypeCode.Long || _type == AquilaTypeCode.Double;

        public override bool IsObject => _type == AquilaTypeCode.Object;

        public override bool IsArray => _type == AquilaTypeCode.AquilaArray;

        public override bool IsPrimitiveType => _type != AquilaTypeCode.Object;

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw new NotSupportedException();
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            var ct = compilation.CoreTypes;

            switch (_type)
            {
                case AquilaTypeCode.Void: return ct.Void.Symbol;
                case AquilaTypeCode.Boolean: return ct.Boolean.Symbol;
                case AquilaTypeCode.Long: return ct.Int64.Symbol;
                case AquilaTypeCode.Int: return ct.Int32.Symbol;
                case AquilaTypeCode.Double: return ct.Double.Symbol;
                case AquilaTypeCode.String: return ct.String.Symbol;

                default:
                    throw ExceptionUtilities.UnexpectedValue(_type);
            }
        }


        public override string ToString()
        {
            switch (_type)
            {
                case AquilaTypeCode.Void: return "void"; // report "void" instead of "undefined"
                case AquilaTypeCode.Long: return "integer";
                case AquilaTypeCode.String:
                case AquilaTypeCode.WritableString: return "string";
                case AquilaTypeCode.AquilaArray: return "array";
                default:
                    return _type.ToString().ToLowerInvariant();
            }
        }

        public override bool Equals(IBoundTypeRef other) =>
            base.Equals(other) || (other is BoundPrimitiveTypeRef pt && pt._type == this._type);
    }

    #endregion


    #region BoundArrayTypeRef

    [DebuggerDisplay("BoundArrayTypeRef ({_elementType})")]
    public sealed partial class BoundArrayTypeRef : BoundTypeRef
    {
        public override bool IsArray => true;

        public override bool IsPrimitiveType => true;

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw new NotSupportedException();
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => AquilaTypeCode.AquilaArray.ToString().ToLowerInvariant();

        // public override IBoundTypeRef Transfer(TypeRefContext source, TypeRefContext target)
        // {
        //     Contract.ThrowIfNull(source);
        //     Contract.ThrowIfNull(target);
        //
        //     if (source == target || _elementType.IsVoid || _elementType.IsAnyType)
        //         return this;
        //
        //     return null;
        //     // note: there should be no circular dependency
        //     // return new BoundArrayTypeRef(target.AddToContext(source, _elementType));
        // }

        public override bool Equals(IBoundTypeRef other) => base.Equals(other) ||
                                                            (other is BoundArrayTypeRef at);
    }

    #endregion

    #region BoundLambdaTypeRef

    [DebuggerDisplay("BoundLambdaTypeRef ({_returnType})")]
    sealed class BoundLambdaTypeRef : BoundTypeRef
    {
        // readonly TypeRefMask _returnType;

        // TODO: signature

        // public BoundLambdaTypeRef(TypeRefMask returnType)
        // {
        //     _returnType = returnType;
        // }

        public override bool IsObject => true; // Closure

        public override bool IsLambda => true;

        // public override TypeRefMask LambdaReturnType => _returnType;

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            throw new NotImplementedException();
            //return compilation.CoreTypes.Closure.Symbol;
        }

        // public override IBoundTypeRef Transfer(TypeRefContext source, TypeRefContext target)
        // {
        //     if (source == target || _returnType.IsVoid || _returnType.IsAnyType)
        //         return this;
        //
        //     // note: there should be no circular dependency
        //     return new BoundLambdaTypeRef(target.AddToContext(source, _returnType) /*, _signature*/);
        // }

        // public override bool Equals(IBoundTypeRef other) => base.Equals(other) ||
        //                                                     (other is BoundLambdaTypeRef lt &&
        //                                                      lt._returnType == this._returnType);

        public override string ToString() => NameUtils.SpecialNames.Closure.ToString();
        public override BoundKind BoundKind { get; }
    }

    #endregion

    #region BoundClassTypeRef

    [DebuggerDisplay("BoundClassTypeRef ({ToString(),nq})")]
    public sealed partial class BoundClassTypeRef
    {
        public QualifiedName ClassName { get; }


        partial void OnCreateImpl(QualifiedName qName, SourceMethodSymbol method, int arity)
        {
            if (qName.IsReservedClassName)
            {
                throw new ArgumentException();
            }
        }

        public override bool IsObject => true;

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw new NotImplementedException();
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            if (ResolvedType.IsValidType() && !ResolvedType.IsUnreachable)
            {
                return ResolvedType;
            }

            TypeSymbol type = null;


            if (type == null)
            {
                type = (_arity <= 0)
                    ? (TypeSymbol)compilation.GlobalSemantics.ResolveType(ClassName)
                    // generic types only exist in external references, use this method to resolve the symbol including arity (needs metadataname instead of QualifiedName)
                    : compilation.GlobalSemantics.GetTypeFromNonExtensionAssemblies(
                        MetadataHelpers.ComposeAritySuffixedMetadataName(ClassName.ClrName(), _arity));
            }

            var containingFile = _method?.Syntax.SyntaxTree; //?? _self?.ContainingFile;

            if (type is AmbiguousErrorTypeSymbol ambiguous && containingFile != null)
            {
                TypeSymbol best = null;

                // choose the one declared in this file unconditionally
                foreach (var x in ambiguous
                        .CandidateSymbols
                        .Cast<TypeSymbol>()
                        .Where(t => !t.IsUnreachable))
                    //.Where(x => x is SourceTypeSymbol srct && !srct.Syntax.IsConditional &&
                    //            srct.ContainingFile == containingFile))
                {
                    if (best == null)
                    {
                        best = x;
                    }
                    else
                    {
                        best = null;
                        break;
                    }
                }

                if (best != null)
                {
                    type = (NamedTypeSymbol)best;
                }
            }


            //
            return (ResolvedType = type);
        }

        public override string ToString() => ClassName.ToString();

        // public override TypeRefMask GetTypeRefMask(TypeRefContext ctx) => ctx.GetTypeMask(this, true);

        public override bool Equals(IBoundTypeRef other) => base.Equals(other) || (other is BoundClassTypeRef ct &&
            ct.ClassName == this.ClassName && ct.TypeArguments.IsDefaultOrEmpty);
    }

    #endregion

    #region BoundGenericClassTypeRef

    [DebuggerDisplay("BoundGenericClassTypeRef ({_targetType,nq}`{_typeArguments.Length})")]
    public sealed partial class BoundGenericClassTypeRef : BoundTypeRef
    {
        readonly IBoundTypeRef _targetType;
        readonly ImmutableArray<BoundTypeRef> _typeArguments;

        public BoundGenericClassTypeRef(IBoundTypeRef targetType, ImmutableArray<BoundTypeRef> typeArguments)
        {
            _targetType = targetType ?? throw ExceptionUtilities.ArgumentNull(nameof(targetType));
            _typeArguments = typeArguments;
        }

        public override bool IsObject => true;

        public override ImmutableArray<IBoundTypeRef> TypeArguments => _typeArguments.CastArray<IBoundTypeRef>();

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw new NotImplementedException();
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            var resolved = (NamedTypeSymbol)(_targetType.Type ?? _targetType.ResolveTypeSymbol(compilation));

            if (resolved.IsValidType())
            {
                // TODO: check _typeArguments are bound (no ErrorSymbol)

                var boundTypeArgs = _typeArguments.SelectAsArray(tref =>
                    (TypeSymbol)(tref.Type ?? tref.ResolveTypeSymbol(compilation)));

                return resolved.Construct(boundTypeArgs);
            }
            else
            {
                // TODO: error type symbol
                return compilation.CoreTypes.Object.Symbol;
            }
        }

        public override string ToString() => _targetType.ToString() + "`" + _typeArguments.Length;

        //public override TypeRefMask GetTypeRefMask(TypeRefContext ctx) => ctx.GetTypeMask(this, true);

        public override bool Equals(IBoundTypeRef other)
        {
            if (ReferenceEquals(this, other)) return true;

            if (other is BoundGenericClassTypeRef gt && gt._targetType.Equals(_targetType) &&
                gt._typeArguments.Length == _typeArguments.Length)
            {
                for (int i = 0; i < _typeArguments.Length; i++)
                {
                    if (!other.TypeArguments[i].Equals(this.TypeArguments[i])) return false;
                }

                return true;
            }

            return false;
        }
    }

    #endregion

    #region BoundIndirectTypeRef

    public sealed class BoundIndirectTypeRef : BoundTypeRef
    {
        public BoundExpression TypeExpression => _typeExpression;
        readonly BoundExpression _typeExpression;

        public BoundIndirectTypeRef(BoundExpression typeExpression, bool objectTypeInfoSemantic)
        {
            _typeExpression = typeExpression ?? throw ExceptionUtilities.ArgumentNull();
            ObjectTypeInfoSemantic = objectTypeInfoSemantic;
        }

        public BoundIndirectTypeRef Update(BoundExpression typeExpression, bool objectTypeInfoSemantic)
        {
            if (typeExpression == _typeExpression && objectTypeInfoSemantic == ObjectTypeInfoSemantic)
            {
                return this;
            }
            else
            {
                return new BoundIndirectTypeRef(typeExpression, ObjectTypeInfoSemantic).WithSyntax(AquilaSyntax);
            }
        }

        /// <summary>
        /// Gets value determining the indirect type reference can refer to an object instance which type is used to get the type info.
        /// </summary>
        public bool ObjectTypeInfoSemantic { get; }

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public override bool IsNullable
        {
            get { return false; }
            set { Debug.Assert(value == false); }
        }

        public override bool IsObject => true;

        internal void EmitClassName(CodeGenerator cg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Whether this is <c>$this</c> variable used as a type.
        /// </summary>
        bool IsThisVariable
        {
            get { throw new NotImplementedException(); }
        }

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(IBoundTypeRef other) => base.Equals(other) ||
                                                            (other is BoundIndirectTypeRef it &&
                                                             it._typeExpression == _typeExpression);

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            // MOVED TO GRAPH REWRITER:

            //// string:
            //if (_typeExpression.ConstantValue.TryConvertToString(out var tname))
            //{
            //    return (TypeSymbol)_model.ResolveType(NameUtils.MakeQualifiedName(tname, true));
            //}
            //else if (IsThisVariable)
            //{
            //    // $this:
            //    if (_typeExpression is BoundVariableRef varref && varref.Name.NameValue.IsThisVariableName)
            //    {
            //        if (TypeCtx.ThisType != null && TypeCtx.ThisType.IsSealed)
            //        {
            //            return TypeCtx.ThisType; // $this, self
            //        }
            //    }
            //    //else if (IsClassOnly(tref.TypeExpression.TypeRefMask))
            //    //{
            //    //    // ...
            //    //}
            //}

            return null; // type cannot be resolved
        }

        // public override IBoundTypeRef Transfer(TypeRefContext source, TypeRefContext target)
        // {
        //     if (source == target) return this;
        //
        //     // it is "an" object within another method:
        //     return new BoundPrimitiveTypeRef(AquilaTypeCode.Object) {IsNullable = false};
        // }

        // public override TypeRefMask GetTypeRefMask(TypeRefContext ctx)
        // {
        //     if (IsThisVariable)
        //     {
        //         return ctx.GetThisTypeMask();
        //     }
        //
        //     return ctx.GetSystemObjectTypeMask();
        // }

        public override string ToString() => "{?}";

        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
        //     visitor.VisitIndirectTypeRef(this);
        public override BoundKind BoundKind { get; }
    }

    #endregion

    #region BoundMultipleTypeRef

    sealed class BoundMultipleTypeRef : BoundTypeRef
    {
        public ImmutableArray<BoundTypeRef> TypeRefs { get; private set; }

        public override bool IsObject => true;

        public BoundMultipleTypeRef(ImmutableArray<BoundTypeRef> trefs)
        {
            this.TypeRefs = trefs;
            this.IsNullable = trefs.Any(t => t.IsNullable);
        }

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw new NotImplementedException();
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            var result = (TypeSymbol)TypeRefs[0].ResolveTypeSymbol(compilation);

            for (int i = 1; i < TypeRefs.Length; i++)
            {
                var tref = TypeRefs[i];
                if (tref is BoundPrimitiveTypeRef pt && pt.TypeCode == AquilaTypeCode.Null)
                {
                    Debug.Assert(IsNullable);
                    continue;
                }

                result = compilation.Merge(result, (TypeSymbol)tref.ResolveTypeSymbol(compilation));
            }

            //if (IsNullable)
            //{
            //    result = compilation.MergeNull(result);
            //}

            return result;
        }

        public override string ToString() => string.Join("|", TypeRefs);

        // public override TypeRefMask GetTypeRefMask(TypeRefContext ctx)
        // {
        //     TypeRefMask result = 0;
        //
        //     foreach (var t in TypeRefs)
        //     {
        //         result |= t.GetTypeRefMask(ctx);
        //     }
        //
        //     if (IsNullable)
        //     {
        //         result |= ctx.GetNullTypeMask();
        //     }
        //
        //     return result;
        // }

        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
        //     visitor.VisitMultipleTypeRef(this);

        public BoundMultipleTypeRef Update(ImmutableArray<BoundTypeRef> trefs)
        {
            if (trefs == this.TypeRefs)
            {
                return this;
            }
            else
            {
                return new BoundMultipleTypeRef(trefs).WithSyntax(AquilaSyntax);
            }
        }

        public override BoundKind BoundKind { get; }
    }

    #endregion

    #region Helper implementations:

    /// <summary>
    /// <see cref="IBoundTypeRef"/> refering to resolved reference type symbol.
    /// </summary>
    public sealed partial class BoundTypeRefFromSymbol : BoundTypeRef
    {
        bool IsAquilaCorLibrary => _symbol.ContainingAssembly is AssemblySymbol ass && ass.IsAquilaCorLibrary;

        public override bool IsObject
        {
            get
            {
                switch (_symbol.SpecialType)
                {
                    case SpecialType.System_DateTime:
                        return true;

                    case SpecialType.System_String:
                        return false;

                    case SpecialType.None:


                        return _symbol.IsReferenceType;

                    default:
                        return _symbol.IsReferenceType;
                }
            }
        }

        public override ITypeSymbol Type => _symbol;


        partial void OnCreateImpl(ITypeSymbol symbol)
        {
            Debug.Assert(((TypeSymbol)symbol).IsValidType());
            Contract.ThrowIfNull(_symbol);
        }

        public override string ToString() => _symbol.ToString();

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false) =>
            throw new NotImplementedException();

        //public override IBoundTypeRef Transfer(TypeRefContext source, TypeRefContext target) => this;

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation) =>
            _symbol;

        public override bool Equals(IBoundTypeRef other) =>
            base.Equals(other) || (other is BoundTypeRefFromSymbol ts && ts._symbol == _symbol);
    }

    #endregion
}