using System;
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
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics.TypeRef
{
    #region BoundPrimitiveTypeRef

    [DebuggerDisplay("BoundPrimitiveTypeRef ({_type})")]
    public sealed partial class BoundPrimitiveTypeRef : BoundTypeRef
    {
        public AquilaTypeCode TypeCode => _type;

        partial void OnCreateImpl(AquilaTypeCode type, ITypeSymbol symbol)
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

        public override bool Equals(IBoundTypeRef other) => base.Equals(other) ||
                                                            (other is BoundArrayTypeRef at);
    }

    #endregion

    #region BoundLambdaTypeRef

    [DebuggerDisplay("BoundLambdaTypeRef ({_returnType})")]
    sealed class BoundLambdaTypeRef : BoundTypeRef
    {
        public override bool IsObject => true; // Closure

        public override bool IsLambda => true;

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => NameUtils.SpecialNames.Closure.ToString();
        public override BoundKind BoundKind { get; }

        public BoundLambdaTypeRef() : base(null)
        {
        }
    }

    #endregion

    #region BoundClassTypeRef

    [DebuggerDisplay("BoundClassTypeRef ({ToString(),nq})")]
    public sealed partial class BoundClassTypeRef
    {
        public QualifiedName ClassName { get; }


        partial void OnCreateImpl(QualifiedName qName, SourceMethodSymbolBase method, ITypeSymbol symbol, int arity)
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

            var containingFile = _method?.Syntax.SyntaxTree;

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

        public BoundGenericClassTypeRef(IBoundTypeRef targetType, ImmutableArray<BoundTypeRef> typeArguments) :
            base(null)
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

        public BoundIndirectTypeRef(BoundExpression typeExpression, bool objectTypeInfoSemantic) : base(null)
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
            return null;
        }

        public override string ToString() => "{?}";

        public override BoundKind BoundKind { get; }
    }

    #endregion

    #region BoundMultipleTypeRef

    sealed class BoundMultipleTypeRef : BoundTypeRef
    {
        public ImmutableArray<BoundTypeRef> TypeRefs { get; private set; }

        public override bool IsObject => true;

        public BoundMultipleTypeRef(ImmutableArray<BoundTypeRef> trefs) : base(null)
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

            return result;
        }

        public override string ToString() => string.Join("|", TypeRefs);

        public BoundMultipleTypeRef Update(ImmutableArray<BoundTypeRef> trefs)
        {
            return trefs == this.TypeRefs ? this : new BoundMultipleTypeRef(trefs).WithSyntax(AquilaSyntax);
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
        bool IsAquilaCorLibrary => ResultType.ContainingAssembly is AssemblySymbol ass && ass.IsAquilaCorLibrary;

        public override bool IsObject
        {
            get
            {
                switch (ResultType.SpecialType)
                {
                    case SpecialType.System_DateTime:
                        return true;

                    case SpecialType.System_String:
                        return false;

                    case SpecialType.None:
                        return ResultType.IsReferenceType;

                    default:
                        return ResultType.IsReferenceType;
                }
            }
        }

        partial void OnCreateImpl(ITypeSymbol symbol)
        {
            Debug.Assert(((TypeSymbol)symbol).IsValidType());
            Contract.ThrowIfNull(symbol);
        }

        public override string ToString() => ResultType.ToString();

        internal override ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false) =>
            throw new NotImplementedException();

        public override ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation) => ResultType;

        public override bool Equals(IBoundTypeRef other) =>
            base.Equals(other) || (other is BoundTypeRefFromSymbol ts && ts.ResultType == ResultType);
    }

    #endregion
}