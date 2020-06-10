using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class RXIf : Expression
    {
        private readonly RoslynEmitter _else;
        private readonly RoslynEmitter _then;
        private readonly Expression _condition;

        public RXIf(RoslynEmitter @else, RoslynEmitter then, Expression condition)
        {
            _else = @else;
            _then = then;
            _condition = condition;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("if");

            using (tw.Parenthesis())
                _condition.Dump(tw);

            _then.Dump(tw);

            if (_else != null)
            {
                tw.W("else");
                _else.Dump(tw);
            }
        }
    }

    public class RXWhile : Expression
    {
        private readonly RoslynEmitter _block;
        private readonly Expression _condition;

        public RXWhile(RoslynEmitter block, Expression condition)
        {
            _block = block;
            _condition = condition;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("while");

            using (tw.Parenthesis())
                _condition.Dump(tw);

            _block.Dump(tw);
        }
    }

    public class RXTry : Expression
    {
        private readonly RoslynEmitter _finallyBlock;
        private readonly RoslynEmitter _catchBlock;
        private readonly RoslynEmitter _tryBlock;

        public RXTry(RoslynEmitter finallyBlock, RoslynEmitter catchBlock, RoslynEmitter tryBlock)
        {
            _finallyBlock = finallyBlock;
            _catchBlock = catchBlock;
            _tryBlock = tryBlock;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("try");
            _tryBlock.Dump(tw);

            if (_catchBlock != null)
            {
                tw.W("catch");
                _catchBlock.Dump(tw);
            }

            if (_finallyBlock != null)
            {
                tw.W("finally");
                _finallyBlock.Dump(tw);
            }
        }
    }
}