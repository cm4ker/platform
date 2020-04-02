using System;
using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class NewArrayExpression : Expression
    {
        private readonly SreType _c;
        private readonly Expression _capacity;

        public NewArrayExpression(SreType c, Expression capacity)
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

    public class NewObjectExpression : Expression
    {
        private readonly SreConstructor _c;
        private readonly Expression[] _args;

        public NewObjectExpression(SreConstructor c, params Expression[] args)
        {
            _c = c;
            _args = args;
        }

        public override void Dump(TextWriter tw)
        {
            if (_c.IsStatic)
            {
                throw new Exception("you can't call static constructors");
            }

            tw.W("new ");

            _c.DumpRef(tw);

            using (tw.Parenthesis())
            {
                var isFirst = true;
                foreach (var arg in _args)
                {
                    if (!isFirst)
                        tw.W(",");
                    arg.Dump(tw);
                    isFirst = false;
                }
            }
        }
    }
}