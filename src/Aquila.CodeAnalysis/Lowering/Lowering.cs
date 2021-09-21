using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Lowering
{
    internal class LocalRewriter : GraphRewriter
    {
        private readonly SourceMethodSymbol _method;

        protected AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;
        protected PrimitiveBoundTypeRefs PrimitiveBoundTypeRefs => DeclaringCompilation.TypeRefs;

        private CoreTypes _ct;
        private CoreMethods _cm;


        private LocalRewriter()
        {
        }

        private LocalRewriter(SourceMethodSymbol method)
            : this()
        {
            _method = method;
            _ct = DeclaringCompilation.CoreTypes;
            _cm = DeclaringCompilation.CoreMethods;
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
            var updatedCFG = (ControlFlowGraph)rewriter.VisitCFG(currentCFG);

            method.ControlFlowGraph = updatedCFG;

            return updatedCFG != currentCFG;
        }

        public override object VisitBinaryEx(BoundBinaryEx x)
        {
            if (x.Left.Type.SpecialType == SpecialType.System_String &&
                x.Right.Type.SpecialType == SpecialType.System_String)
            {
                //TODO: more advanced logic for Expr + Expr + Expr + ...etc
                var transLeft = BoundArgument.Create((BoundExpression)VisitExpression(x.Left));
                var transRight = BoundArgument.Create((BoundExpression)VisitExpression(x.Right));

                var args = new[] { transLeft, transRight }.ToImmutableArray();
                var typeArgs = ImmutableArray<ITypeSymbol>.Empty;

                return new BoundStaticCallEx(_cm.Operators.Concat_String_String, null, args, typeArgs,
                        _ct.String.Symbol)
                    .WithAccess(x);
            }

            return base.VisitBinaryEx(x);
        }

        public override object VisitMatchEx(BoundMatchEx x)
        {
            var updated = (BoundMatchEx)base.VisitMatchEx(x);

            var updatedArms = new List<BoundMatchArm>();

            foreach (var arm in x.Arms)
            {
                if (arm.Pattern is BoundWildcardEx)
                    updatedArms.Add(arm);
                else
                {
                    var pattern = new BoundBinaryEx(updated.Expression, arm.Pattern, Operations.Equal,
                        _ct.Boolean.Symbol);
                    updatedArms.Add(arm.Update(pattern, arm.WhenGuard, arm.MatchResult, arm.ResultType));
                }
            }

            return updated.Update(updated.Expression, updatedArms, updated.ResultType);
        }
    }
}