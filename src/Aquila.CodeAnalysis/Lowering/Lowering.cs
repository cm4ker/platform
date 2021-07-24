using System.Collections.Immutable;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Semantics.Graph;

namespace Aquila.CodeAnalysis.Lowering
{
    internal class LocalRewriter : GraphRewriter
    {
        private readonly SourceMethodSymbol _method;

        protected AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;
        protected BoundTypeRefFactory BoundTypeRefFactory => DeclaringCompilation.TypeRefFactory;

        private LocalRewriter()
        {
        }

        private LocalRewriter(SourceMethodSymbol method)
            : this()
        {
            _method = method;
        }

        public static bool TryTransform(SourceMethodSymbol method)
        {
            if (method.ControlFlowGraph == null)
            {
                // abstract method
                return false;
            }

            var rewriter = new LocalRewriter(method);
            var currentCFG = method.ControlFlowGraph;
            var updatedCFG = (ControlFlowGraph) rewriter.VisitCFG(currentCFG);

            method.ControlFlowGraph = updatedCFG;

            return updatedCFG != currentCFG;
        }

        public override object VisitBinaryEx(BoundBinaryEx x)
        {
            var typeCtx = _method.TypeRefContext;

            // if (typeCtx.IsAString(x.Left) && typeCtx.IsAString(x.Right.TypeRefMask))
            // {
            //     return new BoundStaticCallEx(_method.DeclaringCompilation.CoreMethods.Operators.Concat_String_String,
            //             new BoundMethodName(new QualifiedName(new Name(nameof(string.Concat)))), new[]
            //             {
            //                 BoundArgument.Create(x.Left), BoundArgument.Create(x.Right)
            //             }.ToImmutableArray(), ImmutableArray<IBoundTypeRef>.Empty)
            //         .WithAccess(x);
            // }

            //
            return base.VisitBinaryEx(x);
        }
    }
}