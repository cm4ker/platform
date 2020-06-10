using System.IO;

namespace Aquila.Compiler.Roslyn.Operations
{
    public class GoTo : Expression
    {
        private readonly RLabel _label;

        public GoTo(RLabel label)
        {
            _label = label;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("goto ");
            _label.Dump(tw);
        }
    }


    public class RXFor : Expression
    {
        private readonly RoslynEmitter _blockBuilder;
        private readonly Expression _increment;
        private readonly Expression _condition;
        private readonly Expression _assign;

        public RXFor(RoslynEmitter blockBuilder, Expression increment, Expression condition, Expression assign)
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