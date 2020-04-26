using System.IO;

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
}