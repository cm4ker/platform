using System;
using System.Collections;
using System.Collections.Generic;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expression;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Shared;


namespace ZenPlatform.Compiler.AST
{
    public class AstVisitor
    {
        private Stack<AstNode> _visitStack;

        public AstVisitor()
        {
            _visitStack = new Stack<AstNode>();
        }

        public virtual void Visit(AstNode node)
        {
            if (node is null) return;

            if (_visitStack.TryPeek(out var parent))
            {
                node.Parent = parent;
            }

            _visitStack.Push(node);

            ItemSwitch<AstNode>
                .Switch(node)
                .CaseIs<CompilationUnit>(VisitCompilationUnit)
                .CaseIs<Name>(VisitName)
                .CaseIs<For>(VisitFor)
                .CaseIs<While>(VisitWhile)
                .CaseIs<Parameter>(VisitParameter)
                .CaseIs<Call>(VisitCall)
                .CaseIs<CallStatement>(VisitCallStatement)
                .CaseIs<Module>(VisitModuleStatement)
                .CaseIs<Function>(VisitFunction)
                .CaseIs<TypeBody>(VisitTypeBody)
                .CaseIs<ZType>(VisitType)
                .CaseIs<InstructionsBodyNode>(VisitInstructionsBody)
                .CaseIs<Variable>(VisitVariable)
                .CaseIs<Assignment>(VisitAssigment)
                .CaseIs<Return>(VisitReturn)
                .CaseIs<BinaryExpression>(VisitBinaryExpression)
                .CaseIs<CastExpression>(VisitCastExpression)
                .CaseIs<FieldExpression>(VisitFieldExpression)
                .CaseIs<If>(VisitIf)
                .CaseIs<Literal>(VisitLiteral)
                .BreakIfExecuted()
                .CaseIs<Expression>(VisitExpression)
                .Case(x => throw new Exception($"Unknown ast construction {x.GetType()}"), null);
            _visitStack.Pop();
        }

        private void VisitLiteral(Literal obj)
        {
        }

        private void VisitIf(If obj)
        {
            Visit(obj.ElseInstructionsBody);
            Visit(obj.Condition);
            Visit(obj.IfInstructionsBody);
        }

        private void VisitFieldExpression(FieldExpression obj)
        {
            Visit(obj.Expression);
        }

        private void VisitCastExpression(CastExpression obj)
        {
            Visit(obj.Value);
        }

        private void VisitBinaryExpression(BinaryExpression obj)
        {
            Visit(obj.Left);
            Visit(obj.Right);
        }

        private void VisitReturn(Return obj)
        {
            Visit(obj.Value);
        }

        private void VisitAssigment(Assignment obj)
        {
            Visit(obj.Value);
            Visit(obj.Index);
        }

        private void VisitVariable(Variable obj)
        {
            //Do nothing
        }

        private void VisitInstructionsBody(InstructionsBodyNode obj)
        {
            obj.Statements.ForEach(Visit);
        }

        private void VisitType(ZType obj)
        {
        }

        private void VisitFunction(Function obj)
        {
            obj.Parameters.ForEach(Visit);
            Visit(obj.Type);
            Visit(obj.InstructionsBody);
        }

        private void VisitTypeBody(TypeBody obj)
        {
            obj.Functions.ForEach(Visit);
        }

        public virtual void VisitModuleStatement(Module obj)
        {
            Visit(obj.TypeBody);
        }

        private void VisitCallStatement(CallStatement obj)
        {
        }

        private void VisitCall(Call obj)
        {
        }

        private void VisitParameter(Parameter obj)
        {
        }

        private void VisitWhile(While obj)
        {
            Visit(obj.InstructionsBody);
        }

        public virtual void VisitFor(For obj)
        {
        }

        public virtual void VisitName(Name obj)
        {
        }


        public virtual void VisitExpression(Expression e)
        {
            throw new Exception("Unknown expression: " + e.GetType());
        }

        public virtual void VisitCompilationUnit(CompilationUnit cu)
        {
            cu.TypeEntities.ForEach(Visit);
        }
    }
}