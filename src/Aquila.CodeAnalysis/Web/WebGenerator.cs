using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Web;

internal class ComponentBaseGenerator
{
    private readonly SynthesizedNamespaceSymbol _webNs;
    private readonly SynthesizedManager _mrg;
    private readonly CoreTypes _ct;
    private readonly AquilaCompilation _compilation;
    private readonly PEModuleBuilder _m;
    private readonly AquilaSyntaxTree _syntaxTree;

    public ComponentBaseGenerator(PEModuleBuilder builder, AquilaSyntaxTree syntaxTree)
    {
        _m = builder;
        _syntaxTree = syntaxTree;
        _mrg = builder.SynthesizedManager;
        _webNs = _mrg.SynthesizeNamespace(builder.SourceModule.GlobalNamespace, "Web");
        _ct = builder.Compilation.CoreTypes;
        _compilation = builder.Compilation;
    }

    public void Build()
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

        var renderMethod = _mrg.SynthesizeMethod(type);
        renderMethod
            .SetVirtual(true)
            .SetOverride(bType.Method("BuildRenderTree", _ct.Web_RenderTreeBuilder))
            .SetMethodBuilder((m, d) => il =>
            {
                var builderArg = new ParamPlace(renderMethod.Parameters.First());
                var v = new Visitor(type, _ct, builderArg, m, d, il);
                //v.AddMarkupContent(_syntaxTree);
                il.EmitRet(true);
            });

        var ctor = _mrg.SynthesizeConstructor(type)
            .SetMethodBuilder((m, d) => il =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, bType.Ctor());
                il.EmitRet(true);
            });

        type.AddMember(ctor);
        type.AddMember(renderMethod);

        _m.SetMethodBody(ctor, ctor.CreateMethodBody(_m, DiagnosticBag.GetInstance()));
        _m.SetMethodBody(renderMethod, renderMethod.CreateMethodBody(_m, DiagnosticBag.GetInstance()));
    }


    public void AddMarkupContent()
    {
        
    }
}

internal class Visitor : AquilaSyntaxVisitor
{
    private readonly TypeSymbol _componentType;
    private readonly CoreTypes _ct;
    private readonly ParamPlace _builder;
    private PEModuleBuilder m;
    private DiagnosticBag d;
    private ILBuilder il;
    private int _seq = 0;

    public Visitor(TypeSymbol componentType, CoreTypes ct, ParamPlace builder, PEModuleBuilder m, DiagnosticBag d,
        ILBuilder il)
    {
        _componentType = componentType;
        _ct = ct;
        _builder = builder;
        this.m = m;
        this.d = d;
        this.il = il;
    }
    
    public void OpenElement(string elementName)
    {
        _builder.EmitLoad(il);
        il.EmitIntConstant(_seq++);
        il.EmitStringConstant(elementName);
        var amc = _ct.Web_RenderTreeBuilder.Method("OpenElement", _ct.Int32, _ct.String);
        il.EmitCall(m, d, ILOpCode.Call, amc);
    }

    public void AddContent(string content)
    {
        _builder.EmitLoad(il);
        il.EmitIntConstant(_seq++);
        il.EmitStringConstant(content);
        var amc = _ct.Web_RenderTreeBuilder.Method("AddContent", _ct.Int32, _ct.String);
        il.EmitCall(m, d, ILOpCode.Call, amc);
    }

    public void AddContent(IPlace place)
    {
        _builder.EmitLoad(il);
        il.EmitIntConstant(_seq++);
        place.EmitLoad(il);
        var amc = _ct.Web_RenderTreeBuilder.Method("AddContent", _ct.Int32, _ct.Object);
        il.EmitCall(m, d, ILOpCode.Call, amc);
    }

    public void CloseElement()
    {
        _builder.EmitLoad(il);
        var amc = _ct.Web_RenderTreeBuilder.Method("CloseElement");
        il.EmitCall(m, d, ILOpCode.Call, amc);
    }

    public void AddAttribute(string name, string value)
    {
        _builder.EmitLoad(il);
        il.EmitIntConstant(_seq++);
        il.EmitStringConstant(name);
        il.EmitStringConstant(value);
        var amc = _ct.Web_RenderTreeBuilder.Method("AddAttribute", _ct.Int32, _ct.String, _ct.String);
        il.EmitCall(m, d, ILOpCode.Call, amc);
    }

    public void AddMarkupContent(string content)
    {
        _builder.EmitLoad(il);
        il.EmitIntConstant(_seq++);
        il.EmitStringConstant(content);
        var amc = _ct.Web_RenderTreeBuilder.Method("AddMarkupContent", _ct.Int32, _ct.String);
        il.EmitCall(m, d, ILOpCode.Call, amc);
    }
}