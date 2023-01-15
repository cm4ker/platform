using Microsoft.CodeAnalysis;
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

        /// <summary>
        /// Checks whether given type may be callable.
        /// </summary>
        internal static bool IsCallable(TypeRefContext ctx)
        {
            return false;
        }
    }
}