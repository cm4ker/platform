using System.Collections.Immutable;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Semantics.Graph;

namespace Aquila.CodeAnalysis.Lowering
{
    internal class LocalRewriter : GraphRewriter
    {
        private readonly SourceRoutineSymbol _routine;

        protected AquilaCompilation DeclaringCompilation => _routine.DeclaringCompilation;
        protected BoundTypeRefFactory BoundTypeRefFactory => DeclaringCompilation.TypeRefFactory;

        private LocalRewriter()
        {
        }

        private LocalRewriter(SourceRoutineSymbol routine)
            : this()
        {
            _routine = routine;
        }

        public static bool TryTransform(SourceRoutineSymbol routine)
        {
            if (routine.ControlFlowGraph == null)
            {
                // abstract method
                return false;
            }

            var rewriter = new LocalRewriter(routine);
            var currentCFG = routine.ControlFlowGraph;
            var updatedCFG = (ControlFlowGraph) rewriter.VisitCFG(currentCFG);

            routine.ControlFlowGraph = updatedCFG;

            return updatedCFG != currentCFG;
        }

        public override object VisitBinaryEx(BoundBinaryEx x)
        {
            var typeCtx = _routine.TypeRefContext;

            if (typeCtx.IsAString(x.Left.TypeRefMask) && typeCtx.IsAString(x.Right.TypeRefMask))
            {
                return new BoundStaticCallEx(_routine.DeclaringCompilation.CoreMethods.Operators.Concat_String_String,
                        new BoundRoutineName(new QualifiedName(new Name(nameof(string.Concat)))), new[]
                        {
                            BoundArgument.Create(x.Left), BoundArgument.Create(x.Right)
                        }.ToImmutableArray(), ImmutableArray<IBoundTypeRef>.Empty)
                    .WithAccess(x);
            }

            //
            return base.VisitBinaryEx(x);
        }
    }
}