using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Attributes
{
    /// <summary>
    /// Represents a retargeting custom attribute
    /// </summary>
    internal sealed class RetargetingAttributeData : SourceAttributeData
    {
        internal RetargetingAttributeData(
            SyntaxReference applicationNode,
            NamedTypeSymbol attributeClass,
            MethodSymbol attributeConstructor,
            ImmutableArray<TypedConstant> constructorArguments,
            ImmutableArray<int> constructorArgumentsSourceIndices,
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments,
            bool hasErrors,
            bool isConditionallyOmitted)
            : base(applicationNode, attributeClass, attributeConstructor, constructorArguments, constructorArgumentsSourceIndices, namedArguments, hasErrors, isConditionallyOmitted)
        {
        }
    }
}
