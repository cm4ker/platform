using System.IO;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class Cast : Expression
    {
        private readonly IType _type;
        private readonly Expression _expression;

        public Cast(IType type, Expression expression)
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
}