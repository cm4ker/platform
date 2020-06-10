using System.IO;

namespace Aquila.Compiler.Roslyn
{
    internal class Statement
    {
        private readonly Expression _exp;

        public Statement(Expression exp)
        {
            _exp = exp;
        }

        public void Dump(TextWriter tw)
        {
            _exp.Dump(tw);
            tw.W(";");
        }
    }
}