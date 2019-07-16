using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Expressions
{
    public abstract class UnaryExpression : Expression
    {
        
        
        protected UnaryExpression(ILineInfo li, Expression value) : base(li)
        {
            Value = value;
        }

        public Expression Value { get; set; }

        public override TypeNode Type => Value.Type;
    }


    public class CastExpression : UnaryExpression
    {
        public TypeNode CastType { get; set; }

        public CastExpression(ILineInfo token, Expression value, TypeNode castType) : base(token, value)
        {
            CastType = castType;
        }

        public override TypeNode Type => CastType;

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCastExpression(this);
        }
    }

    public class IndexerExpression : UnaryExpression
    {
        public Expression Indexer { get; }


        public IndexerExpression(ILineInfo token, Expression indexer, Expression value) : base(token, value)
        {
            Indexer = indexer;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerExpression(this);
        }
    }

    public class LogicalOrArithmeticExpression : UnaryExpression
    {
        public UnaryOperatorType Type { get; }

        public LogicalOrArithmeticExpression(ILineInfo token, Expression value, UnaryOperatorType type) :
            base(token, value)
        {
            Type = type;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLogicalOrArithmeticExpression(this);
        }
    }
}