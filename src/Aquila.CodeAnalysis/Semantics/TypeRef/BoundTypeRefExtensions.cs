using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using MoreLinq.Extensions;

namespace Aquila.CodeAnalysis.Semantics
{
    internal static class BoundTypeRefExtensions
    {
        public static ITypeSymbol
            EmitLoadTypeInfo(this IBoundTypeRef tref, CodeGenerator cg, bool throwOnError = false) =>
            ((BoundTypeRef) tref).EmitLoadTypeInfo(cg, throwOnError);

        public static void EmitClassName(this IBoundTypeRef tref, CodeGenerator cg)
        {
            if (tref is BoundIndirectTypeRef it)
            {
                it.EmitClassName(cg);
            }
            else if (tref is BoundClassTypeRef ct)
            {
                cg.Builder.EmitStringConstant(ct.ClassName.ToString());
            }
            else
            {
                // Template: {LOAD PhpTypeInfo}.Name
                tref.EmitLoadTypeInfo(cg, true);
                cg.EmitCall(ILOpCode.Call, null)
                    .Expect(SpecialType.System_String);
            }
        }

        /// <summary>
        /// Gets <see cref="TypeSymbol"/> suitable to be used for the runtime operations.
        /// Does not return <c>null</c> nor <see cref="ErrorTypeSymbol"/>.
        /// </summary>
        public static TypeSymbol ResolveRuntimeType(this IBoundTypeRef tref, AquilaCompilation compilation)
        {
            var boundType = (BoundTypeRef) tref;

            var t = boundType.ResolvedType ?? (TypeSymbol) boundType.ResolveTypeSymbol(compilation);

            if (t.IsErrorTypeOrNull()) // error type => class could not be found
            {
                if (t is AmbiguousErrorTypeSymbol ambiguous)
                {
                    // ambiguity -> try to find a common base
                    var common = compilation.FindCommonBase(ambiguous._candidates);
                    if (common.IsValidType())
                    {
                        return common;
                    }
                }

                t = compilation.CoreTypes.Object.Symbol;
            }

            return t;
        }

        /// <summary>
        /// Gets value indicating the <paramref name="tref"/> represents <c>self</c> keyword.
        /// </summary>
        public static bool IsSelf(this IBoundTypeRef tref) => false;

        /// <summary>
        /// Gets value indicating the <paramref name="tref"/> represents <c>parent</c> keyword.
        /// </summary>
        public static bool IsParent(this IBoundTypeRef tref) => false;

        /// <summary>
        /// Gets value indicating the <paramref name="tref"/> represents <c>static</c> keyword.
        /// </summary>
        public static bool IsStatic(this IBoundTypeRef tref) => false;
    }
}