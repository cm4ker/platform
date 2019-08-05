using System;
using System.Collections.Generic;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using Attribute = ZenPlatform.Language.Ast.Definitions.Attribute;

namespace ZenPlatform.Language.Ast
{
    public abstract class AstVisitorBase<T>
    {
        private Stack<SyntaxNode> _visitStack;
        private bool _break;

        protected Stack<SyntaxNode> VisitStack => _visitStack;

        public AstVisitorBase()
        {
            _visitStack = new Stack<SyntaxNode>();
        }

        public virtual T Visit(SyntaxNode visitable)
        {
            if (visitable is null) return default;
            ;

            return visitable.Accept(this);
        }


        public T VisitArrayTypeNode(ArrayTypeSyntax arg)
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

        public virtual T VisitMultiType(UnionTypeSyntax obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitSingleType(SingleTypeSyntax obj)
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

        public virtual T VisitPostDecrementExpression(PostDecrementExpression obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitPostIncrementExpression(PostIncrementExpression obj)
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

        public virtual T VisitGetFieldExpression(GetFieldExpression obj)
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

        public virtual T VisitAssignment(Assignment obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitVariable(Variable obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitInstructionsBody(Block obj)
        {
            return DefaultVisit(obj);
            ;
        }

        public virtual T VisitType(TypeSyntax obj)
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

        public virtual T VisitParameter(Parameter obj)
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
        }

        public virtual T VisitExpression(Expression e)
        {
            return DefaultVisit(e);
        }


        public virtual T VisitExpressionStatement(ExpressionStatement e)
        {
            return DefaultVisit(e);
        }


        public virtual T VisitCompilationUnit(CompilationUnit cu)
        {
            return DefaultVisit(cu);
            ;
        }

        public virtual T DefaultVisit(SyntaxNode node)
        {
            return default;
        }

        public virtual T VisitAttribute(Attribute obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitDoStatement(DoWhile obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitBlock(Block obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitPrimitiveTypeSyntax(PrimitiveTypeSyntax obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitSingleTypeSyntax(SingleTypeSyntax obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitArrayTypeSyntax(ArrayTypeSyntax obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitUnionTypeSyntax(UnionTypeSyntax obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitDoWhile(DoWhile obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitNamespaceBase(NamespaceBase obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitNamespace(Namespace obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitClassNamespace(ClassNamespace obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitThrow(Throw obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitMatch(Match obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitMatchAtom(MatchAtom obj)
        {
            return DefaultVisit(obj);
        }

        public virtual T VisitAssignFieldExpression(AssignFieldExpression obj)
        {
            return DefaultVisit(obj);
        }
    }

    public class AstWalker<T> : AstVisitorBase<T>
    {
        public override T DefaultVisit(SyntaxNode node)
        {
            Console.WriteLine($"We are visit: {node}");

            foreach (var child in node.Childs)
            {
                Visit(child as SyntaxNode);
            }

            return base.DefaultVisit(node);
        }
    }
}