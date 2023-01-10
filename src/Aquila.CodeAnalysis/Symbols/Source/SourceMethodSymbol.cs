using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Symbols;

internal sealed class SourceMethodSymbol : SourceMethodSymbolBase
{
     
    readonly FuncDecl _syntax;

    public SourceMethodSymbol(NamedTypeSymbol type, FuncDecl syntax): base(type)
    {

        Contract.ThrowIfNull(syntax);

        _syntax = syntax;
    }
        
    TextSpan NameSpan => _syntax.Identifier.Span;
        
    internal override ParameterListSyntax SyntaxSignature => _syntax.ParameterList;

    internal override  TypeEx SyntaxReturnType => _syntax.ReturnType;

    internal override AquilaSyntaxNode Syntax => _syntax;

    internal override IList<StmtSyntax> Statements => _syntax.Body?.Statements.ToList();
        
    public override void GetDiagnostics(DiagnosticBag diagnostic)
    {
            
    }

    public override string Name => _syntax.Identifier.Text;
        
    public override TypeSymbol ReturnType
    {
        get
        {
            if (SyntaxReturnType == null)
                return DeclaringCompilation.GetSpecialType(SpecialType.System_Void);

            return DeclaringCompilation.GetBinder(_syntax).BindType(SyntaxReturnType);
        }
    }

    public override bool IsStatic =>
        ContainingSymbol is SourceModuleTypeSymbol || _syntax.GetModifiers().IsStatic();
        
    /// <summary>
    /// Lazily bound semantic block.
    /// Entry point of analysis and emitting.
    /// </summary>
    public override ControlFlowGraph ControlFlowGraph
    {
        get
        {
            if (_cfg == null && this.Statements != null) // ~ Statements => non abstract method
            {
                // create initial flow state
                var state = StateBinder.CreateInitialState(this);

                var binder = DeclaringCompilation.GetBinder(_syntax);

                // build control flow graph
                var cfg = new ControlFlowGraph(this.Statements, binder);
                cfg.Start.FlowState = state;

                //
                Interlocked.CompareExchange(ref _cfg, cfg, null);
            }

            //
            return _cfg;
        }
        internal set { _cfg = value; }
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