using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
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