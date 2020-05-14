using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class RXIf : Expression
    {
        private readonly RBlockBuilder _else;
        private readonly RBlockBuilder _then;
        private readonly Expression _condition;

        public RXIf(RBlockBuilder @else, RBlockBuilder then, Expression condition)
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
        private readonly RBlockBuilder _block;
        private readonly Expression _condition;

        public RXWhile(RBlockBuilder block, Expression condition)
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
        private readonly RBlockBuilder _finallyBlock;
        private readonly RBlockBuilder _catchBlock;
        private readonly RBlockBuilder _tryBlock;

        public RXTry(RBlockBuilder finallyBlock, RBlockBuilder catchBlock, RBlockBuilder tryBlock)
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