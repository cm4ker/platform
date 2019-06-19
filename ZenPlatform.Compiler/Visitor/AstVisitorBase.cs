using System;
using System.Collections.Generic;
using System.Xml.XPath;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;
using ZenPlatform.Shared;

namespace ZenPlatform.Compiler.Visitor
{
    public class BreakException : Exception
    {
    }

    public class BreakWithResultException : Exception
    {
        public BreakWithResultException(object result)
        {
            Result = result;
        }

        public object Result { get; }
    }

    public abstract class AstVisitorBase<T> : IVisitor<T>
    {
        private Stack<AstNode> _visitStack;
        private bool _break;

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


        public T Visit(IVisitable visitable)
        {
            if (visitable is null) return default;

            _break = false;

            T result = default;

            try
            {
                BeforeVisitNode(visitable as AstNode);
                result = VisitInternal(visitable as AstNode);
            }
            catch (BreakException e)
            {
                //ignored
            }
            catch (BreakWithResultException e)
            {
                result = (T) e.Result;
            }

            if (!_break)
                visitable.Accept(this);
            AfterVisitNode(visitable as AstNode);

            return result;
        }


        /// <summary>
        /// Прервать текущее хождение по текущей ветке дерева. Визитор продолжит идти по следующей ветке
        /// </summary>
        protected void Stop()
        {
            _break = true;
            throw new BreakException();
        }

        /// <summary>
        /// Прервать текущее хождение по текущей ветке дерева. Визитор продолжит идти по следующей ветке
        /// </summary>
        protected void StopWithResult(T result)
        {
            _break = true;
            throw new BreakWithResultException(result);
        }


        private T VisitInternal(AstNode node)
        {
            return ItemSwitchWithResult<AstNode, T>
                .Switch(node)
                .CaseIs<Root>(VisitRoot)
                .CaseIs<CompilationUnit>(VisitCompilationUnit)
                .CaseIs<Name>(VisitName)
                .CaseIs<For>(VisitFor)
                .CaseIs<While>(VisitWhile)
                .CaseIs<Parameter>(VisitParameter)
                .CaseIs<Call>(VisitCall)
                .CaseIs<CallStatement>(VisitCallStatement)
                .CaseIs<Module>(VisitModule)
                .CaseIs<Function>(VisitFunction)
                .CaseIs<TypeBody>(VisitTypeBody)
                .CaseIs<TypeNode>(VisitType)
                .CaseIs<SingleTypeNode>(VisitSingleType)
                .CaseIs<MultiTypeNode>(VisitMultiType)
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
                .CaseIs<LogicalOrArithmeticExpression>(VisitLogicalOrArithmeticExpression)
                .CaseIs<If>(VisitIf)
                .CaseIs<Literal>(VisitLiteral)
                .CaseIs<IndexerExpression>(VisitIndexerExpression)
                .CaseIs<Try>(VisitTry)
                .CaseIs<Class>(VisitClass)
                .CaseIs<Property>(VisitProperty)
                .CaseIs<Field>(VisitField)
                .CaseIs<Expression>(VisitExpression)
                .Case(x => throw new Exception($"Unknown ast construction {x.GetType()}"), null)
                .Result();
        }

        public virtual T VisitLogicalOrArithmeticExpression(LogicalOrArithmeticExpression arg)
        {
            return default;
        }

        public virtual T VisitRoot(Root obj)
        {
            return default;
        }

        public virtual T VisitMultiType(MultiTypeNode obj)
        {
            return default;
        }

        public virtual T VisitSingleType(SingleTypeNode obj)
        {
            return default;
        }

        public virtual T VisitField(Field obj)
        {
            return default;
        }

        public virtual T VisitProperty(Property obj)
        {
            return default;
        }

        public virtual T VisitClass(Class obj)
        {
            return default;
        }

        public virtual T VisitTry(Try obj)
        {
            return default;
        }

        public virtual T VisitIndexerExpression(IndexerExpression obj)
        {
            return default;
        }

        public virtual T VisitPostDecrementStatement(PostDecrementStatement obj)
        {
            return default;
        }

        public virtual T VisitPostIncrementStatement(PostIncrementStatement obj)
        {
            return default;
        }

        public virtual T VisitArgument(Argument obj)
        {
            return default;
        }

        public virtual T VisitLiteral(Literal obj)
        {
            return default;
        }

        public virtual T VisitIf(If obj)
        {
            return default;
        }

        public virtual T VisitFieldExpression(FieldExpression obj)
        {
            return default;
        }

        public virtual T VisitCastExpression(CastExpression obj)
        {
            return default;
        }

        public virtual T VisitBinaryExpression(BinaryExpression obj)
        {
            return default;
        }

        public virtual T VisitReturn(Return obj)
        {
            return default;
        }

        public virtual T VisitAssigment(Assignment obj)
        {
            return default;
        }

        public virtual T VisitVariable(Variable obj)
        {
            //Do nothing
            return default;
        }

        public virtual T VisitInstructionsBody(InstructionsBodyNode obj)
        {
            return default;
        }

        public virtual T VisitType(TypeNode obj)
        {
            return default;
        }

        public virtual T VisitFunction(Function obj)
        {
            return default;
        }

        public virtual T VisitTypeBody(TypeBody obj)
        {
            return default;
        }

        public virtual T VisitModule(Module obj)
        {
            return default;
        }

        public virtual T VisitCallStatement(CallStatement obj)
        {
            return default;
        }

        public virtual T VisitCall(Call obj)
        {
            return default;
        }

        public virtual T VisitParameter(Parameter obj)
        {
            return default;
        }

        public virtual T VisitWhile(While obj)
        {
            return default;
        }

        public virtual T VisitFor(For obj)
        {
            return default;
        }

        public virtual T VisitName(Name obj)
        {
            return default;
        }

        public virtual T VisitExpression(Expression e)
        {
            throw new Exception("Unknown expression: " + e.GetType());
        }

        public virtual T VisitCompilationUnit(CompilationUnit cu)
        {
            return default;
        }
    }
}