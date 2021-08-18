using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace Aquila.Metadata
{
    public static class SymantecHelper
    {
        /// <summary>
        /// Get types with prefix/postfix addition for generating types, schema, etc...
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<(string prefix, string postfix, SMType type, bool isType, bool isComplex)>
            GetOrderedFlattenTypes(this IEnumerable<SMType> types)
        {
            var smTypes = types.OrderBy(x => x.Name).ToImmutableArray();

            var isComplexType = smTypes.Count() > 1;

            if (isComplexType)
            {
                yield return new("", $"_T", new SMType(SMType.Int), true, isComplexType);
            }

            foreach (var type in smTypes)
            {
                var postfix = GetPropertyPostfix(type);
                yield return new("", $"{((isComplexType) ? "_" + postfix : "")}", type, false, isComplexType);
            }
        }

        /// <summary>
        /// Get types with prefix/postfix addition for generating types, schema, etc...
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<(string prefix, string postfix, SMType type, bool isType, bool isComplex)>
            GetOrderedFlattenTypes(this SMProperty prop)
        {
            return prop.Types.GetOrderedFlattenTypes();
        }

        private static string GetPropertyPostfix(SMType type)
        {
            var postfix = type.Kind switch
            {
                SMTypeKind.Int => "I",
                SMTypeKind.DateTime => "D",
                SMTypeKind.Binary => "B",
                SMTypeKind.String => "S",
                SMTypeKind.Guid => "G",
                SMTypeKind.Reference => "R",
                SMTypeKind.Decimal => "N",
                SMTypeKind.Long => "L",
                _ => throw new NotImplementedException()
            };
            return postfix;
        }
    }
}