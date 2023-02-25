using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols;

internal sealed class SourceMethodSymbol : SourceMethodSymbolBase
{
    readonly FuncDecl _syntax;

    public SourceMethodSymbol(Symbol containingSymbol, FuncDecl syntax) : base(containingSymbol)
    {
        Contract.ThrowIfNull(syntax);
        _syntax = syntax;
    }

    internal override ParameterListSyntax SyntaxSignature => _syntax.ParameterList;

    internal override TypeEx SyntaxReturnType => _syntax.ReturnType;

    internal override AquilaSyntaxNode Syntax => _syntax;

    internal override IEnumerable<StmtSyntax> Statements => _syntax.Body?.Statements;

    public override void GetDiagnostics(DiagnosticBag diagnostic)
    {
    }

    public override string Name => _syntax.Identifier.Text;

    public override bool IsStatic =>
        ContainingSymbol is SourceModuleTypeSymbol || _syntax.GetModifiers().IsStatic();

    protected override Binder GetMethodBinderCore()
    {
        return DeclaringCompilation.GetBinder(_syntax);
    }

    public override Accessibility DeclaredAccessibility
    {
        get
        {
            var accessibility = _syntax.GetModifiers().GetAccessibility();

            if (_syntax.IsGlobal)
                accessibility = Accessibility.Public;

            return accessibility;
        }
    }

    public override ImmutableArray<AttributeData> GetAttributes()
    {
        var builder = ImmutableArray.CreateBuilder<AttributeData>();
        builder.AddRange(base.GetAttributes());

        foreach (var attrList in _syntax.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var type = (NamedTypeSymbol)DeclaringCompilation.GetBinder(_syntax).BindType(attr.Name);
                builder.Add(new SourceAttributeData(null, type, type.Ctor(), false));
            }
        }


        return builder.ToImmutable();
    }
}