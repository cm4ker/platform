using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Is : Expression
    {
        private readonly RoslynType _type;
        private readonly Expression _expression;

        public Is(RoslynType type, Expression expression)
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