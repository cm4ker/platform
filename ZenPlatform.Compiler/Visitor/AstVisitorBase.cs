using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Shared;

namespace ZenPlatform.Compiler.Visitor
{
    public abstract class AstVisitorBase : IVisitor
    {
        private Stack<AstNode> _visitStack;

        protected Stack<AstNode> VisitStack => _visitStack;

        public AstVisitorBase()
        {
            _visitStack = new Stack<AstNode>();
        }

        public virtual void BeforeVisitNode(AstNode node)
        {
            if (node is null) return;

            if (_visitStack.TryPeek(out var parent))
            {
                node.Parent = parent;
            }

            _visitStack.Push(node);
        }

        public virtual void AfterVisitNode(AstNode node)
        {
            _visitStack.Pop();
        }


        public void Visit(IVisitable visitable)
        {
            if (visitable is null) return;

            BeforeVisitNode(visitable as AstNode);
            Visit(visitable as AstNode);
            visitable.Accept(this);
            AfterVisitNode(visitable as AstNode);
        }

        public virtual void Visit(AstNode node)
        {
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
                .CaseIs<TypeNode>(VisitType)
                .CaseIs<InstructionsBodyNode>(VisitInstructionsBody)
                .CaseIs<Variable>(VisitVariable)
                .CaseIs<Assignment>(VisitAssigment)
                .CaseIs<PostIncrementStatement>(VisitPostIncrementStatement)
                .CaseIs<PostDecrementStatement>(VisitPostDecrementStatement)
                .CaseIs<Argument>(VisitArgument)
                .CaseIs<Return>(VisitReturn)
                .CaseIs<BinaryExpression>(VisitBinaryExpression)
                .CaseIs<CastExpression>(VisitCastExpression)
                .CaseIs<FieldExpression>(VisitFieldExpression)
                .CaseIs<If>(VisitIf)
                .CaseIs<Literal>(VisitLiteral)
                .CaseIs<IndexerExpression>(VisitIndexerExpression)
                .CaseIs<Try>(VisitTry)
                .BreakIfExecuted()
                .CaseIs<Expression>(VisitExpression)
                .Case(x => throw new Exception($"Unknown ast construction {x.GetType()}"), null);
        }

        public virtual void VisitTry(Try obj)
        {
            
        }

        public virtual void VisitIndexerExpression(IndexerExpression obj)
        {
        }

        public virtual void VisitPostDecrementStatement(PostDecrementStatement obj)
        {
        }

        public virtual void VisitPostIncrementStatement(PostIncrementStatement obj)
        {
        }

        public virtual void VisitArgument(Argument obj)
        {
        }

        public virtual void VisitLiteral(Literal obj)
        {
        }

        public virtual void VisitIf(If obj)
        {
        }

        public virtual void VisitFieldExpression(FieldExpression obj)
        {
        }

        public virtual void VisitCastExpression(CastExpression obj)
        {
        }

        public virtual void VisitBinaryExpression(BinaryExpression obj)
        {
        }

        public virtual void VisitReturn(Return obj)
        {
        }

        public virtual void VisitAssigment(Assignment obj)
        {
        }

        public virtual void VisitVariable(Variable obj)
        {
            //Do nothing
        }

        public virtual void VisitInstructionsBody(InstructionsBodyNode obj)
        {
        }

        public virtual void VisitType(TypeNode obj)
        {
        }

        public virtual void VisitFunction(Function obj)
        {
        }

        public virtual void VisitTypeBody(TypeBody obj)
        {
        }

        public virtual void VisitModuleStatement(Module obj)
        {
        }

        public virtual void VisitCallStatement(CallStatement obj)
        {
        }

        public virtual void VisitCall(Call obj)
        {
        }

        public virtual void VisitParameter(Parameter obj)
        {
        }

        public virtual void VisitWhile(While obj)
        {
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
        }
    }
}