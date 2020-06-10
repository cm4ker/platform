using System;
using System.IO;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class NewObjectExpression : Expression
    {
        private readonly IConstructor _c;
        private readonly Expression[] _args;

        public NewObjectExpression(IConstructor c, params Expression[] args)
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