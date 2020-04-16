using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
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
}