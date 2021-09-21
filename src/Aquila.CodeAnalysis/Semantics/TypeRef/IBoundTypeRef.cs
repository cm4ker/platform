using System;
using System.Collections.Immutable;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Provides a type reference and binding to <see cref="ITypeSymbol"/>.
    /// </summary>
    public interface IBoundTypeRef : IEquatable<IBoundTypeRef>, IAquilaOperation
    {
        /// <summary>
        /// Gets value indicting that the type allows a <c>NULL</c> reference.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Gets value indicating whether the type represents an object (class or interface) and not a primitive type.
        /// </summary>
        bool IsObject { get; }

        /// <summary>
        /// Gets value indicating whether the type represents an array.
        /// </summary>
        bool IsArray { get; }

        /// <summary>
        /// Gets value indicating whether the type represents a primitive type.
        /// </summary>
        bool IsPrimitiveType { get; }

        /// <summary>
        /// Gets value indicating whether the type represents a lambda function.
        /// </summary>
        bool IsLambda { get; }

        /// <summary>
        /// In case of generic type reference, gets its bound type arguments.
        /// </summary>
        ImmutableArray<IBoundTypeRef> TypeArguments { get; }

        /// <summary>
        /// Resolve <see cref="ITypeSymbol"/> if possible.
        /// Can be <c>null</c>.
        /// </summary>
        ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation);
    }

    /// <summary>
    /// Common <see cref="IBoundTypeRef"/> implementation.
    /// </summary>
    public abstract partial class BoundTypeRef : IBoundTypeRef
    {
        public virtual bool IsNullable { get; set; }
        public virtual bool IsObject => false;
        public virtual bool IsArray => false;
        public virtual bool IsPrimitiveType => false;
        public virtual bool IsLambda => false;

        public virtual ImmutableArray<IBoundTypeRef> TypeArguments => ImmutableArray<IBoundTypeRef>.Empty;

        internal abstract ITypeSymbol EmitLoadTypeInfo(CodeGenerator cg, bool throwOnError = false);
        public abstract ITypeSymbol ResolveTypeSymbol(AquilaCompilation compilation);

        public virtual bool Equals(IBoundTypeRef other) => ReferenceEquals(this, other);
        public override bool Equals(object obj) => obj is IBoundTypeRef t && Equals(t);
        public override int GetHashCode() => base.GetHashCode();

        public override OperationKind Kind => OperationKind.None;
        public LangElement AquilaSyntax { get; set; }

        /// <summary>
        /// Lazily set type symbol if resolved.
        /// </summary>
        internal TypeSymbol ResolvedType { get; set; }

    
        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.DefaultVisit(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.DefaultVisit(this, argument);
        }
    }
}