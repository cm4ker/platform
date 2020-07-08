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
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Syntax;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Errors;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;

namespace Aquila.CodeAnalysis.FlowAnalysis.Passes
{
    internal partial class DiagnosticWalker<T> : GraphExplorer<T>
    {
        private readonly DiagnosticBag _diagnostics;
        private SourceMethodSymbol _method;

        private bool CallsParentCtor { get; set; }

        AquilaCompilation DeclaringCompilation => _method.DeclaringCompilation;

        TypeRefContext TypeCtx => _method.TypeRefContext;

        void CheckMissusedPrimitiveType(IBoundTypeRef tref)
        {
            if (tref.IsPrimitiveType)
            {
                // error: use of primitive type {0} is misused // primitive type does not make any sense in this context
                _diagnostics.Add(_method, tref.AquilaSyntax, ErrorCode.ERR_PrimitiveTypeNameMisused, tref);
            }
        }

        void Add(Span span, Aquila.Syntax.Errors.ErrorInfo err, params string[] args)
        {
            _diagnostics.Add(DiagnosticBagExtensions.ParserDiagnostic(_method, span, err, args));
        }

        void CannotInstantiate(IAquilaOperation op, string kind, IBoundTypeRef t)
        {
            _diagnostics.Add(_method, op.AquilaSyntax, ErrorCode.ERR_CannotInstantiateType, kind, t.Type);
        }

        public static void Analyse(DiagnosticBag diagnostics, SourceMethodSymbol method)
        {
            //
            method.GetDiagnostics(diagnostics);

            var visitor = new DiagnosticWalker<VoidStruct>(diagnostics, method);

            //
            if (method.ControlFlowGraph != null) // non-abstract method
            {
                visitor.VisitCFG(method.ControlFlowGraph);
            }

            //
            visitor.CheckParams();
        }

        private DiagnosticWalker(DiagnosticBag diagnostics, SourceMethodSymbol method)
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

        private void CheckParams()
        {
            // check source parameters
            var srcparams = _method.SourceParameters;
            foreach (var p in srcparams)
            {
                if (!CheckParameterDefaultValue(p))
                {
                    // var expectedtype =
                    //     (p.Syntax.TypeHint is NullableTypeRef nullable ? nullable.TargetType : p.Syntax.TypeHint)
                    //     .ToString(); // do not show "?" in nullable types
                    // var valuetype = TypeCtx.ToString(p.Initializer.TypeRefMask);
                    //
                    // _diagnostics.Add(_method, p.Syntax.InitValue, ErrorCode.ERR_DefaultParameterValueTypeMismatch,
                    //     p.Name, expectedtype, valuetype);
                }
            }
        }

        bool CheckParameterDefaultValue(SourceParameterSymbol p)
        {
            // var thint = p.Syntax.TypeHint;
            // if (thint != null)
            // {
            //     // check type hint and default value
            //     var defaultvalue = p.Initializer;
            //     if (defaultvalue != null && !defaultvalue.TypeRefMask.IsAnyType && !defaultvalue.TypeRefMask.IsDefault)
            //     {
            //         var valuetype = defaultvalue.TypeRefMask;
            //
            //         if (TypeCtx.IsNull(valuetype))
            //         {
            //             // allow NULL anytime
            //             return true;
            //         }
            //
            //         if (thint is NullableTypeRef nullable)
            //         {
            //             // unwrap nullable type hint
            //             thint = nullable.TargetType;
            //         }
            //
            //         if (thint is PrimitiveTypeRef primitive)
            //         {
            //             switch (primitive.PrimitiveTypeName)
            //             {
            //                 case PrimitiveTypeRef.PrimitiveType.@bool:
            //                     return TypeCtx.IsBoolean(valuetype);
            //
            //                 case PrimitiveTypeRef.PrimitiveType.array:
            //                     return TypeCtx.IsArray(valuetype);
            //
            //                 case PrimitiveTypeRef.PrimitiveType.@string:
            //                     return TypeCtx.IsAString(valuetype);
            //
            //                 case PrimitiveTypeRef.PrimitiveType.@object:
            //                     return false;
            //
            //                 case PrimitiveTypeRef.PrimitiveType.@float:
            //                 case PrimitiveTypeRef.PrimitiveType.@int:
            //                     return TypeCtx.IsNumber(valuetype);
            //             }
            //         }
            //         else if (thint is ClassTypeRef classtref)
            //         {
            //             return false; // cannot have default value other than NULL
            //         }
            //     }
            // }

            // ok
            return true;
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
                    Add(labels[i].LabelSpan, Aquila.Syntax.Errors.Errors.UndefinedLabel, labels[i].Label);
                }

                if ((flags & ControlFlowGraph.LabelBlockFlags.Used) == 0)
                {
                    // Warning: label not used
                }

                if ((flags & ControlFlowGraph.LabelBlockFlags.Redefined) != 0)
                {
                    Add(labels[i].LabelSpan, Aquila.Syntax.Errors.Errors.LabelRedeclared, labels[i].Label);
                }
            }
        }
        //
        // public override T VisitEval(BoundEvalEx x)
        // {
        //     _diagnostics.Add(_method, null /*'eval'*/,
        //         ErrorCode.INF_EvalDiscouraged);
        //
        //     return base.VisitEval(x);
        // }

        public override T VisitArrayEx(BoundArrayEx x)
        {
            return base.VisitArrayEx(x);
        }

        // internal override T VisitIndirectTypeRef(BoundIndirectTypeRef x)
        // {
        //     return base.VisitIndirectTypeRef(x);
        // }

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
            if (x.ContainingType != null)
            {
                // class const
                // static field
                CheckMissusedPrimitiveType(x.ContainingType);
            }

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

        public override T VisitCFGCatchBlock(CatchBlock x)
        {
            // TODO: x.TypeRefs -> CheckMissusedPrimitiveType

            return base.VisitCFGCatchBlock(x);
        }

        // public override T VisitInstanceOf(BoundInstanceOfEx x)
        // {
        //     CheckMissusedPrimitiveType(x.AsType);
        //
        //     return base.VisitInstanceOf(x);
        // }

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

        public override T VisitDeclareStmt(BoundDeclareStmt x)
        {
            // _diagnostics.Add(
            //     _method,
            //     ((Stmt) x.AquilaSyntax).GetDeclareClauseSpan(),
            //     ErrorCode.WRN_NotYetImplementedIgnored,
            //     "Declare construct");

            return base.VisitDeclareStmt(x);
        }

        // public override T VisitAssertEx(BoundAssertEx x)
        // {
        //     base.VisitAssertEx(x);
        //
        //     var args = x.ArgumentsInSourceOrder;
        //
        //     // check number of parameters
        //     // check whether it is not always false or always true
        //     if (args.Length >= 1)
        //     {
        //         if (args[0].Value.ConstantValue.EqualsOptional(false.AsOptional()))
        //         {
        //             // always failing
        //             _diagnostics.Add(_method, x.AquilaSyntax, ErrorCode.WRN_AssertAlwaysFail);
        //         }
        //
        //         if (TypeCtx.IsAString(args[0].Value.TypeRefMask))
        //         {
        //             // deprecated and not supported
        //             _diagnostics.Add(_method, args[0].Value.AquilaSyntax, ErrorCode.WRN_StringAssertionDeprecated);
        //         }
        //
        //         if (args.Length > 2)
        //         {
        //             // too many args
        //             _diagnostics.Add(_method, x.AquilaSyntax, ErrorCode.WRN_TooManyArguments, "assert", 2,
        //                 args.Length);
        //         }
        //     }
        //     else
        //     {
        //         // assert() expects at least 1 parameter, 0 given
        //         _diagnostics.Add(_method, x.AquilaSyntax, ErrorCode.WRN_MissingArguments, "assert", 1, 0);
        //     }
        //
        //     return default;
        // }

        public override T VisitUnaryEx(BoundUnaryEx x)
        {
            base.VisitUnaryEx(x);

            switch (x.Operation)
            {
                case Operations.Clone:
                    // check we only pass object instances to the "clone" operation
                    // anything else causes a runtime warning!
                    // var operandTypeMask = x.Operand.TypeRefMask;
                    // if (!operandTypeMask.IsAnyType &&
                    //     !operandTypeMask.IsRef &&
                    //     !TypeCtx.IsObjectOnly(operandTypeMask))
                    // {
                    //     _diagnostics.Add(_method, x.AquilaSyntax, ErrorCode.WRN_CloneNonObject,
                    //         TypeCtx.ToString(operandTypeMask));
                    // }

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
                            Add(x.Right.AquilaSyntax.Span, Warnings.DivisionByZero);
                        }
                    }

                    break;
            }

            return default;
        }

        public override T VisitConversionEx(BoundConversionEx x)
        {
            base.VisitConversionEx(x);

            // if (!x.IsImplicit && x.AquilaSyntax != null &&
            //     x.Operand.TypeRefMask.IsSingleType &&
            //     x.TargetType == TypeCtx.GetTypes(x.Operand.TypeRefMask).FirstOrDefault())
            // {
            //     _diagnostics.Add(_method, x.AquilaSyntax, ErrorCode.INF_RedundantCast);
            // }

            return default;
        }

        void CheckMethodCallTargetInstance(BoundExpression target, string methodName)
        {
            if (target == null)
            {
                // syntax error (?)
                return;
            }

            string nonobjtype = null;

            if (target.ResultType != null)
            {
                switch (target.ResultType.SpecialType)
                {
                    case SpecialType.System_Void:
                    case SpecialType.System_Int32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_String:
                    case SpecialType.System_Boolean:
                        nonobjtype = null;
                        break;
                    default:

                        break;
                }
            }
            else
            {
                // var tmask = target.TypeRefMask;
                // if (!tmask.IsAnyType && !tmask.IsRef && !TypeCtx.IsObject(tmask))
                // {
                //     nonobjtype = TypeCtx.ToString(tmask);
                // }
            }

            //
            if (nonobjtype != null)
            {
                _diagnostics.Add(_method, target.AquilaSyntax, ErrorCode.ERR_MethodCalledOnNonObject,
                    methodName ?? "{}",
                    nonobjtype);
            }
        }

        static string GetMemberNameForDiagnostic(Aquila.CodeAnalysis.Symbols.Symbol target, bool isMemberName)
        {
            string name = target.AquilaName();

            if (isMemberName)
            {
                var qname = target.ContainingType.AquilaQualifiedName(); // TOOD: _statics
                name = qname.ToString(new Name(name), false);
            }

            return name;
        }


        static Microsoft.CodeAnalysis.Text.TextSpan GetMemberNameSpanForDiagnostic(LangElement node)
        {
            return node.Span.ToTextSpan();
        }

        void CheckObsoleteSymbol(LangElement syntax, Aquila.CodeAnalysis.Symbols.Symbol target, bool isMemberCall)
        {
            var obsolete = target?.ObsoleteAttributeData;
            if (obsolete != null)
            {
                // _diagnostics.Add(_method, GetMemberNameSpanForDiagnostic(syntax), ErrorCode.WRN_SymbolDeprecated,
                //     target.Kind.ToString(), GetMemberNameForDiagnostic(target, isMemberCall), obsolete.Message);
            }
        }

        private void CheckUndefinedMethodCall(BoundCallEx x, TypeSymbol type, BoundMethodName name)
        {
            if (x.TargetMethod is MissingMethodSymbol)
            {
                if (x.AquilaSyntax == null)
                    throw new Exception("Internal compiler error");

                var span = x.AquilaSyntax is CallEx fnc ? fnc.Span : x.AquilaSyntax.Span;
                _diagnostics.Add(_method, span.ToTextSpan(), ErrorCode.WRN_UndefinedMethodCall, type.Name,
                    name.NameValue.ToString());
            }
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

        public override T VisitCFGTryCatchEdge(TryCatchEdge x)
        {
            return base.VisitCFGTryCatchEdge(x);
        }

        public override T VisitStaticVarStmt(BoundStaticVarStmt x)
        {
            return base.VisitStaticVarStmt(x);
        }

        public override T VisitCFGForeachEnumereeEdge(ForeachEnumereeEdge x)
        {
            base.VisitCFGForeachEnumereeEdge(x);

            // var enumereeTypeMask = x.Enumeree.TypeRefMask;
            // if (!enumereeTypeMask.IsAnyType && !enumereeTypeMask.IsRef)
            // {
            //     // Apart from array, any object can possibly implement Traversable, hence no warning for them
            //     var types = TypeCtx.GetTypes(enumereeTypeMask);
            //     if (!types.Any(t => t.IsArray || t.IsObject)
            //     ) // Using !All causes too many false positives (due to explode(..) etc.)
            //     {
            //         // Using non-iterable type for enumeree
            //         _diagnostics.Add(_method, x.Enumeree.AquilaSyntax, ErrorCode.WRN_ForeachNonIterable,
            //             TypeCtx.ToString(enumereeTypeMask));
            //     }
            // }

            return default;
        }
    }
}