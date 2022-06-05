using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;

namespace Aquila.CodeAnalysis.Web;

internal class ComponentBaseGenerator
{
    private readonly SynthesizedNamespaceSymbol _webNs;
    private readonly SynthesizedManager _mrg;
    private readonly CoreTypes _ct;

    public ComponentBaseGenerator(PEModuleBuilder builder)
    {
        _mrg = builder.SynthesizedManager;
        _webNs = _mrg.SynthesizeNamespace(builder.Compilation.GlobalNamespace, "Web");
        _ct = builder.Compilation.CoreTypes;
    }

    public void ConstructTypes()
    {
        var bType = _ct.Web_ComponentBase;
        var type = _mrg.SynthesizeType(_webNs, "Component");

        type.SetBaseType(_ct.Web_ComponentBase);
        var method = _mrg.SynthesizeMethod(type);
        method.SetOverride(bType.Method("BuildRenderTree", _ct.Web_RenderTreeBuilder));
    }

    public void Metdod(MethodSymbol m)
    {
        var seq = 0;

        var op = _ct.Web_RenderTreeBuilder.Method("OpenComponent");
    }
}