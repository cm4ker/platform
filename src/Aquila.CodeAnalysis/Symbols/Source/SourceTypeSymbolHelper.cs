using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Source;

public static class SourceTypeSymbolHelper
{
    /// <summary>
    /// Add members for injecting context to the instance type 
    /// </summary>
    /// <param name="typeSymbol">owned symbol</param>
    /// <param name="builder">array builder for members</param>
    internal static void AddDefaultInstanceTypeSymbolMembers(NamedTypeSymbol typeSymbol,
        ImmutableArray<Symbol>.Builder builder)
    {
        Debug.Assert(!typeSymbol.IsInterface && !typeSymbol.IsAbstract);
        
        var ctxField = new SynthesizedFieldSymbol(typeSymbol)
            .SetName(SpecialParameterSymbol.ContextName)
            .SetAccess(Accessibility.Private)
            .SetType(typeSymbol.DeclaringCompilation.CoreTypes.AqContext);

        builder.Add(ctxField);
        var ctor = new SynthesizedCtorSymbol(typeSymbol);
        var ctxParam = new SpecialParameterSymbol(ctor, typeSymbol.DeclaringCompilation.CoreTypes.AqContext,
            SpecialParameterSymbol.ContextName, 0);

        var thisPlace = new ThisArgPlace(typeSymbol);
        var ctxPS = new ParamPlace(ctxParam);
        var ctxFieldPlace = new FieldPlace(ctxField);

        ctor.SetParameters(ctxParam)
            .SetMethodBuilder((m, db) =>
            {
                return il =>
                {
                    thisPlace.EmitLoad(il);
                    il.EmitCall(m, db, ILOpCode.Call, typeSymbol.BaseType.Ctor());

                    thisPlace.EmitLoad(il);
                    ctxPS.EmitLoad(il);
                    ctxFieldPlace.EmitStore(il);

                    il.EmitRet(true);
                };
            });

        builder.Add(ctor);
    }
}