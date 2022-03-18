using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Public;

internal static class SynthExtensions
{
    public static void CreatePropertyWithBackingField(this PlatformSymbolCollection ps, SynthesizedTypeSymbol type,
        TypeSymbol propType, string name)
    {
        var backingField = ps.SynthesizeField(type)
            .SetName($"<{name}>k_BackingField")
            .SetAccess(Accessibility.Private)
            .SetType(propType);

        //args and fields
        var field = new FieldPlace(backingField);
        var thisArg = new ArgPlace(type, 0);

        var getter = ps.SynthesizeMethod(type)
            .SetName($"get_{name}")
            .SetAccess(Accessibility.Public)
            .SetReturn(propType)
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisArg.EmitLoad(il);
                field.EmitLoad(il);

                //TODO: this hack. initialize possible null fields in ctor
                if (field.Type.SpecialType == SpecialType.System_String)
                {
                    var elseLabel = new NamedLabel("<else>");

                    il.EmitOpCode(ILOpCode.Ldnull);
                    il.EmitOpCode(ILOpCode.Ceq);
                    il.EmitBranch(ILOpCode.Brfalse, elseLabel);
                    il.EmitStringConstant("");
                    il.EmitRet(false);

                    il.MarkLabel(elseLabel);

                    thisArg.EmitLoad(il);
                    field.EmitLoad(il);
                }

                il.EmitRet(false);
            });

        var setter = ps.SynthesizeMethod(type)
            .SetName($"set_{name}")
            .SetAccess(Accessibility.Public);

        var param = new SynthesizedParameterSymbol(setter, propType, 0, RefKind.None);
        setter.SetParameters(param);

        var paramPlace = new ParamPlace(param);

        setter
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisArg.EmitLoad(il);
                paramPlace.EmitLoad(il);

                field.EmitStore(il);
                il.EmitRet(true);
            });

        var peProp = ps.SynthesizeProperty(type);
        peProp
            .SetName(name)
            .SetGetMethod(getter)
            .SetSetMethod(setter)
            .SetType(propType);

        type.AddMember(backingField);
        type.AddMember(getter);
        type.AddMember(setter);
        type.AddMember(peProp);
    }
}