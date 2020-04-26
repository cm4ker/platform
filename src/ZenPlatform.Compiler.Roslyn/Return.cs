using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Return : Expression
    {
        private readonly Expression _exp;

        public Return(Expression exp)
        {
            _exp = exp;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("return");

            if (_exp != null)
            {
                tw.Space();
                _exp.Dump(tw);
            }
        }
    }
}