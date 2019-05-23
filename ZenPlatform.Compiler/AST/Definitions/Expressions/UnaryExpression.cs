using Antlr4.Runtime;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.AST.Definitions.Expressions
{
    public abstract class UnaryExpression : Expression
    {
        protected UnaryExpression(ILineInfo li, Expression value) : base(li)
        {
            Value = value;
        }

        public Expression Value { get; }

        public override IType Type => Value.Type;
    }


    public class CastExpression : UnaryExpression
    {
        public IType CastType { get; }

        public CastExpression(ILineInfo token, Expression value, IType castType) : base(token, value)
        {
            CastType = castType;
        }

        public override IType Type => CastType;
    }

    public class IndexerExpression : UnaryExpression
    {
        public Expression Indexer { get; }


        public IndexerExpression(ILineInfo token, Expression indexer, Expression value) : base(token, value)
        {
            Indexer = indexer;
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
    }
}