using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class LookUp : Expression
    {
        private readonly Expression _lookup;
        private readonly Expression _expression;

        public LookUp(Expression lookup, Expression expression)
        {
            _lookup = lookup;
            _expression = expression;
        }

        public override void Dump(TextWriter tw)
        {
            _expression.Dump(tw);

            tw.Dot();

            _lookup.Dump(tw);
        }
    }

    public class Is : Expression
    {
        private readonly SreType _type;
        private readonly Expression _expression;

        public Is(SreType type, Expression expression)
        {
            _type = type;
            _expression = expression;
        }

        public override void Dump(TextWriter tw)
        {
            _expression.Dump(tw);
            tw.W(" is ");
            _type.DumpRef(tw);
        }
    }

    public class RXIf : Expression
    {
        private readonly RBlockBuilder _else;
        private readonly RBlockBuilder _then;
        private readonly Expression _condition;

        public RXIf(RBlockBuilder @else, RBlockBuilder then, Expression condition)
        {
            _else = @else;
            _then = then;
            _condition = condition;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("if");

            using (tw.Parenthesis())
                _condition.Dump(tw);

            _then.Dump(tw);

            if (_else != null)
            {
                tw.W("else");
                _else.Dump(tw);
            }
        }
    }


    public class For : Expression
    {
        private readonly RBlockBuilder _blockBuilder;
        private readonly Expression _increment;
        private readonly Expression _condition;
        private readonly Expression _assign;

        public For(RBlockBuilder blockBuilder, Expression increment, Expression condition, Expression assign)
        {
            _blockBuilder = blockBuilder;
            _increment = increment;
            _condition = condition;
            _assign = assign;
        }

        public override void Dump(TextWriter tw)
        {
            using (tw.W("for").Parenthesis())
            {
                _assign?.Dump(tw);
                tw.Comma();
                _condition?.Dump(tw);
                tw.Comma();
                _increment?.Dump(tw);
            }

            _blockBuilder?.Dump(tw);
        }
    }
}