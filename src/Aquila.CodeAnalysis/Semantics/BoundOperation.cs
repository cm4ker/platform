using System;
using System.Collections.Generic;
using Aquila.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;


namespace Aquila.CodeAnalysis.Semantics
{
    public abstract class BoundOperation : IOperation
    {
        #region Unsupported

        SyntaxNode IOperation.Syntax => null;

        IOperation IOperation.Parent => null;

        IEnumerable<IOperation> IOperation.Children => Array.Empty<IOperation>();

        SemanticModel IOperation.SemanticModel => null;

        #endregion

        public string Language => LanguageConstants.LanguageId;

        public virtual bool IsImplicit => false;

        public abstract OperationKind Kind { get; }

        public abstract BoundKind BoundKind { get; }

        public virtual ITypeSymbol Type => null;

        /// <summary>
        /// Resolved value of the expression.
        /// </summary>
        Optional<object> IOperation.ConstantValue => ConstantValueHlp;

        protected virtual Optional<object> ConstantValueHlp => default(Optional<object>);

        public abstract void Accept(OperationVisitor visitor);

        public abstract TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor,
            TArgument argument);

        public virtual TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
            => throw new NotImplementedException();
    }
}