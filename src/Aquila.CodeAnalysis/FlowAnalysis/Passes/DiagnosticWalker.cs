using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.Syntax.Ast;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    internal partial class DiagnosticWalker<T> : GraphExplorer<T>
    {
        private readonly DiagnosticBag _diagnostics;
        private SourceMethodSymbolBase _method;

        private bool CallsParentCtor { get; set; }

        AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;

        void CheckMissusedPrimitiveType(IBoundTypeRef tref)
        {
            if (tref.IsPrimitiveType)
            {
                // error: use of primitive type {0} is misused // primitive type does not make any sense in this context
                _diagnostics.Add(_method, tref.AquilaSyntax, ErrorCode.ERR_PrimitiveTypeNameMisused, tref);
            }
        }

        void CannotInstantiate(IAquilaOperation op, string kind, IBoundTypeRef t)
        {
            _diagnostics.Add(_method, op.AquilaSyntax, ErrorCode.ERR_CannotInstantiateType, kind, t.Type);
        }

        public static void Analyse(DiagnosticBag diagnostics, SourceMethodSymbolBase method)
        {
            //
            method.GetDiagnostics(diagnostics);

            var visitor = new DiagnosticWalker<VoidStruct>(diagnostics, method);

            //
            if (method.ControlFlowGraph != null) // non-abstract method
            {
                visitor.VisitCFG(method.ControlFlowGraph);
            }
        }

        private DiagnosticWalker(DiagnosticBag diagnostics, SourceMethodSymbolBase method)
        {
            _diagnostics = diagnostics;
            _method = method;
        }

        protected override void VisitCFGInternal(ControlFlowGraph x)
        {
            Debug.Assert(x == _method.ControlFlowGraph);

            base.VisitCFGInternal(x);

            if (CallsParentCtor == false &&
                new Name(_method.Name).IsConstructName &&
                HasBaseConstruct(_method.ContainingType))
            {
                // Missing calling parent::__construct
                _diagnostics.Add(_method, null, //_method.SyntaxSignature.Span.ToTextSpan(),
                    ErrorCode.WRN_ParentCtorNotCalled, _method.ContainingType.Name);
            }

            // analyse missing or redefined labels
            CheckLabels(x.Labels);

            // report unreachable blocks
            CheckUnreachableCode(x);
        }

        /// <summary>
        /// Checks the base class has implementation of `__construct` which should be called.
        /// </summary>
        /// <param name="type">Self.</param>
        /// <returns>Whether the base of <paramref name="type"/> has `__construct` method implementation.</returns>
        static bool HasBaseConstruct(NamedTypeSymbol type)
        {
            var btype = type?.BaseType;
            if (btype != null && btype.SpecialType != SpecialType.System_Object && btype.IsClassType() &&
                !btype.IsAbstract)
            {
            }

            return false;
        }

        /// <summary>
        /// Determines if both identifiers differ only in casing.
        /// </summary>
        static bool IsLetterCasingMismatch(string str1, string str2)
        {
            return str1 != str2 && string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
        }

        void CheckLabels(ImmutableArray<ControlFlowGraph.LabelBlockState> labels)
        {
            if (labels == null || labels.Length == 0)
            {
                return;
            }

            for (int i = 0; i < labels.Length; i++)
            {
                var flags = labels[i].Flags;
                if ((flags & ControlFlowGraph.LabelBlockFlags.Defined) == 0)
                {
                    //TODO: Create diagnostics here
                    throw new NotImplementedException();
                }

                if ((flags & ControlFlowGraph.LabelBlockFlags.Used) == 0)
                {
                    // Warning: label not used
                }

                if ((flags & ControlFlowGraph.LabelBlockFlags.Redefined) != 0)
                {
                    //TODO: Create diagnostics here
                    throw new NotImplementedException();
                }
            }
        }
        
        public override T VisitTypeRef(BoundTypeRef typeRef)
        {
            CheckUndefinedType(typeRef);

            // Check that the right case of a class name is used
            if (typeRef.IsObject && typeRef is BoundClassTypeRef ct && ct.Type != null)
            {
                string refName = ct.ClassName.Name.Value;

                if (ct.Type.Kind != SymbolKind.ErrorType)
                {
                    var symbolName = ct.Type.Name;

                    if (IsLetterCasingMismatch(refName, symbolName))
                    {
                        // Wrong class name case
                        _diagnostics.Add(_method, typeRef.AquilaSyntax, ErrorCode.INF_TypeNameCaseMismatch, refName,
                            symbolName);
                    }
                }
            }

            return base.VisitTypeRef(typeRef);
        }

        public override T VisitReturnStmt(BoundReturnStmt x)
        {
            if (_method.Syntax is MethodDecl m)
            {
            }

            return base.VisitReturnStmt(x);
        }

        public override T VisitAssignEx(BoundAssignEx x)
        {
            return base.VisitAssignEx(x);
        }

        public override T VisitFieldRef(BoundFieldRef x)
        {
            if (x.Access.IsWrite &&
                ((Microsoft.CodeAnalysis.Operations.IMemberReferenceOperation)x).Member is PropertySymbol prop &&
                prop.SetMethod == null)
            {
                // read-only property written
                _diagnostics.Add(_method, GetMemberNameSpanForDiagnostic(x.AquilaSyntax),
                    ErrorCode.ERR_ReadOnlyPropertyWritten,
                    prop.ContainingType.MakeQualifiedName().ToString(), // TOOD: _statics
                    prop.Name);
            }

            //
            return base.VisitFieldRef(x);
        }

        public override T VisitVariableRef(BoundVariableRef x)
        {
            CheckUninitializedVariableUse(x);

            return base.VisitVariableRef(x);
        }

        public override T VisitTemporalVariableRef(BoundTemporalVariableRef x)
        {
            // do not make diagnostics on syntesized variables
            return default;
        }

        

        public override T VisitUnaryEx(BoundUnaryEx x)
        {
            base.VisitUnaryEx(x);

            switch (x.Operation)
            {
                case Operations.Clone:
                    break;
            }

            return default;
        }

        public override T VisitBinaryEx(BoundBinaryEx x)
        {
            base.VisitBinaryEx(x);

            //

            switch (x.Operation)
            {
                case Operations.Div:
                    if (x.Right.IsConstant())
                    {
                        if (x.Right.ConstantValue.IsZero())
                        {
                            //TODO: Create diagnostics here
                            throw new NotImplementedException();
                        }
                    }

                    break;
            }

            return default;
        }

        public override T VisitConversionEx(BoundConversionEx x)
        {
            base.VisitConversionEx(x);
            return default;
        }


        static Microsoft.CodeAnalysis.Text.TextSpan GetMemberNameSpanForDiagnostic(AquilaSyntaxNode node)
        {
            return node.Span;
        }

        private void CheckUninitializedVariableUse(BoundVariableRef x)
        {
            if (x.MaybeUninitialized && !x.Access.IsQuiet && x.AquilaSyntax != null)
            {
                _diagnostics.Add(_method, x.AquilaSyntax, ErrorCode.WRN_UninitializedVariableUse,
                    x.Name.NameValue.ToString());
            }
        }

        private void CheckUndefinedType(BoundTypeRef typeRef)
        {
            var type = typeRef.ResolvedType;

            // Ignore indirect types (e.g. $foo = new $className())
            if (type.IsErrorTypeOrNull() && !(typeRef is BoundIndirectTypeRef))
            {
                var errtype = typeRef.ResolvedType as ErrorTypeSymbol;
                if (errtype != null && errtype.CandidateReason == CandidateReason.Ambiguous)
                {
                    // type is declared but ambiguously,
                    // warning with declaration ambiguity was already reported, we may skip following
                    return;
                }
            }
        }
        public override T VisitCFGForeachEnumereeEdge(ForeachEnumereeEdge x)
        {
            base.VisitCFGForeachEnumereeEdge(x);
            return default;
        }
    }
}