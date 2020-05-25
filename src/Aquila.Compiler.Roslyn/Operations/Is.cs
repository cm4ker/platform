using System.IO;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class Is : Expression
    {
        private readonly IType _type;
        private readonly Expression _expression;

        public Is(IType type, Expression expression)
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
}