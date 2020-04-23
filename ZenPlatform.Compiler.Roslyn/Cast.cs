using System.IO;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Cast : Expression
    {
        private readonly RoslynType _type;
        private readonly Expression _expression;

        public Cast(RoslynType type, Expression expression)
        {
            _type = type;
            _expression = expression;
        }

        public override void Dump(TextWriter tw)
        {
            using (tw.Parenthesis())
            {
                using (tw.Parenthesis())
                    _type.DumpRef(tw);

                _expression.Dump(tw);
            }
        }
    }
}