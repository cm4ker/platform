using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    static class WellKnownMembersHelper
    {
        public static AttributeData CreateCompilerGeneratedAttribute(this AquilaCompilation compilation)
        {
            // [CompilerGenerated]
            var compilergenerated =
                (MethodSymbol)compilation.GetWellKnownTypeMember(WellKnownMember
                    .System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor);

            return new SynthesizedAttributeData(
                compilergenerated,
                ImmutableArray<TypedConstant>.Empty,
                ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
        }

        public static AttributeData CreateParamsAttribute(this AquilaCompilation compilation)
        {
            return new SynthesizedAttributeData(
                (MethodSymbol)compilation.GetWellKnownTypeMember(WellKnownMember.System_ParamArrayAttribute__ctor),
                ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
        }

        public static AttributeData CreateNotNullAttribute(this AquilaCompilation compilation)
        {
            return new SynthesizedAttributeData(
                null,
                ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
        }

        public static AttributeData CreateDefaultValueAttribute(this AquilaCompilation compilation,
            TypeSymbol containingType, FieldSymbol field)
        {
            var namedparameters = ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty;

            var fieldContainer = field.ContainingType;

            if (fieldContainer != containingType)
            {
                namedparameters = ImmutableArray.Create(new KeyValuePair<string, TypedConstant>(
                    "ExplicitType",
                    compilation.CreateTypedConstant(fieldContainer)));
            }

            // [DefaultValueAttribute(name) { ExplicitType = ... }]
            return new SynthesizedAttributeData(
                null,
                ImmutableArray.Create(compilation.CreateTypedConstant(field.Name)),
                namedparameters);
        }
    }
}