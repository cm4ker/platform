using Microsoft.CodeAnalysis.Operations;
using Aquila.CodeAnalysis.Symbols;
using System;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.Syntax.Syntax;


namespace Aquila.CodeAnalysis.Semantics
{
    #region BoundVariable

    /// <summary>
    /// Represents a variable within routine.
    /// </summary>
    public partial class BoundVariable : BoundOperation
    {
        /// <summary>
        /// Associated symbol, local or parameter.
        /// </summary>
        internal abstract Symbol Symbol { get; }

        /// <summary>
        /// Name of the variable.
        /// </summary>
        public virtual string Name => this.Symbol.Name;

        public bool IsInvalid => false;

        public SyntaxNode Syntax => null;
    }

    #endregion

    #region BoundLocal

    public partial class BoundLocal : IVariableDeclaratorOperation
    {
        IVariableInitializerOperation IVariableDeclaratorOperation.Initializer => null;

        ILocalSymbol IVariableDeclaratorOperation.Symbol => _localSymbol;

        internal override Symbol Symbol => _localSymbol;

        ImmutableArray<IOperation> IVariableDeclaratorOperation.IgnoredArguments => ImmutableArray<IOperation>.Empty;

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitVariableDeclarator(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitVariableDeclarator(this, argument);
        }
    }

    #endregion

    #region BoundIndirectLocal

    public partial class BoundIndirectLocal : IVariableDeclaratorOperation
    {
        public override OperationKind Kind => OperationKind.VariableDeclaration;

        internal override Symbol Symbol => null;

        IVariableInitializerOperation IVariableDeclaratorOperation.Initializer => null;

        ILocalSymbol IVariableDeclaratorOperation.Symbol => (ILocalSymbol) Symbol;

        ImmutableArray<IOperation> IVariableDeclaratorOperation.IgnoredArguments => ImmutableArray<IOperation>.Empty;

        public override string Name => null;

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitVariableDeclarator(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitVariableDeclarator(this, argument);
        }
    }

    #endregion

    #region BoundParameter

    public partial class BoundParameter : BoundVariable, IParameterInitializerOperation
    {
        IParameterSymbol IParameterInitializerOperation.Parameter => _parameterSymbol;

        ImmutableArray<ILocalSymbol> ISymbolInitializerOperation.Locals => ImmutableArray<ILocalSymbol>.Empty;

        public IOperation Value => _initializer;

        internal override Symbol Symbol => _parameterSymbol;

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.VisitParameterInitializer(this);
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.VisitParameterInitializer(this, argument);
        }
    }

    #endregion

    #region BoundThisParameter

    /// <summary>
    /// Represents <c>$this</c> variable in PHP code.
    /// </summary>
    public partial class BoundThisParameter : BoundVariable
    {
        public override string Name => VariableName.ThisVariableName.Value;

        internal override Aquila.CodeAnalysis.Symbols.Symbol Symbol => null;

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result)
        {
            result = visitor.DefaultVisit(this, argument);
        }

        partial void AcceptImpl(OperationVisitor visitor)
        {
            visitor.DefaultVisit(this);
        }
    }

    #endregion

    #region BoundSuperGlobalVariable

    public partial class BoundSuperGlobalVariable : BoundVariable
    {
        private VariableName _name;

        public BoundSuperGlobalVariable(VariableName name)
            : base(VariableKind.GlobalVariable)
        {
            _name = name;
        }

        public override string Name => _name.Value;

        public override OperationKind Kind => OperationKind.None;

        internal override Aquila.CodeAnalysis.Symbols.Symbol Symbol
        {
            get { throw new NotImplementedException(); }
        }

        public override void Accept(OperationVisitor visitor)
        {
            throw new NotSupportedException();
        }

        public override TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument)
        {
            throw new NotSupportedException();
        }
    }

    #endregion
}