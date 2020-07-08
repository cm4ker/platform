using System;
using Aquila.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis
{
    internal static partial class TypeRefFactory
    {
        /// <summary>
        /// Creates type context for a method within given type, determines naming, type context.
        /// </summary>
        public static TypeRefContext CreateTypeRefContext(NamedTypeSymbol containingType)
        {
            Contract.ThrowIfNull(containingType);

            return new TypeRefContext(
                containingType.DeclaringCompilation,
                containingType, // scope
                null);
        }
    }
}