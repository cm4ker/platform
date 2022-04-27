using Microsoft.CodeAnalysis.Operations;
using Aquila.CodeAnalysis.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Semantics.TypeRef;

namespace Aquila.CodeAnalysis.Semantics.Graph
{
    /// <summary>
    /// Visitor used to traverse CFG and all its operations.
    /// </summary>
    /// <remarks>Visitor does not implement infinite recursion prevention.</remarks>
    internal abstract class GraphWalker<T> : GraphVisitor<T>
    {
        // #region ControlFlowGraph
        //
        // public override T VisitCFG(ControlFlowGraph x)
        // {
        //     x.Start.Accept(this);
        //
        //     return default;
        // }
        //
        // #endregion
        //
        // #region Graph.Block
        //
        // void VisitCFGBlockStatements(BoundBlock x)
        // {
        //     for (int i = 0; i < x.Statements.Count; i++)
        //     {
        //         Accept(x.Statements[i]);
        //     }
        // }
        //
        // /// <summary>
        // /// Visits block statements and its edge to next block.
        // /// </summary>
        // protected override T DefaultVisitBlock(BoundBlock x)
        // {
        //     VisitCFGBlockStatements(x);
        //
        //     AcceptEdge(x, x.NextEdge);
        //
        //     return default;
        // }
        //
        // protected virtual T AcceptEdge(BoundBlock fromBlock, Edge edge)
        // {
        //     return edge != null ? edge.Accept(this) : default;
        // }
        //
        // public override T VisitCFGBlock(BoundBlock x)
        // {
        //     DefaultVisitBlock(x);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGExitBlock(ExitBlock x)
        // {
        //     VisitCFGBlock(x);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGCatchBlock(CatchBlock x)
        // {
        //     Accept(x.TypeRef);
        //     Accept(x.Variable);
        //
        //     DefaultVisitBlock(x);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGCaseBlock(MatchArmBlock x)
        // {
        //     if (x.MatchValue == null)
        //     {
        //         //VisitCFGBlock(x.);
        //     }
        //
        //     if (x.MatchValue != null)
        //     {
        //         Accept(x.MatchValue);
        //     }
        //
        //     DefaultVisitBlock(x);
        //
        //     return default;
        // }
        //
        // #endregion
        //
        // #region Graph.Edge
        //
        // public override T VisitCFGSimpleEdge(SimpleEdge x)
        // {
        //     Debug.Assert(x.NextBlock != null);
        //     x.NextBlock.Accept(this);
        //
        //     DefaultVisitEdge(x);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGConditionalEdge(ConditionalEdge x)
        // {
        //     Accept(x.Condition);
        //
        //     x.TrueTarget.Accept(this);
        //     x.FalseTarget.Accept(this);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGTryCatchEdge(TryCatchEdge x)
        // {
        //     x.BodyBlock.Accept(this);
        //
        //     foreach (var c in x.CatchBlocks)
        //         c.Accept(this);
        //
        //     if (x.FinallyBlock != null)
        //         x.FinallyBlock.Accept(this);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGForeachEnumereeEdge(ForeachEnumereeEdge x)
        // {
        //     Accept(x.Enumeree);
        //
        //     x.NextBlock.Accept(this);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGForeachMoveNextEdge(ForeachMoveNextEdge x)
        // {
        //     Accept(x.ValueVariable);
        //     Accept(x.KeyVariable);
        //
        //     x.BodyBlock.Accept(this);
        //     x.NextBlock.Accept(this);
        //
        //     return default;
        // }
        //
        // public override T VisitCFGSwitchEdge(MatchEdge x)
        // {
        //     Accept(x.SwitchValue);
        //
        //     //
        //     var arr = x.MatchBlocks;
        //     for (int i = 0; i < arr.Length; i++)
        //         arr[i].Accept(this);
        //
        //     return default;
        // }
        //
        // #endregion
        //
        // #region Expressions
        //
        // public override T VisitLiteral(BoundLiteral x)
        // {
        //     //VisitLiteralExpression(x);
        //
        //     return default;
        // }
        //
        // public override T VisitArgument(BoundArgument x)
        // {
        //     Accept(x.Value);
        //
        //     return default;
        // }
        // //
        // // internal override T VisitTypeRef(BoundTypeRef x)
        // // {
        // //     return base.VisitTypeRef(x);
        // // }
        // //
        // // internal override T VisitIndirectTypeRef(BoundIndirectTypeRef x)
        // // {
        // //     Accept(x.TypeExpression);
        // //     return base.VisitIndirectTypeRef(x);
        // // }
        // //
        // // internal override T VisitMultipleTypeRef(BoundMultipleTypeRef x)
        // // {
        // //     Debug.Assert(x != null);
        // //     Debug.Assert(x.TypeRefs.Length > 1);
        // //
        // //     for (int i = 0; i < x.TypeRefs.Length; i++)
        // //     {
        // //         x.TypeRefs[i].Accept(this);
        // //     }
        // //
        // //     return default;
        // // }
        //
        // public override T VisitMethodName(BoundMethodName x)
        // {
        //     Accept(x.NameExpression);
        //
        //     return default;
        // }
        //
        // public override T VisitBinaryEx(BoundBinaryEx x)
        // {
        //     Accept(x.Left);
        //     Accept(x.Right);
        //
        //     return default;
        // }
        //
        // public override T VisitUnaryEx(BoundUnaryEx x)
        // {
        //     Accept(x.Operand);
        //
        //     return default;
        // }
        //
        // public override T VisitConversionEx(BoundConversionEx x)
        // {
        //     Accept(x.Operand);
        //     Accept(x.TargetType);
        //
        //     return default;
        // }
        //
        // public override T VisitIncDecEx(BoundIncDecEx x)
        // {
        //     Accept(x.Target);
        //     Accept(x.Value);
        //
        //     return default;
        // }
        //
        // public override T VisitConditionalEx(BoundConditionalEx x)
        // {
        //     Accept(x.Condition);
        //     Accept(x.IfTrue);
        //     Accept(x.IfFalse);
        //
        //     return default;
        // }
        //
        // public override T VisitAssignEx(BoundAssignEx x)
        // {
        //     Accept(x.Target);
        //     Accept(x.Value);
        //
        //     return default;
        // }
        //
        // public override T VisitCompoundAssignEx(BoundCompoundAssignEx x)
        // {
        //     Accept(x.Target);
        //     Accept(x.Value);
        //
        //     return default;
        // }
        //
        // public override T VisitVariableName(BoundVariableName x)
        // {
        //     Accept(x.NameExpression);
        //
        //     return default;
        // }
        //
        // public override T VisitVariableRef(BoundVariableRef x)
        // {
        //     Accept(x.Name);
        //
        //     return default;
        // }
        //
        // public override T VisitTemporalVariableRef(BoundTemporalVariableRef x)
        // {
        //     // BoundSynthesizedVariableRef is based solely on BoundVariableRef so far 
        //     VisitVariableRef(x);
        //
        //     return default;
        // }
        //
        // public override T VisitListEx(BoundListEx x)
        // {
        //     x.Items.ForEach(pair =>
        //     {
        //         Accept(pair.Key);
        //         Accept(pair.Value);
        //     });
        //
        //     return default;
        // }
        //
        // public override T VisitFieldRef(BoundFieldRef x)
        // {
        //     Accept(x.Instance);
        //
        //     return default;
        // }
        //
        // public override T VisitArrayEx(BoundArrayEx x)
        // {
        //     x.Items.ForEach(pair =>
        //     {
        //         Accept(pair.Key);
        //         Accept(pair.Value);
        //     });
        //
        //     return default;
        // }
        //
        // public override T VisitArrayItemEx(BoundArrayItemEx x)
        // {
        //     Accept(x.Array);
        //     Accept(x.Index);
        //
        //     return default;
        // }
        //
        // public override T VisitArrayItemOrdEx(BoundArrayItemOrdEx x)
        // {
        //     Accept(x.Array);
        //     Accept(x.Index);
        //
        //     return default;
        // }
        //
        // // public override T VisitInstanceOfEx(BoundInstanceOfEx x)
        // // {
        // //     Accept(x.Operand);
        // //     Accept(x.AsType);
        // //
        // //     return default;
        // // }
        //
        // // public override T VisitGlobalConstUse(BoundGlobalConst x)
        // // {
        // //     return default;
        // // }
        //
        // // public override T VisitGlobalConstDecl(BoundGlobalConstDeclStmt x)
        // // {
        // //     Accept(x.Value);
        // //
        // //     return default;
        // // }
        //
        // // public override T VisitPseudoConstUse(BoundPseudoConst x)
        // // {
        // //     return default;
        // // }
        // //
        // // public override T VisitPseudoClassConstUse(BoundPseudoClassConst x)
        // // {
        // //     Accept(x.TargetType);
        // //
        // //     return default;
        // // }
        //
        // // public override T VisitIsEmpty(BoundIsEmptyEx x)
        // // {
        // //     Accept(x.Operand);
        // //
        // //     return default;
        // // }
        // //
        // // public override T VisitIsSet(BoundIsSetEx x)
        // // {
        // //     Accept(x.VarReference);
        // //
        // //     return default;
        // // }
        //
        // // public override T VisitOffsetExists(BoundOffsetExists x)
        // // {
        // //     Accept(x.Receiver);
        // //     Accept(x.Index);
        // //
        // //     return default;
        // // }
        // //
        // // public override T VisitTryGetItem(BoundTryGetItem x)
        // // {
        // //     Accept(x.Array);
        // //     Accept(x.Index);
        // //     Accept(x.Fallback);
        // //
        // //     return default;
        // // }
        // //
        // // public override T VisitLambda(BoundLambda x)
        // // {
        // //     return default;
        // // }
        // //
        // // public override T VisitEval(BoundEvalEx x)
        // // {
        // //     Accept(x.CodeExpression);
        // //
        // //     return default;
        // // }
        //
        // public override T VisitThrowEx(BoundThrowEx x)
        // {
        //     Accept(x.Thrown);
        //
        //     return default;
        // }
        //
        // // public override T VisitYieldEx(BoundYieldEx boundYieldEx)
        // // {
        // //     return default;
        // // }
        // //
        // // public override T VisitYieldFromEx(BoundYieldFromEx x)
        // // {
        // //     Accept(x.Operand);
        // //
        // //     return default;
        // // }
        //
        // #endregion
        //
        // #region Statements
        //
        // public override T VisitEmptyStmt(BoundEmptyStmt x)
        // {
        //     return default;
        // }
        //
        // public override T VisitBlock(Graph.BoundBlock x)
        // {
        //     Debug.Assert(x.NextEdge == null);
        //
        //     for (int i = 0; i < x.Statements.Count; i++)
        //     {
        //         Accept(x.Statements[i]);
        //     }
        //
        //     return default;
        // }
        //
        // public override T VisitExpressionStmt(BoundExpressionStmt x)
        // {
        //     Accept(x.Expression);
        //
        //     return default;
        // }
        //
        // public override T VisitReturnStmt(BoundReturnStmt x)
        // {
        //     Accept(x.Returned);
        //
        //     return default;
        // }
        //
        // public override T VisitMethodDeclStmt(BoundMethodDeclStmt x)
        // {
        //     return default;
        // }
        //
        // // public override T VisitTypeDeclaration(BoundTypeDeclStatement x)
        // // {
        // //     return default;
        // // }
        //
        // // public override T VisitGlobalStatement(BoundGlobalVariableStatement x)
        // // {
        // //     Accept(x.Variable);
        // //
        // //     return default;
        // // }
        // //
        // // public override T VisitStaticStatement(BoundStaticVarStmt x)
        // // {
        // //     return default;
        // // }
        // //
        // // public override T VisitYieldStatement(BoundYieldStmt boundYieldStmt)
        // // {
        // //     Accept(boundYieldStmt.YieldedValue);
        // //     Accept(boundYieldStmt.YieldedKey);
        // //
        // //     return default;
        // // }
        //
        // public override T VisitDeclareStmt(BoundDeclareStmt x)
        // {
        //     return default;
        // }
        //
        // #endregion
    }
}