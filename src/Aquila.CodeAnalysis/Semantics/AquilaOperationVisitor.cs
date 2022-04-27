﻿using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics
{
    /// <summary>
    /// Base visitor for Aquila operations.
    /// </summary>
    /// <typeparam name="TResult">Return type of all the Visit operations, use <see cref="VoidStruct"/> if none.</typeparam>
    // public abstract class AquilaOperationVisitor<TResult>
    // {
    //     /// <summary>Visits given operation.</summary>
    //     protected TResult Accept(IAquilaOperation x) => (x != null) ? x.Accept(this) : default;
    //
    //     #region Expressions
    //
    //     protected virtual TResult DefaultVisitOperation(BoundOperation x) => default;
    //
    //     protected virtual TResult VisitMethodCall(BoundMethodCall x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitLiteral(BoundLiteral x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitCopyValue(BoundCopyValue x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitArgument(BoundArgument x) => DefaultVisitOperation(x);
    //
    //     internal virtual TResult VisitTypeRef(BoundTypeRef x) => DefaultVisitOperation(x);
    //
    //     internal virtual TResult VisitIndirectTypeRef(BoundIndirectTypeRef x) => DefaultVisitOperation(x);
    //
    //     internal virtual TResult VisitMultipleTypeRef(BoundMultipleTypeRef x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitMethodName(BoundMethodName x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitGlobalFunctionCall(BoundGlobalFunctionCall x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitInstanceFunctionCall(BoundInstanceFunctionCall x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitStaticFunctionCall(BoundCall x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitEcho(BoundEcho x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitConcat(BoundConcatEx x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitNew(BoundNewEx x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitInclude(BoundIncludeEx x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitExit(BoundExitEx x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitAssert(BoundAssertEx x) => VisitMethodCall(x);
    //
    //     public virtual TResult VisitBinaryExpression(BoundBinaryEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitUnaryExpression(BoundUnaryEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitConversion(BoundConversionEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitIncDec(BoundIncDecEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitConditional(BoundConditionalEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitAssign(BoundAssignEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitCompoundAssign(BoundCompoundAssignEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitVariableName(BoundVariableName x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitVariableRef(BoundVariableRef x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitTemporalVariableRef(BoundTemporalVariableRef x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitList(BoundListEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitFieldRef(BoundFieldRef x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitArray(BoundArrayEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitArrayItem(BoundArrayItemEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitArrayItemOrd(BoundArrayItemOrdEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitInstanceOf(BoundInstanceOfEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitGlobalConstUse(BoundGlobalConst x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitGlobalConstDecl(BoundGlobalConstDeclStatement x) => DefaultVisitOperation(x);
    //
    //     // public virtual TResult VisitPseudoConstUse(BoundPseudoConst x) => DefaultVisitOperation(x);
    //     //
    //     // public virtual TResult VisitPseudoClassConstUse(BoundPseudoClassConst x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitIsEmpty(BoundIsEmptyEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitIsSet(BoundIsSetEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitOffsetExists(BoundOffsetExists x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitTryGetItem(BoundTryGetItem x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitLambda(BoundLambda x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitThrow(BoundThrowExpression x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitEval(BoundEvalEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitYieldEx(BoundYieldEx x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitYieldFromEx(BoundYieldFromEx x) => DefaultVisitOperation(x);
    //
    //     #endregion
    //
    //     #region Statements
    //
    //     //public virtual TResult VisitUnset(BoundUnset x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitEmptyStatement(BoundEmptyStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitBlockStatement(Graph.BoundBlock x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitExpressionStatement(BoundExpressionStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitReturn(BoundReturnStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitFunctionDeclaration(BoundFunctionDeclStatement x) => DefaultVisitOperation(x);
    //
    //     // public virtual TResult VisitTypeDeclaration(BoundTypeDeclStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitGlobalStatement(BoundGlobalVariableStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitStaticStatement(BoundStaticVariableStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitYieldStatement(BoundYieldStatement x) => DefaultVisitOperation(x);
    //
    //     public virtual TResult VisitDeclareStatement(BoundDeclareStatement x) => DefaultVisitOperation(x);
    //
    //     #endregion
    // }
}


namespace Aquila.CodeAnalysis.Semantics
{
    // public partial class AquilaOperationVisitor<TResult>
    // {
    //     /// <summary>Visits given operation.</summary>
    //     protected TResult Accept(IAquilaOperation x) => (x != null) ? x.Accept(this) : default;
    //
    //     public virtual TResult VisitDefault(BoundOperation x) => default;
    // }
}