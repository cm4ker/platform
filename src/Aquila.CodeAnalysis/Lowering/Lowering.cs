using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax.Syntax;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Syntax;
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

        private int _tmpVariableIndex = 0;
        protected string NextTmpVariableName() => "<tmp>'" + _tmpVariableIndex++;

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

        public override object VisitAllocEx(BoundAllocEx x)
        {
            /*
             unfold the AllocEx like 
             
             (Type) 
             {
                Field1 = (BoundEx)
                Field2 = (BoundEx)
                ...
                etc
             }
             
             into >>> 
             
             BoundGroupedEx
             {
                 var tmp = new Type();
                 (AssignEx) tmp.Field1 = (BoundEx)
                 (AssignEx) tmp.Field2 = (BoundEx)
                 ...
                 etc
                 
                 return tmp; // finally resulting expression is tmp
             }             
             */
            var type = (NamedTypeSymbol)x.TypeRef;
            var ctor = type.InstanceConstructors.FirstOrDefault(x => x.ParameterCount == 0);

            var exprs = new List<BoundExpression>();

            var instance = new BoundNewEx(ctor, type, ImmutableArray<BoundArgument>.Empty,
                ImmutableArray<ITypeSymbol>.Empty, type);

            var name = new VariableName(NextTmpVariableName());
            var variable = _method.LocalsTable.BindTemporalVariable(name, type);

            //NOTE: we read value from the variable => we need read access
            //TODO: maybe ExprAnalyses make this for us. Puts marks for emitter - not a great work (run it after transformation)
            var varRef = new BoundVariableRef(name.Value, type).WithAccess(BoundAccess.Read);
            varRef.Variable = variable;

            var assign = new BoundAssignEx(varRef, instance);

            exprs.Add(assign);

            if (x.Initializer.Any())
            {
                foreach (var item in x.Initializer)
                {
                    var sym = item.ReceiverSymbol;
                    BoundReferenceEx target = sym.Kind switch
                    {
                        SymbolKind.Field => new BoundFieldRef((IFieldSymbol)sym, varRef),
                        SymbolKind.Property => new BoundPropertyRef((IPropertySymbol)sym, varRef),
                        _ => throw new NotImplementedException("this symbol kind is not implemented")
                    };

                    exprs.Add(new BoundAssignEx(target.WithAccess(BoundAccess.Write),
                        item.Expression.WithAccess(BoundAccess.Read)).WithAccess(BoundAccess.None));
                }
            }

            exprs.Add(varRef);

            return new BoundGroupedEx(exprs, type);
        }


        public override object VisitMatchEx(BoundMatchEx x)
        {
            //TODO: maybe transform it into the more simple operations?
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