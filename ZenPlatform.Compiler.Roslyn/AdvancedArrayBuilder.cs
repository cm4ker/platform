using System.Collections.Generic;
using System.IO;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class AdvancedArrayBuilder : Expression
    {
        private readonly Expression _capacity;
        private readonly RoslynType _type;
        private readonly RBlockBuilder _parentBlock;

        private readonly List<Expression> _args = new List<Expression>();

        public AdvancedArrayBuilder(Expression capacity, RoslynType type, RBlockBuilder parentBlock)
        {
            _capacity = capacity;
            _type = type;
            _parentBlock = parentBlock;
        }

        public RBlockBuilder Block => _parentBlock;

        public AdvancedArrayBuilder PopArg()
        {
            _args.Add((Expression) Block.Pop());
            return this;
        }

        public RBlockBuilder EndBuild()
        {
            _parentBlock.Push(this);
            return _parentBlock;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W("new ");

            _type.DumpRef(tw);
            using (tw.SquareBrace())
            {
                _capacity?.Dump(tw);
            }

            using (tw.CurlyBrace())
            {
                var wasFirst = false;
                foreach (var arg in _args)
                {
                    if (wasFirst)
                        tw.W(",");

                    arg.Dump(tw);

                    wasFirst = true;
                }
            }

            //new Test[10] {exp1, exp2, exp3}
        }
    }
}