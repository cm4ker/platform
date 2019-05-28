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
    public abstract class AstVisitorBase
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

        public virtual void Visit(AstNode node)
        {
            BeforeVisitNode(node);

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

        public virtual void VisitType(ZType obj)
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

    public class AstSymbolVisitor : AstVisitorBase
    {
        public override void VisitExpression(Expression e)
        {
        }

        public override void VisitVariable(Variable obj)
        {
            if (obj.Parent is InstructionsBodyNode ibn)
            {
                ibn.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception($"Invalid register variable in scope {obj.Name}");
            }
        }

        public override void VisitParameter(Parameter obj)
        {
            if (obj.Parent is Function f)
            {
                f.InstructionsBody.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register parameter in scope");
            }
        }

        public override void VisitTypeBody(TypeBody obj)
        {
            obj.SymbolTable.Clear();
        }

        public override void VisitFunction(Function obj)
        {
            obj.InstructionsBody.SymbolTable.Clear();
            if (obj.Parent is TypeEntity te)
            {
                te.TypeBody.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register function in scope");
            }
        }
    }


    /// <summary>
    /// Визитор для вычисления типа
    /// </summary>
    public class AstTypeCalculationVisitor : AstVisitorBase
    {
    }
}