using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class RXThrow : Expression
    {
        private readonly Expression _expression;

        public RXThrow(Expression expression)
        {
            _expression = expression;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("throw ");
            _expression.Dump(tw);
        }
    }
}