using System.Collections.Generic;
using System.Diagnostics;

namespace Aquila.Language.Ast.Extension
{
    internal static class SpecialTypes
    {
        /// <summary>
        /// Array of names for types from Cor Library.
        /// The names should correspond to ids from TypeId enum so
        /// that we could use ids to index into the array
        /// </summary>
        /// <remarks></remarks>
        private static readonly string?[] s_emittedNames = new string?[]
        {
            // The following things should be in sync:
            // 1) SpecialType enum
            // 2) names in SpecialTypes.EmittedNames array.
            // 3) languageNames in SemanticFacts.cs
            // 4) languageNames in SemanticFacts.vb
            null, // SpecialType.None
            "System.Object",
            "System.Enum",
            "System.MulticastDelegate",
            "System.Delegate",
            "System.ValueType",
            "System.Void",
            "System.Boolean",
            "System.Char",
            "System.SByte",
            "System.Byte",
            "System.Int16",
            "System.UInt16",
            "System.Int32",
            "System.UInt32",
            "System.Int64",
            "System.UInt64",
            "System.Decimal",
            "System.Single",
            "System.Double",
            "System.String",
            "System.IntPtr",
            "System.UIntPtr",
            "System.Array",
            "System.Collections.IEnumerable",
            "System.Collections.Generic.IEnumerable`1",
            "System.Collections.Generic.IList`1",
            "System.Collections.Generic.ICollection`1",
            "System.Collections.IEnumerator",
            "System.Collections.Generic.IEnumerator`1",
            "System.Collections.Generic.IReadOnlyList`1",
            "System.Collections.Generic.IReadOnlyCollection`1",
            "System.Nullable`1",
            "System.DateTime",
            "System.Runtime.CompilerServices.IsVolatile",
            "System.IDisposable",
            "System.TypedReference",
            "System.ArgIterator",
            "System.RuntimeArgumentHandle",
            "System.RuntimeFieldHandle",
            "System.RuntimeMethodHandle",
            "System.RuntimeTypeHandle",
            "System.IAsyncResult",
            "System.AsyncCallback",
            "System.Runtime.CompilerServices.RuntimeFeature",
        };

        private readonly static Dictionary<string, SpecialType> s_nameToTypeIdMap;

        private static readonly PrimitiveTypeCode[] s_typeIdToTypeCodeMap;
        private static readonly SpecialType[] s_typeCodeToTypeIdMap;

        static SpecialTypes()
        {
            s_nameToTypeIdMap = new Dictionary<string, SpecialType>((int)SpecialType.Count);

            int i;

            for (i = 1; i < s_emittedNames.Length; i++)
            {
                string? name = s_emittedNames[i];
                Debug.Assert(name.IndexOf('+') < 0); // Compilers aren't prepared to lookup for a nested special type.
                s_nameToTypeIdMap.Add(name, (SpecialType)i);
            }

            s_typeIdToTypeCodeMap = new PrimitiveTypeCode[(int)SpecialType.Count + 1];

            for (i = 0; i < s_typeIdToTypeCodeMap.Length; i++)
            {
                s_typeIdToTypeCodeMap[i] = PrimitiveTypeCode.NotPrimitive;
            }

            s_typeIdToTypeCodeMap[(int)SpecialType.System_Boolean] = PrimitiveTypeCode.Boolean;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Char] = PrimitiveTypeCode.Char;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Void] = PrimitiveTypeCode.Void;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_String] = PrimitiveTypeCode.String;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int64] = PrimitiveTypeCode.Int64;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int32] = PrimitiveTypeCode.Int32;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int16] = PrimitiveTypeCode.Int16;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_SByte] = PrimitiveTypeCode.Int8;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt64] = PrimitiveTypeCode.UInt64;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt32] = PrimitiveTypeCode.UInt32;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt16] = PrimitiveTypeCode.UInt16;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Byte] = PrimitiveTypeCode.UInt8;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Single] = PrimitiveTypeCode.Float32;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Double] = PrimitiveTypeCode.Float64;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_IntPtr] = PrimitiveTypeCode.IntPtr;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UIntPtr] = PrimitiveTypeCode.UIntPtr;

            s_typeCodeToTypeIdMap = new SpecialType[(int)PrimitiveTypeCode.Invalid + 1];

            for (i = 0; i < s_typeCodeToTypeIdMap.Length; i++)
            {
                s_typeCodeToTypeIdMap[i] = SpecialType.None;
            }

            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Boolean] = SpecialType.System_Boolean;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Char] = SpecialType.System_Char;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Void] = SpecialType.System_Void;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.String] = SpecialType.System_String;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Int64] = SpecialType.System_Int64;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Int32] = SpecialType.System_Int32;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Int16] = SpecialType.System_Int16;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Int8] = SpecialType.System_SByte;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.UInt64] = SpecialType.System_UInt64;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.UInt32] = SpecialType.System_UInt32;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.UInt16] = SpecialType.System_UInt16;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.UInt8] = SpecialType.System_Byte;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Float32] = SpecialType.System_Single;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.Float64] = SpecialType.System_Double;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.IntPtr] = SpecialType.System_IntPtr;
            s_typeCodeToTypeIdMap[(int)PrimitiveTypeCode.UIntPtr] = SpecialType.System_UIntPtr;
        }

        /// <summary>
        /// Gets the name of the special type as it would appear in metadata.
        /// </summary>
        public static string? GetMetadataName(this SpecialType id)
        {
            return s_emittedNames[(int)id];
        }

        public static SpecialType GetTypeFromMetadataName(string metadataName)
        {
            SpecialType id;

            if (s_nameToTypeIdMap.TryGetValue(metadataName, out id))
            {
                return id;
            }

            return SpecialType.None;
        }

        public static SpecialType GetTypeFromMetadataName(PrimitiveTypeCode typeCode)
        {
            return s_typeCodeToTypeIdMap[(int)typeCode];
        }

        public static PrimitiveTypeCode GetTypeCode(SpecialType typeId)
        {
            return s_typeIdToTypeCodeMap[(int)typeId];
        }
    }
}