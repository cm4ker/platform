using System;
using System.Collections.Generic;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast
{
    public abstract class AstVisitorBase<T>
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

        public virtual T Visit(AstNode visitable)
        {
            if (visitable is null) return default;
            ;

            return visitable.Accept(this);
        }


        public T VisitArrayTypeNode(ArrayTypeNode arg)
        {
            return DefaultVisit(arg);
            ;
        }

        public virtual T VisitLogicalOrArithmeticExpression(LogicalOrArithmeticExpression arg)
        {
            return DefaultVisit(arg);
            ;
        }

        public virtual T VisitRoot(Root obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitMultiType(UnionTypeNode obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitSingleType(SingleTypeNode obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitField(Field obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitProperty(Property obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitClass(Class obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitTry(Try obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitIndexerExpression(IndexerExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitPostDecrementStatement(PostDecrementExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitPostIncrementStatement(PostIncrementExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitArgument(Argument obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitLiteral(Literal obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitIf(If obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitFieldExpression(FieldExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitCastExpression(CastExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitBinaryExpression(BinaryExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitReturn(Return obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitAssigment(Assignment obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitVariable(Variable obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitInstructionsBody(BlockNode obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitType(TypeNode obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitFunction(Function obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitTypeBody(TypeBody obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitModule(Module obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitCall(Call obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitParameter(ParameterNode obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitWhile(While obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitFor(For obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitName(Name obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitExpression(Expression e)
        {
            throw new Exception("Unknown expression: " + e.GetType());
        }

        public virtual T VisitCompilationUnit(CompilationUnit cu)
        {
            return DefaultVisit(cu);
            ;
        }

        public virtual T DefaultVisit(AstNode node)
        {
            return default;
        }
    }
}