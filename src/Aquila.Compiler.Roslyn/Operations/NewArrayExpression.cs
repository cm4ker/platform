using System.IO;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class NewArrayExpression : Expression
    {
        private readonly RoslynType _c;
        private readonly Expression _capacity;

        public NewArrayExpression(RoslynType c, Expression capacity)
        {
            _c = c;
            _capacity = capacity;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("new ");

            _c.DumpRef(tw);
            using (tw.SquareBrace())
            {
                _capacity?.Dump(tw);
            }
        }
    }
}