using System;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Metadata;
using Aquila.Syntax.Syntax;

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
    }
}