using System.IO;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Call : Expression
    {
        private readonly RoslynInvokableBase _method;
        private readonly Expression[] _args;

        public Call(RoslynInvokableBase method, params Expression[] args)
        {
            _method = method;
            _args = args;
        }

        public override void Dump(TextWriter tw)
        {
            if (_method.IsStatic)
            {
                _method.DeclaringType.DumpRef(tw);
                tw.Dot();
            }

            _method.DumpRef(tw);

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