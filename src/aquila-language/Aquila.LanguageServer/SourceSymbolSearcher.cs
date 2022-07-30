using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Utilities;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.LanguageServer
{
    internal class SourceSymbolSearcher : GraphExplorer<VoidStruct>
    {
        public class SymbolStat
        {
            public TextSpan Span { get; set; }
            public BoundExpression BoundExpression { get; set; }
            public ISymbol Symbol { get; set; }
            public TypeRefContext TypeCtx { get; }

            public SymbolStat(TypeRefContext tctx, TextSpan span, BoundExpression expr = null, ISymbol symbol = null)
            {
                this.TypeCtx = tctx;
                this.Span = span;
                this.BoundExpression = expr;
                this.Symbol = symbol;
            }
        }

        private int _position;

        private SymbolStat _result;

        AquilaCompilation DeclaringCompilation { get; set; }

        private SourceSymbolSearcher(int position)
        {
            _position = position;
        }

        public static SymbolStat SearchCFG(AquilaCompilation compilation, ControlFlowGraph cfg, int position)
        {
            var visitor = new SourceSymbolSearcher(position)
            {
                DeclaringCompilation = compilation,
            };
            visitor.VisitCFG(cfg);
            return visitor._result;
        }

        protected override void DefaultVisitUnexploredBlock(BoundBlock x)
        {
            if (_result == null)
            {
                base.DefaultVisitUnexploredBlock(x);
            }
        }
    }
}