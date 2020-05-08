using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class Assign : Expression
    {
        private readonly Expression _exp;
        private readonly Expression _value;

        public Assign(Expression value, Expression exp)
        {
            _exp = exp;
            _value = value;
        }

        public override void Dump(TextWriter tw)
        {
            _exp.Dump(tw);
            tw.W(" = ");
            _value.Dump(tw);
        }
    }
}