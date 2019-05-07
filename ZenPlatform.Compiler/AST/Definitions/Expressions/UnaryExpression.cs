using System;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Expression
{
    public abstract class UnaryExpression : Infrastructure.Expression
    {
        protected UnaryExpression(Infrastructure.Expression value)
        {
            Value = value;
        }

        public Infrastructure.Expression Value { get; }

        public override ZType Type => Value.Type;
    }


    public class CastExpression : UnaryExpression
    {
        public ZType CastType { get; }

        public CastExpression(Infrastructure.Expression value, ZType castType) : base(value)
        {
            CastType = castType;
        }

        public override ZType Type => CastType;
    }

    public class IndexerExpression : UnaryExpression
    {
        public Infrastructure.Expression Indexer { get; }


        public IndexerExpression(Infrastructure.Expression indexer, Infrastructure.Expression value) : base(value)
        {
            Indexer = indexer;
        }
    }

    public class LogicalOrArithmeticExpression : UnaryExpression
    {
        public UnaryOperatorType Type { get; }

        public LogicalOrArithmeticExpression(Infrastructure.Expression value, UnaryOperatorType type) : base(value)
        {
            Type = type;
        }
    }
}