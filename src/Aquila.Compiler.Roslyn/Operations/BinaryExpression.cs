using System;
using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class BinaryExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly BKind _kind;

        public BinaryExpression(Expression right, Expression left, BKind kind)
        {
            _left = left;
            _right = right;
            _kind = kind;
        }

        public override void Dump(TextWriter tw)
        {
            var token = _kind switch
            {
                BKind.Plus => "+",
                BKind.Minus => "-",
                BKind.Div => "/",
                BKind.Multiply => "*",
                BKind.Eq => "==",
                BKind.Gt => ">",
                BKind.Lt => "<",
                BKind.Get => ">=",
                BKind.Let => "<=",
                BKind.Ne => "!=",
                BKind.Rem => "%",
                BKind.And => "&&",
                BKind.Or => "||",

                _ => throw new Exception()
            };

            _left.Dump(tw);
            tw.Space().W(token).Space();
            _right.Dump(tw);
        }
    }
}