// using Aquila.Syntax.Syntax;
// using Aquila.CodeAnalysis.Utilities;
//
// namespace Aquila.CodeAnalysis.FlowAnalysis
// {
//     /// <summary>
//     /// Used by analysis of method in case we know additional information about called context.
//     /// </summary>
//     public struct CallInfo
//     {
//         #region Fields
//
//         /// <summary>
//         /// Context of type references used in call info.
//         /// </summary>
//         private readonly TypeRefContext _typeCtx;
//
//         ///// <summary>
//         ///// Known parameters value.
//         ///// </summary>
//         //public readonly object[] _paramsValue;
//
//         #endregion
//
//         #region Construction
//
//         /// <summary>
//         /// Initializes <see cref="CallInfo"/>.
//         /// </summary>
//         /// <param name="ctx">Type context of the caller.</param>
//         /// <param name="paramsCount">Amount of parameters used for the call.</param>
//         public CallInfo(TypeRefContext ctx, int paramsCount)
//             : this(ctx, paramsCount)
//         {
//         }
//
//         // private static TypeRefMask[] GetParamsTypeArr(int count, TypeRefMask value)
//         // {
//         //     if (count < 0)
//         //         return null;
//         //
//         //     if (count == 0)
//         //         return EmptyArray<TypeRefMask>.Instance;
//         //
//         //     //
//         //     var arr = new TypeRefMask[count];
//         //     for (int i = 0; i < arr.Length; i++)
//         //         arr[i] = value;
//         //
//         //     return arr;
//         // }
//
//         #endregion
//
//         /// <summary>
//         /// Gets known parameters count. If call info is empty, the method gets <c>-1</c>.
//         /// </summary>
//         public int ParametersCount
//         {
//             get { return (_paramsType != null) ? _paramsType.Length : -1; }
//         }
//
//         /// <summary>
//         /// Gets actual parameter type if provided. Otherwise <c>void</c>.
//         /// </summary>
//         /// <param name="ctx">Target type context.</param>
//         /// <param name="index">Index of parameter.</param>
//         /// <returns>Type mask of the parameter or <c>void</c>.</returns>
//         public TypeRefMask GetParamType(TypeRefContext ctx, int index)
//         {
//             if (ctx == null) throw ExceptionUtilities.ArgumentNull(nameof(ctx));
//
//             if (_typeCtx != null && index >= 0 && index < _paramsType.Length)
//                 return ctx.AddToContext(_typeCtx, _paramsType[index]);
//
//             return 0;
//         }
//
//         /// <summary>
//         /// Gets actual lates static bind type (type of <c>static</c>) if provided.
//         /// Otherwise <c>void</c>.
//         /// </summary>
//         /// <param name="ctx">Target type context.</param>
//         /// <returns>TYpe mask of <c>static</c> in given context or <c>void</c>.</returns>
//         public TypeRefMask GetLateStaticBindType(TypeRefContext ctx)
//         {
//             if (ctx == null) throw ExceptionUtilities.ArgumentNull("ctx");
//
//             if (_typeCtx != null && !_lateStaticBindType.IsUninitialized)
//                 return ctx.AddToContext(_typeCtx, _lateStaticBindType);
//
//             return 0;
//         }
//
//         ///// <summary>
//         ///// Gets actual parameter value if known. Otherwise <see cref="Helpers.ExpressionValue.UnknownValue"/>.
//         ///// </summary>
//         //public object GetParamValue(int index)
//         //{
//         //    // if (index >= 0 && index < _paramValues.Length) return _paramValues[index];
//
//         //    return Helpers.ExpressionValue.UnknownValue;
//         //}
//     }
// }