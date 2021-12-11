using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Base class representing a statement semantic.
    /// </summary>
    public abstract partial class BoundStatement : BoundOperation, IAquilaStatement
    {
        public virtual bool IsInvalid => false;

        public AquilaSyntaxNode AquilaSyntax { get; set; }
    }

    public sealed partial class BoundEmptyStmt : BoundStatement, IEmptyOperation
    {
    }

    /// <summary>
    /// Represents an expression statement.
    /// </summary>
    public sealed partial class BoundExpressionStmt : BoundStatement, IExpressionStatementOperation
    {
        /// <summary>
        /// Expression of the statement.
        /// </summary>
        IOperation IExpressionStatementOperation.Operation => Expression;

        partial void AcceptImpl(OperationVisitor visitor) => visitor.VisitExpressionStatement(this);

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitExpressionStatement(this, argument);
        }
    }

    /// <summary>
    /// return <c>optional</c>;
    /// </summary>
    public sealed partial class BoundReturnStmt : BoundStatement, IReturnOperation
    {
        IOperation IReturnOperation.ReturnedValue => Returned;

        partial void AcceptImpl(OperationVisitor visitor) => visitor.VisitReturn(this);

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitReturn(this, argument);
        }
    }

    /// <summary>
    /// Conditionally declared functions.
    /// </summary>
    public sealed partial class BoundMethodDeclStmt : IInvalidOperation
    {
        internal MethodDecl FunctionDecl => (MethodDecl)AquilaSyntax;

        partial void OnCreateImpl(SourceMethodSymbol method)
        {
            this.AquilaSyntax = (MethodDecl)method.Syntax;
        }

        partial void AcceptImpl(OperationVisitor visitor) => visitor.VisitInvalid(this);

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitInvalid(this, argument);
        }
    }

    public sealed partial class BoundGlobalVariableStatement : BoundStatement, IVariableDeclarationOperation
    {
        ImmutableArray<IOperation> IVariableDeclarationOperation.IgnoredDimensions => ImmutableArray<IOperation>.Empty;

        public override OperationKind Kind => OperationKind.VariableDeclaration;
        public override BoundKind BoundKind { get; }

        ImmutableArray<IVariableDeclaratorOperation> IVariableDeclarationOperation.Declarators =>
            ImmutableArray<IVariableDeclaratorOperation>.Empty; // unbound yet

        IVariableInitializerOperation IVariableDeclarationOperation.Initializer => null;

        /// <summary>
        /// The variable that will be referenced to a global variable.
        /// </summary>
        public BoundVariableRef Variable { get; internal set; }

        public BoundGlobalVariableStatement(BoundVariableRef variable)
        {
            Variable = variable;
        }

        public BoundGlobalVariableStatement Update(BoundVariableRef variable)
        {
            if (variable == Variable)
            {
                return this;
            }
            else
            {
                return new BoundGlobalVariableStatement(variable);
            }
        }

        public override void Accept(OperationVisitor visitor)
            => visitor.VisitVariableDeclaration(this);

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
            => visitor.VisitVariableDeclaration(this, argument);

        // /// <summary>Invokes corresponding <c>Visit</c> method on given <paramref name="visitor"/>.</summary>
        // /// <param name="visitor">A reference to a <see cref="AquilaOperationVisitor{TResult}"/> instance. Cannot be <c>null</c>.</param>
        // /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        // public override TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor) =>
        //     visitor.VisitGlobalStatement(this);
    }

    public sealed partial class BoundGlobalConstDeclStmt
    {
        partial void OnCreateImpl(QualifiedName name, BoundExpression value)
        {
            Debug.Assert(value.Access.IsRead);
        }
    }

    public sealed partial class BoundStaticVarStmt : IVariableDeclarationOperation
    {
        ImmutableArray<IOperation> IVariableDeclarationOperation.IgnoredDimensions => ImmutableArray<IOperation>.Empty;

        internal struct StaticVarDecl : IEquatable<StaticVarDecl>
        {
            public BoundExpression InitialValue;

            /// <summary>
            /// Variable name.
            /// </summary>
            public string Name => string.Empty;

            public bool Equals(StaticVarDecl other) =>
                InitialValue == other.InitialValue;

            public override bool Equals(object obj) => obj is StaticVarDecl v && Equals(v);

            public override int GetHashCode() => -1;

            public static bool operator ==(StaticVarDecl a, StaticVarDecl b) => a.Equals(b);

            public static bool operator !=(StaticVarDecl a, StaticVarDecl b) => !a.Equals(b);
        }

        public override OperationKind Kind => OperationKind.VariableDeclaration;

        ImmutableArray<IVariableDeclaratorOperation> IVariableDeclarationOperation.Declarators =>
            ImmutableArray<IVariableDeclaratorOperation>.Empty;

        IVariableInitializerOperation IVariableDeclarationOperation.Initializer => null;

        internal StaticVarDecl Declaration => _variable;
        readonly StaticVarDecl _variable;

        /// <summary>
        /// Synthesized type containing <c>value</c> field with the actual value.
        /// Cannot be <c>null</c>.
        /// </summary>
        internal NamedTypeSymbol HolderClass => _holderClass;

        readonly SynthesizedStaticLocHolder _holderClass;

        internal BoundStaticVarStmt(StaticVarDecl variable, SynthesizedStaticLocHolder holder)
        {
            _variable = variable;
            _holderClass = holder ?? throw ExceptionUtilities.ArgumentNull(nameof(holder));
        }

        internal BoundStaticVarStmt Update(StaticVarDecl variable)
        {
            if (variable == _variable)
            {
                return this;
            }
            else
            {
                return new BoundStaticVarStmt(variable, _holderClass);
            }
        }

        partial void AcceptImpl(OperationVisitor visitor) => visitor.VisitVariableDeclaration(this);

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitVariableDeclaration(this, argument);
        }
    }

    /// <summary>
    /// Represents yield return and continuation.
    /// </summary>
    public partial class BoundYieldStmt : IReturnOperation
    {
        public BoundExpression YieldedValue { get; internal set; }
        public BoundExpression YieldedKey { get; internal set; }

        IOperation IReturnOperation.ReturnedValue => YieldedValue;

        /// <summary>
        /// The yield expression unique ordinal value within the method.
        /// Indexed from one.
        /// </summary>
        public int YieldIndex { get; }

        /// <summary>
        /// Gets value indicating the `yield` is a part of `yield from` semantics.
        /// In result, keys yielded by this statement do not update Generator auto-incremented keys.
        /// </summary>
        public bool IsYieldFrom { get; set; }

        /// <summary>
        /// "try" scopes in which is this statement included ("catch" and "finally" are handled differently).
        /// Generator state machine may only jump before these scopes (CIL does not allow jumping into).
        /// </summary>
        public LinkedList<TryCatchEdge> ContainingTryScopes { get; private set; } = new LinkedList<TryCatchEdge>();

        public BoundYieldStmt(int index, BoundExpression valueExpression, BoundExpression keyExpression,
            IEnumerable<TryCatchEdge> tryScopes = null)
        {
            Debug.Assert(index > 0);

            YieldIndex = index;
            YieldedValue = valueExpression;
            YieldedKey = keyExpression;

            tryScopes?.ForEach(ts => ContainingTryScopes.AddLast(ts));
        }

        public BoundYieldStmt Update(int index, BoundExpression valueExpression, BoundExpression keyExpression)
        {
            if (index == YieldIndex && valueExpression == YieldedValue && keyExpression == YieldedKey)
            {
                return this;
            }
            else
            {
                return new BoundYieldStmt(index, valueExpression, keyExpression, ContainingTryScopes);
            }
        }

        partial void AcceptImpl(OperationVisitor visitor) => visitor.VisitReturn(this);

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitReturn(this, argument);
        }
    }

    /// <summary>
    /// Represents declare statement
    /// </summary>
    public sealed partial class BoundDeclareStmt : BoundStatement
    {
    }
}