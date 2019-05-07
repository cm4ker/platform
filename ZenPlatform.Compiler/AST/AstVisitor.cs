using System;
using System.Collections;
using System.Collections.Generic;
using ZenPlatform.Compiler.AST.Definitions;
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
                .CaseIs<TypeBody>(VisitTypeBody)
                .BreakIfExecuted()
                .CaseIs<Expression>(VisitExpression)
                .Case(x => throw new Exception($"Unknown ast construction {x.GetType()}"), null);
            _visitStack.Pop();
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