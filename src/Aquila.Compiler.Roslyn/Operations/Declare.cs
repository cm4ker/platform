using System.IO;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class Declare : Expression
    {
        private readonly RoslynType _type;
        private readonly NameExpression _name;
        private readonly Expression _initializer;

        public Declare(Expression initializer, NameExpression name, RoslynType type)
        {
            _type = type;
            _name = name;
            _initializer = initializer;
        }

        public override void Dump(TextWriter tw)
        {
            _type.DumpRef(tw);
            tw.Space();
            _name.Dump(tw);

            if (_initializer != null)
            {
                tw.W(" = ");
                _initializer.Dump(tw);
            }
        }
    }
}