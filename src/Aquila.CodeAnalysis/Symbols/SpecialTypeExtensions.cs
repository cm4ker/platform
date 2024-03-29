﻿using System;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    internal static class SpecialTypeExtensions
    {
        /// <summary>
        /// Checks if a type is considered a "built-in integral" by CLR.
        /// </summary>
        public static bool IsIntegralType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSignedIntegralType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanBeConst(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The type is one of the simple types defined in Dev10 C#, see "predeftype.h"/simple
        /// </summary>
        public static bool IsIntrinsicType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                // NOTE: VB treats System.DateTime as an intrinsic, while C# does not, see "predeftype.h"
                //case SpecialType.System_DateTime:
                case SpecialType.System_Decimal:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsValidVolatileFieldType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Char:
                case SpecialType.System_Single:
                case SpecialType.System_Boolean:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                    return true;
                default:
                    return false;
            }
        }

        public static int FixedBufferElementSizeInBytes(this SpecialType specialType)
        {
            // SizeInBytes() handles decimal (contrary to the language spec).  But decimal is not allowed
            // as a fixed buffer element type.
            return specialType == SpecialType.System_Decimal ? 0 : specialType.SizeInBytes();
        }
        
        
        public static SpecialType FromRuntimeTypeOfLiteralValue(object value)
        {
            RoslynDebug.Assert(value != null);

            // Perf: Note that JIT optimizes each expression val.GetType() == typeof(T) to a single register comparison.
            // Also the checks are sorted by commonality of the checked types.

            if (value.GetType() == typeof(int))
            {
                return SpecialType.System_Int32;
            }

            if (value.GetType() == typeof(string))
            {
                return SpecialType.System_String;
            }

            if (value.GetType() == typeof(bool))
            {
                return SpecialType.System_Boolean;
            }

            if (value.GetType() == typeof(char))
            {
                return SpecialType.System_Char;
            }

            if (value.GetType() == typeof(long))
            {
                return SpecialType.System_Int64;
            }

            if (value.GetType() == typeof(double))
            {
                return SpecialType.System_Double;
            }

            if (value.GetType() == typeof(uint))
            {
                return SpecialType.System_UInt32;
            }

            if (value.GetType() == typeof(ulong))
            {
                return SpecialType.System_UInt64;
            }

            if (value.GetType() == typeof(float))
            {
                return SpecialType.System_Single;
            }

            if (value.GetType() == typeof(decimal))
            {
                return SpecialType.System_Decimal;
            }

            if (value.GetType() == typeof(short))
            {
                return SpecialType.System_Int16;
            }

            if (value.GetType() == typeof(ushort))
            {
                return SpecialType.System_UInt16;
            }

            if (value.GetType() == typeof(DateTime))
            {
                return SpecialType.System_DateTime;
            }

            if (value.GetType() == typeof(byte))
            {
                return SpecialType.System_Byte;
            }

            if (value.GetType() == typeof(sbyte))
            {
                return SpecialType.System_SByte;
            }

            return SpecialType.None;
        }
    }
}
