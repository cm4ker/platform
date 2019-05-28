using Antlr4.Runtime;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;

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
        public IType CastType { get; set; }

        public CastExpression(ILineInfo token, Expression value, IType castType) : base(token, value)
        {
            CastType = castType;
        }

        public override IType Type => CastType;

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Value);
        }
    }

    public class IndexerExpression : UnaryExpression
    {
        public Expression Indexer { get; }


        public IndexerExpression(ILineInfo token, Expression indexer, Expression value) : base(token, value)
        {
            Indexer = indexer;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Indexer);
            visitor.Visit(Value);
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Value);
        }
    }
}