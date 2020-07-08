using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Attributes
{
    /// <summary>
    /// Class to represent a synthesized attribute
    /// </summary>
    internal sealed class SynthesizedAttributeData : SourceAttributeData
    {
        internal SynthesizedAttributeData(MethodSymbol ctor, ImmutableArray<TypedConstant> arguments, ImmutableArray<KeyValuePair<String, TypedConstant>> namedArguments)
            : base(
            applicationNode: null,
            attributeClass: (NamedTypeSymbol)ctor.ContainingType,
            attributeConstructor: ctor,
            constructorArguments: arguments,
            constructorArgumentsSourceIndices: default(ImmutableArray<int>),
            namedArguments: namedArguments,
            hasErrors: false,
            isConditionallyOmitted: false)
        {
            Debug.Assert((object)ctor != null);
            Debug.Assert(!arguments.IsDefault);
            Debug.Assert(!namedArguments.IsDefault); // Frequently empty though.
        }
    }
}
