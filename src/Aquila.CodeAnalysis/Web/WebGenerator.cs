using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Web;

internal class ComponentBaseGenerator
{
    private readonly SynthesizedNamespaceSymbol _webNs;
    private readonly SynthesizedManager _mrg;
    private readonly CoreTypes _ct;
    private readonly AquilaCompilation _compilation;
    private readonly PEModuleBuilder _m;

    public ComponentBaseGenerator(PEModuleBuilder builder)
    {
        _m = builder;
        _mrg = builder.SynthesizedManager;
        _webNs = _mrg.SynthesizeNamespace(builder.SourceModule.GlobalNamespace, "Web");

        _ct = builder.Compilation.CoreTypes;

        _compilation = builder.Compilation;
    }

    public void ConstructTypes()
    {
        var bType = _ct.Web_ComponentBase;

        var type = _mrg.SynthesizeType(_webNs, "Component")
                .SetBaseType(bType)
                .AddAttribute(new SynthesizedAttributeData(_ct.Web_Route.Ctor(_ct.String),
                    ImmutableArray.Create(new TypedConstant(_ct.String.Symbol,
                        TypedConstantKind.Primitive, "/view/library/invoice")),
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
            ;

        var thisPlace = new ThisArgPlace(type);

        var ctor = _mrg.SynthesizeConstructor(type)
            .SetMethodBuilder((m, d) => il =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, bType.Ctor());
                il.EmitRet(true);
            });

        type.AddMember(ctor);

        _m.SetMethodBody(ctor, ctor.CreateMethodBody(_m, DiagnosticBag.GetInstance()));
    }

    public void Metdod(MethodSymbol m)
    {
        var seq = 0;

        var op = _ct.Web_RenderTreeBuilder.Method("OpenComponent");
    }
}