// using System;
// using Microsoft.VisualBasic.CompilerServices;
//
// namespace Aquila.Core
// {
//     /// <summary>
//     /// Conversion routines for <see cref="AqValue"/>.
//     /// </summary>
//     public static class AqValueConverter
//     {
//         static class GenericConverter<T>
//         {
//             //public static readonly Func<T, PhpValue> s_toPhpValue = (Func<T, PhpValue>)Create_T_to_PhpValue();
//             public static readonly Func<AqValue, T> s_fromPhpValue = (Func<AqValue, T>)Create_PhpValue_to_T();
//
//             //static Delegate Create_T_to_PhpValue()
//             //{
//             //    var type = typeof(T);
//
//             //    if (type == typeof(PhpValue)) return Utilities.FuncExtensions.Identity<PhpValue>();
//             //    if (type == typeof(int)) return new Func<int, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(long)) return new Func<long, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(bool)) return new Func<bool, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(double)) return new Func<double, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(float)) return new Func<float, PhpValue>(x => PhpValue.Create((double)x));
//             //    if (type == typeof(string)) return new Func<string, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(uint)) return new Func<uint, PhpValue>(x => PhpValue.Create((long)x));
//             //    if (type == typeof(byte[])) return new Func<byte[], PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(PhpNumber)) return new Func<PhpNumber, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(PhpArray)) return new Func<PhpArray, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(PhpString)) return new Func<PhpString, PhpValue>(x => PhpValue.Create(x));
//             //    if (type == typeof(PhpAlias)) return new Func<PhpAlias, PhpValue>(x => PhpValue.Create(x));
//             //    //if (type == typeof(object)) return new Func<object, PhpValue>(x => PhpValue.FromClass(x));
//             //    throw new NotImplementedException();
//             //}
//
//             static Delegate Create_PhpValue_to_T()
//             {
//                 if (typeof(T) == typeof(AqValue)) return Utilities.FuncExtensions.Identity<AqValue>();
//                 if (typeof(T) == typeof(PhpAlias)) return new Func<AqValue, PhpAlias>(x => x.AsPhpAlias());
//                 if (typeof(T) == typeof(string)) return new Func<AqValue, string>(x => x.ToString());
//                 if (typeof(T) == typeof(double)) return new Func<AqValue, double>(x => x.ToDouble());
//                 if (typeof(T) == typeof(float)) return new Func<AqValue, float>(x => (float)x.ToDouble());
//                 if (typeof(T) == typeof(long)) return new Func<AqValue, long>(x => x.ToLong());
//                 if (typeof(T) == typeof(int)) return new Func<AqValue, int>(x => (int)x.ToLong());
//                 if (typeof(T) == typeof(bool)) return new Func<AqValue, bool>(x => x.ToBoolean());
//                 if (typeof(T) == typeof(PhpArray)) return new Func<AqValue, PhpArray>(x => x.ToArray());
//                 if (typeof(T) == typeof(byte)) return new Func<AqValue, byte>(x => (byte)x.ToLong());
//                 if (typeof(T) == typeof(char)) return new Func<AqValue, char>(x => Convert.ToChar(x));
//                 if (typeof(T) == typeof(uint)) return new Func<AqValue, uint>(x => (uint)x.ToLong());
//                 if (typeof(T) == typeof(ulong)) return new Func<AqValue, ulong>(x => (ulong)x.ToLong());
//                 if (typeof(T) == typeof(DateTime)) return new Func<AqValue, DateTime>(Convert.ToDateTime);
//                 if (typeof(T) == typeof(byte[]))
//                     return new Func<AqValue, byte[]>(x => x.ToBytesOrNull() ?? throw new InvalidCastException());
//
//                 if (typeof(T).IsValueType)
//                 {
//                     if (typeof(T).IsGenericType)
//                     {
//                         // Nullable<U>
//                         if (typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
//                         {
//                             var typeU = typeof(T).GenericTypeArguments[0];
//                             if (typeU == typeof(bool))
//                                 return new Func<AqValue, Nullable<bool>>(x =>
//                                     Operators.IsSet(x) ? (bool?)x.ToBoolean() : null);
//                             if (typeU == typeof(int))
//                                 return new Func<AqValue, Nullable<int>>(x =>
//                                     Operators.IsSet(x) ? (int?)(int)x.ToLong() : null);
//                             if (typeU == typeof(long))
//                                 return new Func<AqValue, Nullable<long>>(x =>
//                                     Operators.IsSet(x) ? (long?)x.ToLong() : null);
//                             if (typeU == typeof(double))
//                                 return new Func<AqValue, Nullable<double>>(x =>
//                                     Operators.IsSet(x) ? (double?)x.ToDouble() : null);
//                         }
//                     }
//                 }
//                 else // type.IsReferenceType
//                 {
//                     // Delegate
//                     if (typeof(T).BaseType == typeof(MulticastDelegate))
//                     {
//                         // Error: needs Context
//                         throw new ArgumentException();
//                     }
//
//                     // Object
//                     return new Func<AqValue, T>(x => (T)x.AsObject());
//                 }
//
//                 throw new NotImplementedException();
//             }
//         }
//
//         //static class GenericDelegate<TDelegate> where TDelegate : MulticastDelegate
//         //{
//         //    // TResult F(Tuple<PhpValue, Context>, arg1, ..., argN) => value.AsCallable().Invoke(Context, new[]{ arg1, ..., argN })
//         //    public static readonly MethodInfo/*!*/s_func = CreateFunc();
//
//         //    static MethodInfo CreateFunc()
//         //    {
//
//         //    }
//         //}
//
//         ///// <summary>
//         ///// Converts given value to <see cref="PhpValue"/>.
//         ///// </summary>
//         ///// <typeparam name="T">Source CLR type.</typeparam>
//         ///// <param name="value">Value to be converted.</param>
//         ///// <returns>Value converted to <see cref="PhpValue"/>.</returns>
//         //public static PhpValue ToPhpValue<T>(this T value) => GenericConverter<T>.s_toPhpValue(value);
//
//         /// <summary>
//         /// Casts <see cref="AqValue"/> to a given type <typeparamref name="T"/>.
//         /// Throws an exception if cast is not possible.
//         /// </summary>
//         /// <typeparam name="T">Conversion target.</typeparam>
//         /// <param name="value">Value to be converted.</param>
//         /// <returns>Value as <typeparamref name="T"/>.</returns>
//         public static T Cast<T>(this AqValue value) => GenericConverter<T>.s_fromPhpValue(value);
//
//         //public static TDelegate CreateDelegate<TDelegate>(this PhpValue value, Context ctx) where TDelegate : MulticastDelegate
//         //{
//         //    /*
//         //     * static TResult F(Tuple<PhpValue, Context>, arg1, ..., argN) => value.AsCallable().Invoke(Context, new[]{ arg1, ..., argN })
//         //     * return Delegate.CreateDelegate(typeof(TDelegate), Tuple<PhpValue, Context>, F)
//         //     */
//
//         //    return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), new Tuple<PhpValue, Context>(value, ctx), GenericDelegate<TDelegate>.s_func);
//         //}
//
//         ///// <summary>
//         ///// Casts <see cref="PhpValue"/> to a given type <paramref name="target"/>.
//         ///// Throws an exception if cast is not possible.
//         ///// </summary>
//         ///// <param name="value">Value to be converted.</param>
//         ///// <param name="target">Target CLR type. Cannot be <c>null</c>.</param>
//         ///// <returns>Value.</returns>
//         //public static object ToClr(this PhpValue value, Type target)
//         //{
//         //    if (value.IsNull)
//         //    {
//         //        return null;
//         //    }
//         //}
//     }
// }