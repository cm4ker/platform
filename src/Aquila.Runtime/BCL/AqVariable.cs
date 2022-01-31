// using System;
// using System.Collections.Generic;
// using System.Text;
//
// namespace Aquila.Core
// {
//     public static class AqVariable
//     {
//         #region Types
//
//         /// <summary>
//         /// PHP name for <see cref="int"/>.
//         /// </summary>
//         public const string TypeNameInt = "int";
//
//         public const string TypeNameInteger = "integer";
//
//         /// <summary>
//         /// PHP name for <see cref="long"/>.
//         /// </summary>
//         public const string TypeNameLongInteger = "int64";
//
//         /// <summary>
//         /// PHP name for <see cref="double"/>.
//         /// </summary>
//         public const string TypeNameDouble = "double";
//
//         public const string TypeNameFloat = "float";
//
//         /// <summary>
//         /// PHP name for <see cref="bool"/>.
//         /// </summary>
//         public const string TypeNameBool = "bool";
//
//         public const string TypeNameBoolean = "boolean";
//
//         /// <summary>
//         /// PHP name for <see cref="string"/>.
//         /// </summary>
//         public const string TypeNameString = "string";
//
//         /// <summary>
//         /// PHP name for <see cref="System.Void"/>.
//         /// </summary>
//         public const string TypeNameVoid = "void";
//
//         /// <summary>
//         /// PHP name for <see cref="System.Object"/>.
//         /// </summary>
//         public const string TypeNameObject = "object";
//
//         /// <summary>
//         /// PHP name for <B>null</B>.
//         /// </summary>
//         public const string TypeNameNull = "NULL";
//
//         /// <summary>
//         /// PHP name for <B>true</B> constant.
//         /// </summary>
//         public const string True = "true";
//
//         /// <summary>
//         /// PHP name for <B>true</B> constant.
//         /// </summary>
//         public const string False = "false";
//
//         /// <summary>
//         /// Gets the PHP name of given value, used by <c>gettype()</c>.
//         /// </summary>
//         /// <param name="value">The object which type name to get.</param>
//         /// <returns>The PHP name of the type of <paramref name="value"/>.</returns>
//         /// <remarks>Returns CLR type name for variables of unknown type.</remarks>
//         public static string GetTypeName(AqValue value) =>
//             value.TypeCode switch
//             {
//                 AquilaTypeCode.Null => TypeNameNull,
//                 AquilaTypeCode.Boolean => TypeNameBoolean,
//                 AquilaTypeCode.Long => TypeNameInteger,
//                 AquilaTypeCode.Double => TypeNameDouble,
//                 AquilaTypeCode.PhpArray => PhpArray.PhpTypeName,
//                 AquilaTypeCode.String => TypeNameString,
//                 AquilaTypeCode.MutableString => TypeNameString,
//                 AquilaTypeCode.Object => value.Object is PhpResource ? PhpResource.PhpTypeName : TypeNameObject,
//                 AquilaTypeCode.Alias => GetTypeName(value.Alias.Value),
//                 _ => throw new ArgumentException(),
//             };
//
//         /// <summary>
//         /// Gets the PHP debug type name of given value, used by <c>get_debug_type()</c>.
//         /// </summary>
//         public static string GetDebugType(AqValue value) =>
//             value.TypeCode switch
//             {
//                 AquilaTypeCode.Null => "null",
//                 AquilaTypeCode.Boolean => TypeNameBool,
//                 AquilaTypeCode.Long => TypeNameInt,
//                 AquilaTypeCode.Double => TypeNameFloat,
//                 AquilaTypeCode.PhpArray => PhpArray.PhpTypeName,
//                 AquilaTypeCode.String => TypeNameString,
//                 AquilaTypeCode.MutableString => TypeNameString,
//                 AquilaTypeCode.Object => value.Object is PhpResource resource
//                     ? resource.IsValid ? $"{PhpResource.PhpTypeName} ({resource.TypeName})" :
//                     $"{PhpResource.PhpTypeName} (closed)"
//                     : value.Object.GetPhpTypeInfo().Name,
//                 AquilaTypeCode.Alias => GetTypeName(value.Alias.Value),
//                 _ => throw new ArgumentException(),
//             };
//
//         /// <summary>
//         /// Gets the PHP class name of given object instance.
//         /// Returns <see cref="TypeNameNull"/> in case of argument is <c>null</c>.
//         /// </summary>
//         public static string GetClassName(object value)
//         {
//             return value != null ? value.GetPhpTypeInfo().Name : TypeNameNull;
//         }
//
//         #endregion
//
//         /// <summary>
//         /// Enumerates deep copy of iterator values.
//         /// </summary>
//         public static IEnumerator<KeyValuePair<IntStringKey, AqValue>> EnumerateDeepCopies(
//             IEnumerator<KeyValuePair<IntStringKey, AqValue>> iterator)
//         {
//             while (iterator.MoveNext())
//             {
//                 var entry = iterator.Current;
//                 yield return new KeyValuePair<IntStringKey, AqValue>(entry.Key, entry.Value.DeepCopy());
//             }
//         }
//
//         /// <summary>
//         /// Checks whether a string is "valid" PHP variable identifier.
//         /// </summary>
//         /// <param name="name">The variable name.</param>
//         /// <returns>
//         /// Whether <paramref name="name"/> is "valid" name of variable, i.e. [_[:alpha:]][_0-9[:alpha:]]*.
//         /// This doesn't say anything about whether a variable of such name can be used in PHP, e.g. <c>${0}</c> is ok.
//         /// </returns>
//         public static bool IsValidName(string name)
//         {
//             if (string.IsNullOrEmpty(name)) return false;
//
//             // first char:
//             if (!char.IsLetter(name[0]) && name[0] != '_') return false;
//
//             // next chars:
//             for (int i = 1; i < name.Length; i++)
//             {
//                 if (!char.IsLetterOrDigit(name[i]) && name[i] != '_') return false;
//             }
//
//             return true;
//         }
//
//         /// <summary>
//         /// Determines if given callable is syntactically valid.
//         /// </summary>
//         public static bool IsValidCallback(IPhpCallable? callable)
//         {
//             return callable is PhpCallback phpcallback
//                 ? phpcallback.IsValid
//                 : callable != null;
//         }
//
//         /// <summary>
//         /// Determines if given callable is valid and referes toi an existing function.
//         /// </summary>
//         public static bool IsValidBoundCallback(Context ctx, IPhpCallable callable)
//         {
//             return callable is PhpCallback phpcallback
//                 ? phpcallback.IsValidBound(ctx)
//                 : callable != null;
//         }
//
//         /// <summary>
//         /// Determines whether the value is <see cref="int"/> or <see cref="long"/>.
//         /// </summary>
//         public static bool IsInteger(this AqValue value)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.Long:
//                     return true;
//
//                 case AquilaTypeCode.Alias:
//                     return IsInteger(value.Alias.Value);
//
//                 default:
//                     return false;
//             }
//         }
//
//         /// <summary>
//         /// Determines whether the value is <see cref="bool"/> (either as a value or an alias).
//         /// </summary>
//         public static bool IsBoolean(this AqValue value) =>
//             value.IsBoolean || (value.IsAlias && value.Alias.Value.IsBoolean);
//
//         /// <summary>
//         /// Determines whether the value is <see cref="double"/>.
//         /// </summary>
//         public static bool IsDouble(this AqValue value)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.Double:
//                     return true;
//
//                 case AquilaTypeCode.Alias:
//                     return IsDouble(value.Alias.Value);
//
//                 default:
//                     return false;
//             }
//         }
//
//         /// <summary>
//         /// In case value is a resource, gets its reference.
//         /// </summary>
//         public static PhpResource? AsResource(this AqValue value)
//         {
//             return value.AsObject() as PhpResource;
//         }
//
//         /// <summary>
//         /// In case value contains <see cref="PhpArray"/>,
//         /// its instance is returned. Otherwise <c>null</c>.
//         /// </summary>
//         /// <remarks>Value is dereferenced if necessary.</remarks>
//         public static PhpArray? ArrayOrNull(this AqValue value) => AsArray(value);
//
//         /// <summary>
//         /// Alias to <see cref="ToStringOrNull(AqValue)"/>.
//         /// </summary>
//         public static string AsString(this AqValue value) => ToStringOrNull(value);
//
//         /// <summary>
//         /// In case given value contains a string (<see cref="string"/> or <see cref="PhpString"/>),
//         /// its string representation is returned.
//         /// Otherwise <c>null</c>.
//         /// </summary>
//         public static string ToStringOrNull(this AqValue value)
//         {
//             IsString(value, out var @string);
//             return @string;
//         }
//
//         /// <summary>
//         /// In case given value contains a string (<see cref="string"/> or <see cref="PhpString"/>),
//         /// its string representation is returned.
//         /// Otherwise <c>null</c>.
//         /// </summary>
//         public static byte[]? ToBytesOrNull(this AqValue value)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.String: return Encoding.UTF8.GetBytes(value.String);
//                 case AquilaTypeCode.MutableString: return value.MutableString.ToBytes(Encoding.UTF8);
//                 case AquilaTypeCode.Alias: return ToBytesOrNull(value.Alias.Value);
//                 default: return null;
//             }
//         }
//
//         /// <summary>
//         /// In case given value contains a string (<see cref="string"/> or <see cref="PhpString"/>),
//         /// its string representation is returned.
//         /// Otherwise <c>null</c>.
//         /// </summary>
//         public static byte[]? ToBytesOrNull(this AqValue value, Context ctx)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.String: return Encoding.UTF8.GetBytes(value.String);
//                 case AquilaTypeCode.MutableString: return value.MutableString.ToBytes(ctx);
//                 case AquilaTypeCode.Alias: return ToBytesOrNull(value.Alias.Value, ctx);
//                 default: return null;
//             }
//         }
//
//         public static byte[] ToBytes(this AqValue value, Context ctx)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.MutableString: return value.MutableString.ToBytes(ctx);
//                 case AquilaTypeCode.Alias: return ToBytes(value.Alias.Value, ctx);
//                 default: return ctx.StringEncoding.GetBytes(value.ToString(ctx));
//             }
//         }
//
//         /// <summary>
//         /// In case the value contains a php string with binary data, gets array of bytes. Otherwise <c>null</c>.
//         /// </summary>
//         public static byte[]? AsBytesOrNull(this AqValue value, Context ctx)
//         {
//             return (value.Object is PhpAlias alias ? alias.Value.Object : value.Object) is PhpString.Blob blob &&
//                    blob.ContainsBinaryData
//                 ? blob.ToBytes(ctx)
//                 : null;
//         }
//
//         /// <summary>
//         /// In case given value contains an array (<see cref="PhpArray"/>),
//         /// it is returned. Otherwise <c>null</c>.
//         /// </summary>
//         public static PhpArray? AsArray(this AqValue value)
//         {
//             return (value.Object is PhpAlias alias ? alias.Value.Object : value.Object) as PhpArray;
//             //return value.Object as PhpArray ?? (value.Object is PhpAlias alias ? alias.Value.Object as PhpArray : null);
//         }
//
//         /// <summary>
//         /// Checks the value is of type <c>string</c> or <c>&amp;string</c> and gets its value.
//         /// Single-byte strings are decoded using <c>UTF-8</c>.
//         /// </summary>
//         public static bool IsPhpArray(this AqValue value, /*[MaybeNullWhen(false)]*/out PhpArray? array) =>
//             (array = value.AsArray()) != null; // TODO: NETSTANDARD2.1
//
//         /// <summary>
//         /// Checks the value is of type <c>string</c> or <c>&amp;string</c> and gets its value.
//         /// Single-byte strings are decoded using <c>UTF-8</c>.
//         /// </summary>
//         public static bool IsString(this AqValue value, out string @string) => value.IsStringImpl(out @string);
//
//         /// <summary>
//         /// Checks the value is constructed as mutable <see cref="PhpString"/>.
//         /// UTF strings are not handled by this method.
//         /// </summary>
//         public static bool IsMutableString(this AqValue value, out PhpString @string) =>
//             value.IsMutableStringImpl(out @string);
//
//         /// <summary>
//         /// Checks the value is of type <c>string</c> (both unicode and single-byte) or an alias to a string.
//         /// </summary>
//         public static bool IsString(this AqValue value) => value.IsStringImpl();
//
//         /// <summary>
//         /// Gets value indicating the variable contains a single-byte string value.
//         /// </summary>
//         public static bool IsBinaryString(this AqValue value, out PhpString @string)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.MutableString:
//                     if (value.MutableStringBlob.ContainsBinaryData)
//                     {
//                         @string = value.MutableString;
//                         return true;
//                     }
//                     else
//                     {
//                         goto default;
//                     }
//
//                 case AquilaTypeCode.Alias:
//                     return value.Alias.Value.IsBinaryString(out @string);
//
//                 default:
//                     @string = default;
//                     return false;
//             }
//         }
//
//         /// <summary>
//         /// Gets value indicating the variable is Unicode string value.
//         /// </summary>
//         public static bool IsUnicodeString(this AqValue value, /*[MaybeNullWhen(false)]*/out string? @string)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.String:
//                     @string = value.String;
//                     return true;
//
//                 case AquilaTypeCode.MutableString:
//                     if (value.MutableStringBlob.ContainsBinaryData)
//                     {
//                         goto default;
//                     }
//                     else
//                     {
//                         @string = value.MutableStringBlob.ToString();
//                         return true;
//                     }
//
//                 case AquilaTypeCode.Alias:
//                     return value.Alias.Value.IsUnicodeString(out @string);
//
//                 default:
//                     @string = null;
//                     return false;
//             }
//         }
//
//         public static bool IsLong(this AqValue value, out long l)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.Long:
//                     l = value.Long;
//                     return true;
//
//                 case AquilaTypeCode.Alias:
//                     return IsLong(value.Alias.Value, out l);
//
//                 default:
//                     l = default;
//                     return false;
//             }
//         }
//
//         public static bool IsDouble(this AqValue value, out double d)
//         {
//             switch (value.TypeCode)
//             {
//                 case AquilaTypeCode.Double:
//                     d = value.Double;
//                     return true;
//                 case AquilaTypeCode.Alias:
//                     return IsDouble(value.Alias.Value, out d);
//                 default:
//                     d = default;
//                     return false;
//             }
//         }
//
//         /// <summary>
//         /// Checks the value is of type <c>bool</c> or <c>&amp;bool</c> and gets its value.
//         /// </summary>
//         public static bool IsBoolean(this AqValue value, out bool b) => value.IsBooleanImpl(out b);
//     }
// }

