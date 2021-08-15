using System;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Metadata;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.Syntax.Metadata
{
    internal static class MetadataTypeProvider
    {
        public static TypeSymbol Resolve(AquilaCompilation compilation, SMType typeName)
        {
            var ct = compilation.CoreTypes;

            return typeName.Kind switch
            {
                SMTypeKind.Int => ct.Int32,
                SMTypeKind.Long => ct.Int64,
                SMTypeKind.String => ct.String,
                SMTypeKind.Double => ct.Double,
                SMTypeKind.Decimal => ct.Decimal,
                SMTypeKind.Guid => ct.Guid,
                SMTypeKind.Reference => compilation.PlatformSymbolCollection.GetType(
                    QualifiedName.Parse(typeName.GetSemantic().ReferenceName, true)),
                _ => throw new NotImplementedException("Unknown metadata type")
            };
        }

        public static SMType Resolve(AquilaCompilation compilation, ITypeSymbol tSymbol)
        {
            return tSymbol switch
            {
                { } x when x.SpecialType == SpecialType.System_Int32 => new SMType(SMType.Int),
                { } x when x.SpecialType == SpecialType.System_String => new SMType(SMType.String),
                { } x when x.SpecialType == SpecialType.System_DateTime => new SMType(SMType.DateTime),
                { } x when x.SpecialType == SpecialType.System_Boolean => new SMType(SMType.Boolean),
                { } x when x.SpecialType == SpecialType.System_Decimal => new SMType(SMType.Numeric),
                _ => throw new NotImplementedException($"Can't translate type {tSymbol} to database type")
            };
        }
    }
}