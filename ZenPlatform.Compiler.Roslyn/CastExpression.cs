using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class CastExpression : Expression
    {
        private readonly SreType _castType;
        private readonly Expression _exp;

        public CastExpression(SreType castType, Expression exp)
        {
            _castType = castType;
            _exp = exp;
        }

        public override void Dump(TextWriter tw)
        {
            using (tw.Parenthesis())
            {
                using (tw.Parenthesis())
                    _castType.DumpRef(tw);

                using (tw.Parenthesis())
                    _exp.Dump(tw);
            }
        }
    }
}