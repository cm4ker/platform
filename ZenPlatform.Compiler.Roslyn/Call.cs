using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Cast : Expression
    {
        private readonly SreType _type;
        private readonly Expression _expression;

        public Cast(SreType type, Expression expression)
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

    public class RXThrow : Expression
    {
        private readonly Expression _expression;

        public RXThrow(Expression expression)
        {
            _expression = expression;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("throw ");
            _expression.Dump(tw);
        }
    }

    public class Call : Expression
    {
        private readonly SreInvokableBase _method;
        private readonly Expression[] _args;

        public Call(SreInvokableBase method, params Expression[] args)
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