﻿// using System;
// using System.Diagnostics;
//
// namespace Aquila.Core
// {
//     [DebuggerDisplay("{DisplayString,nq}", Type = "{DebugTypeName,nq}")]
//     [DebuggerTypeProxy(typeof(PhpValueDebugView))]
//     [DebuggerNonUserCode, DebuggerStepThrough]
//     partial struct AqValue
//     {
//         static string UndefinedTypeName => "undefined";
//
//         /// <summary>
//         /// Debug textual representation of the value.
//         /// </summary>
//         public string DisplayString => TypeCode switch
//         {
//             PhpTypeCode.Null => "null", // lowercased `null` as it is shown for other CLR null references,
//             PhpTypeCode.Boolean => Boolean ? AqVariable.True : AqVariable.False, // CONSIDER: CLR's True/False
//             PhpTypeCode.Long => Convert.ToString(Long),
//             PhpTypeCode.Double => Convert.ToString(Double),
//             PhpTypeCode.PhpArray => "array (length = " + Array.Count.ToString() + ")",
//             PhpTypeCode.String => "'" + String + "'",
//             PhpTypeCode.MutableString => "'" + MutableStringBlob.ToString() + "'",
//             PhpTypeCode.Object => (Object is PhpResource resource)
//                 ? $"resource id='{resource.Id}' type='{resource.TypeName}'"
//                 : Object.GetPhpTypeInfo().Name + "#" + Object.GetHashCode().ToString("X"),
//             PhpTypeCode.Alias => "&" + Alias.Value.DisplayString,
//             _ => "invalid",
//         };
//
//         /// <summary>
//         /// Gets php type name of the value.
//         /// </summary>
//         internal string DebugTypeName => AqVariable.GetTypeName(this);
//
//         sealed class PhpValueDebugView
//         {
//             [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
//             public object DebugValue { get; }
//
//             public PhpValueDebugView(AqValue value)
//             {
//                 DebugValue = value.ToClr();
//             }
//         }
//     }
// }

