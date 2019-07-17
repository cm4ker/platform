using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Expressions
{
    public abstract class UnaryExpression : Expression
    {
        private const int VALUE_SLOT = 0;

        private Expression _value;

        protected UnaryExpression(ILineInfo li, Expression value) : base(li)
        {
            _value = Children.SetSlot(value, VALUE_SLOT);
        }

        public Expression Value
        {
            get => _value;
        }

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
        //Unary expression has already exp on slot = 0
        private const int INDEXER_SLOT = 1;

        public Expression Indexer { get; }

        public IndexerExpression(ILineInfo token, Expression indexer, Expression value) : base(token, value)
        {
            Indexer = Children.SetSlot(indexer, INDEXER_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIndexerExpression(this);
        }
    }

    public class LogicalOrArithmeticExpression : UnaryExpression
    {
        public UnaryOperatorType OperaotrType { get; }

        public LogicalOrArithmeticExpression(ILineInfo token, Expression value, UnaryOperatorType operaotrType) :
            base(token, value)
        {
            OperaotrType = operaotrType;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitLogicalOrArithmeticExpression(this);
        }
    }
}