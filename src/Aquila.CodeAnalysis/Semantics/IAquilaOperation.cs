﻿using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.Syntax;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics
{
    public interface IAquilaOperation : IOperation
    {
        /// <summary>
        /// Corresponding syntax node.
        /// </summary>
        AquilaSyntaxNode AquilaSyntax { get; set; }

        /// <summary>
        /// Visitor with return value implementation.
        /// </summary>
        /// <typeparam name="TResult">Result type of the <paramref name="visitor"/>, <see cref="VoidStruct"/> if none.</typeparam>
        /// <param name="visitor">A reference to <see cref="AquilaOperationVisitor{TResult}"/> instance.</param>
        /// <returns>The value returned by the <paramref name="visitor"/>.</returns>
        TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor);
    }

    /// <summary>
    /// Abstract Aquila expression semantic.
    /// </summary>
    public interface IAquilaExpression : IAquilaOperation
    {
        /// The way the expression is accessed.
        /// May specify an additional operation or conversion.
        /// May specify the type that the expression will be converted to.
        /// </summary>
        BoundAccess Access { get; }

        /// <summary>
        /// Whether the expression needs current <c>Context</c> to be evaluated.
        /// If not, the expression can be evaluated in compile time or in app context.
        /// </summary>
        bool RequiresContext { get; }

        /// <summary>
        /// Decides whether an expression represented by this operation should be copied if it is passed by value (assignment, return).
        /// </summary>
        bool IsDeeplyCopied { get; }
    }

    public interface IAquilaStatement : IAquilaOperation
    {
    }
}