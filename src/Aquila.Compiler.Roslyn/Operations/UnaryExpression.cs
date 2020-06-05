using System;
using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class UnaryExpression : Expression
    {
        private readonly Expression _exp;
        private readonly UKind _kind;

        public UnaryExpression(Expression exp, UKind kind)
        {
            _exp = exp;
            _kind = kind;
        }

        public override void Dump(TextWriter tw)
        {
            switch (_kind)
            {
                case UKind.PostInc:
                    _exp.Dump(tw);
                    tw.W("++");
                    break;
                case UKind.PreInc:
                    tw.W("++");
                    _exp.Dump(tw);
                    break;
                case UKind.PostDec:
                    _exp.Dump(tw);
                    tw.W("--");
                    break;
                case UKind.PreDec:
                    tw.W("--");
                    _exp.Dump(tw);
                    break;
                case UKind.Not:
                    tw.W("!");
                    using (tw.Parenthesis())
                        _exp.Dump(tw);
                    break;
                case UKind.Negative:
                    tw.W("-");
                    _exp.Dump(tw);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}