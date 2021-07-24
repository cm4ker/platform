﻿using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.TypeRef;

namespace Aquila.CodeAnalysis.FlowAnalysis
{
    /// <summary>
    /// Provides helper methods for working with types.
    /// </summary>
    internal static class TypeHelpers
    {
        public static readonly Func<IBoundTypeRef, bool> s_isarray = new Func<IBoundTypeRef, bool>(t => t.IsArray);
        public static readonly Func<IBoundTypeRef, bool> s_isnumber = new Func<IBoundTypeRef, bool>(IsNumber);
        public static readonly Func<IBoundTypeRef, bool> s_isobject = new Func<IBoundTypeRef, bool>(t => t.IsObject);

        /// <summary>
        /// Determines if given <see cref="IBoundTypeRef"/> represents a number (integral or real).
        /// </summary>
        public static bool IsNumber(this IBoundTypeRef tref) => tref is BoundPrimitiveTypeRef pt && pt.IsNumber;

        ///// <summary>
        ///// Gets value determining whether <paramref name="totype"/> type can be assigned from <paramref name="fromtype"/>.
        ///// </summary>
        ///// <param name="totype">Type mask which we check whether is assignable from <paramref name="fromtype"/>.</param>
        ///// <param name="fromtype">Type mask we check whether is equal or is a subclass of <paramref name="totype"/>.</param>
        ///// <param name="ctx">Type context for resolving class names from type mask.</param>
        ///// <param name="model">Helper object which caches class inheritance.</param>
        ///// <remarks>
        ///// Gets <c>true</c>, if <paramref name="totype"/> is equal to or is a base type of <paramref name="fromtype"/>.
        ///// Gets <c>False</c> for <c>void</c> type masks.
        ///// </remarks>
        //public static bool IsAssignableFrom(this TypeRefMask totype, TypeRefMask fromtype, TypeRefContext ctx, ISemanticModel model)
        //{
        //    Debug.Assert(ctx != null);
        //    Debug.Assert(model != null);

        //    if ((totype & fromtype & ~(ulong)TypeRefMask.FlagsMask) != 0)
        //        return true;    // types are equal (or at least one of them is Any Type)

        //    // object <-> unspecified object instance
        //    if ((ctx.IsObject(totype) && ctx.IsObject(fromtype)) &&
        //        (ctx.IsAnObject(totype) || ctx.IsAnObject(fromtype)))
        //        return true;

        //    if (IsImplicitConversion(fromtype, totype, ctx, model))
        //        return true;

        //    // cut off object types (primitive types do not have subclasses)
        //    var selfObjs = ctx.GetObjectTypes(totype);
        //    if (selfObjs.Count == 0)
        //        return false;  // self mask does not represent a class

        //    var typeObjs = ctx.GetObjectTypes(fromtype);
        //    if (typeObjs.Count == 0)
        //        return false;  // type mask does not represent a class

        //    // build inheritance graph and check whether any type from self is a base of anything in type
        //    if (selfObjs.Count == 1 && typeObjs.Count == 1)
        //        return model.IsAssignableFrom(selfObjs[0].QualifiedName, typeObjs[0].QualifiedName);

        //    //
        //    return model.IsAssignableFrom(selfObjs.Select(t => t.QualifiedName), typeObjs.Select(t => t.QualifiedName));
        //}

        ///// <summary>
        ///// Determines whether there is an implicit conversion from one type to another.
        ///// </summary>
        //private static bool IsImplicitConversion(TypeRefMask fromtype, TypeRefMask totype, TypeRefContext ctx, ISemanticModel model)
        //{
        //    // TODO: optimize bit operations

        //    //
        //    if (ctx.IsArray(totype) && ctx.IsArray(fromtype))
        //    {
        //        // both types are arrays, type may be more specific (int[]) and self just ([])

        //        // TODO: check whether their element types are assignable,
        //        // avoid infinite recursion!

        //        return true;
        //    }

        //    // any callable -> "callable"
        //    if (ctx.IsLambda(totype) && IsCallable(fromtype, ctx, model))
        //        return true;

        //    //// allowed conversions

        //    // int <-> bool            
        //    //if (ctx.IsInteger(totype) && ctx.IsBoolean(fromtype))   // int -> bool
        //    //    return true;

        //    // int <-> double
        //    if (ctx.IsNumber(fromtype) && ctx.IsNumber(totype)) // TODO: maybe settings for strict number type check
        //        return true;

        //    if (ctx.IsString(totype) && IsConversionToString(fromtype, ctx, model))
        //        return true;

        //    //
        //    if (ctx.IsNull(fromtype) && ctx.IsNullable(totype))
        //        return true;    // NULL can be assigned to any nullable

        //    //
        //    return false;
        //}

        ///// <summary>
        ///// Determines whether given type can be converted to string without warning.
        ///// </summary>
        //internal static bool IsConversionToString(TypeRefMask fromtype, TypeRefContext ctx, ISemanticModel model)
        //{
        //    // primitive -> string
        //    if (ctx.IsPrimitiveType(fromtype))
        //        return true;

        //    // object with __toString() -> string
        //    if (ctx.IsObject(fromtype) && ctx.GetObjectTypes(fromtype).Any(tref => model.GetClass(tref.QualifiedName).HasMethod(NameUtils.SpecialNames.__toString, model)))
        //        return true;

        //    //
        //    return false;
        //}

        /// <summary>
        /// Checks whether given type may be callable.
        /// </summary>
        internal static bool IsCallable(TypeRefContext ctx)
        {
            return false;
        }

        ///// <summary>
        ///// Gets value indicating whether specified object of type <paramref name="type"/> can be used as <c>foreach</c> enumerable variable.
        ///// </summary>
        ///// <param name="type">Type of the object.</param>
        ///// <param name="ctx">Type context.</param>
        ///// <param name="model">Type graph.</param>
        ///// <returns>True iff <paramref name="type"/> is allowed as <c>foreach</c> enumerator.</returns>
        //internal static bool IsTraversable(TypeRefMask type, TypeRefContext ctx, ISemanticModel model)
        //{
        //    //
        //    if (type.IsAnyType || type.IsVoid || ctx.IsArray(type))
        //        return true;

        //    // object implementing Traversable
        //    if (ctx.IsObject(type))
        //    {
        //        var types = ctx.GetObjectTypes(type);
        //        foreach (var t in types)
        //            if (model.IsAssignableFrom(NameUtils.SpecialNames.Traversable, t.QualifiedName))
        //                return true;
        //    }

        //    //
        //    return false;
        //}
    }
}